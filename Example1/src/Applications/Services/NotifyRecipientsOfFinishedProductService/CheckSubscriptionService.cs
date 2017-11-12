using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ExchangeManagement.Contract.Messages;
using Thinktecture.IdentityModel.Client;

namespace NotifyRecipientsOfFinishedProductService
{
    public class CheckSubscriptionService: ICheckSubscriptionService
    {
        private readonly HttpClient _client;
        private readonly Func<TokenResponse> _tokenFactory;

        public CheckSubscriptionService()
        {
            //_client = client ?? throw new ArgumentNullException(nameof(client));
            //_tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        public bool Check(MessageMetadata message, string query)
        {
            //var token = _tokenFactory.Invoke();

            //Log.Verbose($"DemoPicture search token={token.AccessToken}");

            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            //var nameValueCollection = HttpUtility.ParseQueryString(query);
            //var pairs = nameValueCollection.AllKeys.SelectMany(nameValueCollection.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));

            //var searchResponce = _client.PostAsync("demopictures/search",new FormUrlNoLimitEncodedContent(pairs)).Result;

            //Log.Verbose($"DemoPicture search responce code: {searchResponce.StatusCode}");

            //return searchResponce.Content.ReadAsAsync<PagedResult<DemoPicture>>().Result;

            return message.Content.EndsWith(query,StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class FormUrlNoLimitEncodedContent : ByteArrayContent
    {
        public FormUrlNoLimitEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(EncodeContent(nameValueCollection))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        static byte[] EncodeContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
                throw new ArgumentNullException("nameValueCollection");

            //
            // Serialization as application/x-www-form-urlencoded
            //
            // Element nodes selected for inclusion are encoded as EltName=value{sep}, where = is a literal
            // character, {sep} is the separator character from the separator attribute on submission,
            // EltName represents the element local name, and value represents the contents of the text node.
            //
            // The encoding of EltName and value are as follows: space characters are replaced by +, and then
            // non-ASCII and reserved characters (as defined by [RFC 2396] as amended by subsequent documents
            // in the IETF track) are escaped by replacing the character with one or more octets of the UTF-8
            // representation of the character, with each octet in turn replaced by %HH, where HH represents
            // the uppercase hexadecimal notation for the octet value and % is a literal character. Line breaks
            // are represented as "CR LF" pairs (i.e., %0D%0A).
            //
            var sb = new List<byte>();
            foreach (var item in nameValueCollection)
            {
                if (sb.Count != 0)
                    sb.Add((byte)'&');

                var data = SerializeValue(item.Key);
                if (data != null)
                    sb.AddRange(data);
                sb.Add((byte)'=');

                data = SerializeValue(item.Value);
                if (data != null)
                    sb.AddRange(data);
            }

            return sb.ToArray();
        }

        static byte[] SerializeValue(string value)
        {
            if (value == null)
                return null;

            value = UrlEncode(value).Replace("%20", "+");
            return Encoding.ASCII.GetBytes(value);
        }


        static string UrlEncode(string input)
        {
            const int maxLength = 30000;
            if (input.Length <= maxLength)
                return Uri.EscapeDataString(input);

            StringBuilder sb = new StringBuilder(input.Length * 2);
            int index = 0;
            while (index < input.Length)
            {
                int length = Math.Min(input.Length - index, maxLength);
                string subString = input.Substring(index, length);
                sb.Append(Uri.EscapeDataString(subString));
                index += subString.Length;
            }

            return sb.ToString();
        }
    }
}
