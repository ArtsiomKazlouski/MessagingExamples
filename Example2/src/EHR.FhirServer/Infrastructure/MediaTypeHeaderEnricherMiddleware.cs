using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace EHR.FhirServer.Infrastructure
{
    public static class MediaTypeHeaderEnricherMiddlewareExtentions
    {
        public static IServiceCollection UseMediaTypeHeaderEnricher(this IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan
                .FromAssembliesOf(typeof(MediaTypeHeaderEnricherMiddleware))
                .AddClasses(filter => filter.AssignableTo<MediaTypeHeaderEnricherMiddleware.AceeptHeaderEnricher>())
                .As<MediaTypeHeaderEnricherMiddleware.AceeptHeaderEnricher>());
            return serviceCollection;
        }

        public static IApplicationBuilder UseMediaTypeHeaderEnricher(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<MediaTypeHeaderEnricherMiddleware>();
            return applicationBuilder;
        }
    }

    public class MediaTypeHeaderEnricherMiddleware
    {
        private const string UrlFormatKey = "_format";
        private readonly IEnumerable<AceeptHeaderEnricher> _aceeptHeaderEnrichers;
        private readonly RequestDelegate _next;
        private readonly IOptions<MvcOptions> _options;

        public MediaTypeHeaderEnricherMiddleware(RequestDelegate next, IOptions<MvcOptions> options,
            IEnumerable<AceeptHeaderEnricher> aceeptHeaderEnrichers)
        {
            _next = next;
            _options = options;
            _aceeptHeaderEnrichers = aceeptHeaderEnrichers;
        }


        public async Task Invoke(HttpContext context)
        {
            var aceeptHeaderEnricherContext = new AceeptHeaderEnricherContext(context, _options);

            foreach (var aceeptHeaderEnricher in _aceeptHeaderEnrichers.Where(a =>
                a.CanExecute(aceeptHeaderEnricherContext)))
                aceeptHeaderEnricher.Execute(aceeptHeaderEnricherContext);

            
            var acceptHeaders = new List<MediaTypeSegmentWithQuality>();
            AcceptHeaderParser.ParseAcceptHeader(context.Request.Headers[HeaderNames.Accept], acceptHeaders);
            context.Features.Set<IOutputFormatter>(OutputFormatterSelector.SelectFormatter(_options.Value.OutputFormatters.OfType<TextOutputFormatter>(), acceptHeaders));


            await _next(context);
        }



        public class OutputFormatterSelector
        {

            public static IOutputFormatter SelectFormatter(IEnumerable<OutputFormatter> formatters, IEnumerable<MediaTypeSegmentWithQuality> acceptHeader)
            {

                foreach (var mediaType in GetSortedAcceptableMediaTypes(acceptHeader.ToList()))
                {
                    foreach (var formatter in formatters)
                    {
                        if (formatter.SupportedMediaTypes.Any(m => m.Equals(mediaType.MediaType.Value)))
                            return formatter;
                    }
                }


                return null;
            }


            private static readonly Comparison<MediaTypeSegmentWithQuality> SortFunction = (left, right) => left.Quality > right.Quality ? -1 : (left.Quality == right.Quality ? 0 : 1);



            private static IEnumerable<MediaTypeSegmentWithQuality> GetSortedAcceptableMediaTypes(List<MediaTypeSegmentWithQuality> acceptHeaders)
            {
                if (acceptHeaders.Select(m => new MediaType(m.MediaType)).Any(mediaType => mediaType.MatchesAllSubTypes && mediaType.MatchesAllTypes))
                {
                    return acceptHeaders;
                }

                acceptHeaders.Sort(SortFunction);

                return acceptHeaders;
            }
            

        }


        public class AceeptHeaderEnricherContext
        {
            public AceeptHeaderEnricherContext(HttpContext context, IOptions<MvcOptions> options)
            {
                ContentType = context.Request.GetTypedHeaders().ContentType;
                Accept = context.Request.GetTypedHeaders().Accept;
                AcceptCharset = context.Request.GetTypedHeaders().AcceptCharset;


                var format = (string) context.Request.Query[UrlFormatKey];
                if (format != null)
                {
                    FormatFromQuery = options.Value.FormatterMappings.GetMediaTypeMappingForFormat(format) ?? format;
                }
               
                Context = context;
            }

            public MediaTypeHeaderValue ContentType { get; }
            public IEnumerable<MediaTypeHeaderValue> Accept { get; }
            public IEnumerable<StringWithQualityHeaderValue> AcceptCharset { get; }
            public string FormatFromQuery { get; }

            public HttpContext Context { get; }
        }

        public abstract class AceeptHeaderEnricher
        {
            protected string BuildQualityString(MediaTypeHeaderValue headerValue)
            {
                return headerValue.Quality == null ? "" : $";q={headerValue.Quality}";
            }


            public abstract bool CanExecute(AceeptHeaderEnricherContext context);

            public abstract void Execute(AceeptHeaderEnricherContext context);
        }

        public class ContentTypeExistAcceptNotExist : AceeptHeaderEnricher
        {
            public override bool CanExecute(AceeptHeaderEnricherContext context)
            {
                if (context.ContentType == null)
                    return false;

                //Если установлен Accept  из ContentType устанавливать не нужно
                if (context.Accept != null && context.Accept.Any())
                    return false;

                //Если есть формат с Url то Accept из ContentType устанавливать не нужно
                if (string.IsNullOrEmpty(context.FormatFromQuery)==false)
                    return false;

                return true;
            }

            public override void Execute(AceeptHeaderEnricherContext context)
            {
                context.Context.Request.Headers.Append(HeaderNames.Accept,
                    $"{context.ContentType.MediaType}{BuildQualityString(context.ContentType)}");
            }
        }


        public class ContentTypeExistAcceptCharsetNotExist : AceeptHeaderEnricher
        {
            public override bool CanExecute(AceeptHeaderEnricherContext context)
            {
                if (string.IsNullOrEmpty(context.ContentType?.Charset))
                    return false;

              
                if (context.Accept != null && context.Accept.Any(a=>string.IsNullOrEmpty(a.Charset)==false))
                    return false;

                //Если установлен AcceptCharset  из ContentType устанавливать не нужно
                if (context.AcceptCharset != null && context.AcceptCharset.Any())
                    return false;


                return true;
            }

            public override void Execute(AceeptHeaderEnricherContext context)
            {
                context.Context.Request.Headers.Append(HeaderNames.AcceptCharset,
                    $"{context.ContentType.Charset}{BuildQualityString(context.ContentType)}");
            }
        }


        public class AcceptExistFormatNotExist : AceeptHeaderEnricher
        {
            public override bool CanExecute(AceeptHeaderEnricherContext context)
            {
                if (context.Accept == null || context.Accept.Any() == false)
                    return false;

                //Если есть формат с Url то Accept устанавливать не нужно
                if (string.IsNullOrEmpty(context.FormatFromQuery) == false)
                    return false;

                return true;
            }

            public override void Execute(AceeptHeaderEnricherContext context)
            {
                context.Context.Request.Headers.Remove(HeaderNames.Accept);
                context.Context.Request.Headers.Append(HeaderNames.Accept,
                    string.Join(",", context.Accept.Select(a => $"{a.MediaType}{BuildQualityString(a)}")));
            }
        }


        public class AcceptExistCharSetNotExist : AceeptHeaderEnricher
        {
            public override bool CanExecute(AceeptHeaderEnricherContext context)
            {
              
                if (context.Accept == null || context.Accept.Any(a => string.IsNullOrEmpty(a.Charset) == false) == false)
                    return false;

                if (context.AcceptCharset != null && context.AcceptCharset.Any())
                    return false;

                return true;
            }

            public override void Execute(AceeptHeaderEnricherContext context)
            {
                context.Context.Request.Headers.Append(HeaderNames.AcceptCharset,
                    string.Join(",", context.Accept.Select(a => $"{a.Charset}{BuildQualityString(a)}")));
            }
        }

        public class FormatInQueryExist : AceeptHeaderEnricher
        {
            public override bool CanExecute(AceeptHeaderEnricherContext context)
            {
                if (string.IsNullOrEmpty(context.FormatFromQuery))
                    return false;


                return true;
            }

            public override void Execute(AceeptHeaderEnricherContext context)
            {
                context.Context.Request.Headers.Remove(HeaderNames.Accept);
                context.Context.Request.Headers.Append(HeaderNames.Accept, context.FormatFromQuery);
            }
        }
    }
}