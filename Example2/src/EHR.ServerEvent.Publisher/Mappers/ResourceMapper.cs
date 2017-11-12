using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using EHR.ServerEvent.Infrastructure;
using EHR.ServerEvent.Infrastructure.Contract;

namespace EHR.ServerEvent.Publisher.Mappers
{
    public class Mapper
    {
        private readonly IEnumerable<ResourceInformationReader.ResourceInformationReader> _actionReaders;

        public Mapper(IEnumerable<ResourceInformationReader.ResourceInformationReader> actionReaders)
        {
            _actionReaders = actionReaders;
        }

        public virtual IEnumerable<ServerEventMessage> ConvertToServerEvent(ActionMetadata actionMetadata)
        {
            var resourceinfoBuilder =_actionReaders.FirstOrDefault(a => a.CanRead(actionMetadata.ActionRequest.HttpMethod));
            if (resourceinfoBuilder == null)
            {
                throw new Exception($"Не зарегистрировано обработчиков для метода {actionMetadata.ActionRequest.HttpMethod}");
            }
            var serverEventMessageRegistered = actionMetadata.ExecutionDateTime.ToUniversalTime().ToString("u");
            var serverEventMessageActionCode = actionMetadata.ActionRequest.HttpMethod;
            var outcome = OutcomeBuilder(actionMetadata.ActionResponce.StatusCode);
            var clientIp = ClientIP.ClientIpFromRequest(actionMetadata.ActionRequest.Header, false);
            var source = actionMetadata.ActionRequest.Header
                    .FirstOrDefault(h => h.Key.Equals("Host", StringComparison.CurrentCultureIgnoreCase)).Value;
           
            var resourceInfo = resourceinfoBuilder.ResourceInformation(actionMetadata);


            JwtSecurityToken tokenS = null;
            var token = actionMetadata.ActionRequest.Header.FirstOrDefault(h => h.Key.Equals("Authorization"))
                .Value?.Split(' ')[1];
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                tokenS = handler.ReadToken(token) as JwtSecurityToken;
            }

           

            

            foreach (var serverEventResourceInfromation in resourceInfo)
            {
                yield return new ServerEventMessage
                {
                    ResourceType = serverEventResourceInfromation.ResourceType,
                    ResourceVersionId = serverEventResourceInfromation.ResourceVersion,
                    ResourceId = serverEventResourceInfromation.ResourceId,

                    PatientId = serverEventResourceInfromation.PatientId,
                    PatientVersionId = serverEventResourceInfromation.PatientVersion,

                    EncounterId = serverEventResourceInfromation.EncounterId,
                    EncounterVersionId = serverEventResourceInfromation.EncounterVersionId,

                    ActionScope = serverEventResourceInfromation.Scope,


                    Registered = serverEventMessageRegistered,
                    ActionCode = serverEventMessageActionCode,

                    Outcome = outcome.Key,
                    OutcomeDesc = outcome.Value,

                    AccessPoint = clientIp,

                    Source = source,


                    ClientId =tokenS==null?null: string.Join("  ",tokenS.Audiences),
                    Claims = tokenS?.Claims.Select(c=>new KeyValuePair<string, string>(c.Type, c.Value)).ToList(),
                    UserId = tokenS?.Subject,
                
                    Resource = serverEventResourceInfromation.ResourceJson
                };
            }
               
             
        }


        public static KeyValuePair<string,string> OutcomeBuilder(int statusCode)
        {
           
            if (statusCode < 400)
            {
              
                return new KeyValuePair<string, string>("success", $"Выполнено успешно. Код результата {statusCode}");
            }
            return new KeyValuePair<string, string>("error", $"Выполнено с ошибками. Код результата {statusCode}");
           


        }


    }


    /// <summary>
    /// Информация для ServerEvent которую можно получить из ресурса
    /// </summary>
    public class ServerEventResourceInfromation
    {
        /// <summary>
        /// Ссылка на ресурс, над которым выполнялось действие, и его версию.
       /// </summary>
        public string ResourceId { get; set; }
        public string ResourceVersion { get; set; }
        public string ResourceType { get; set; }

        /// <summary>
        /// Ссылка на ресурс пациент, над которым выполнялось действие, и его версию.
        /// </summary>
        public string PatientId { get; set; }
        public string PatientVersion { get; set; }

        /// <summary>
        ///  Ссылка на ресурс encounter, в рамках которого происходило действие, и его версия.
        /// </summary>
        public string EncounterId { get; set; }
        public string EncounterVersionId { get; set; }


        /// <summary>
        ///В рамках какого действия произошло событие (Регистрация карты, Закрытие больничного ...).
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Сериализованный ресурс
        /// </summary>
        public string ResourceJson { get; set; }
    }



    public static class ClientIP
    {
       
        public static string ClientIpFromRequest(IEnumerable<KeyValuePair<string,string>> headers, bool skipPrivate)
        {
            foreach (var item in SHeaderItems)
            {
                var ipString = headers.FirstOrDefault(h=>h.Key.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase)).Value;

                if (string.IsNullOrEmpty(ipString))
                    continue;

                return ipString;
            }

            return null;
        }

    

       

      
       
        private sealed class HeaderItem
        {
            public readonly string Key;
           
            public HeaderItem(string key)
            {
                Key = key;
               
            }
        }

      
        private static readonly HeaderItem[] SHeaderItems =
            new HeaderItem[] {
                new HeaderItem("HTTP_CLIENT_IP"),
                new HeaderItem("HTTP_X_FORWARDED_FOR"),
                new HeaderItem("HTTP_X_FORWARDED"),
                new HeaderItem("HTTP_X_CLUSTER_CLIENT_IP"),
                new HeaderItem("HTTP_FORWARDED_FOR"),
                new HeaderItem("HTTP_FORWARDED"),
                new HeaderItem("HTTP_VIA"),
                new HeaderItem("REMOTE_ADDR"),
                new HeaderItem("X-Original-For"), 
            };
    }



























}