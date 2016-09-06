namespace FacebookApiWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Http;
    using System.IO;
    using System.Security.Cryptography;

    public class AudienceManager
    {
        private readonly string accessToken;

        public AudienceManager(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public string CreateCustomAudience(string name, string description, string adAccountId)
        {
            return CreateCustomAudience(name, "CUSTOM", description, adAccountId);
        }

        public string CreateCustomAudience(string name, string subtype, string description, string adAccountId)
        {
            var client = new HttpClient();

            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("subtype", subtype),
                new KeyValuePair<string, string>("description", description),
                new KeyValuePair<string, string>("access_token", accessToken)
            });

            var url = string.Format("https://graph.facebook.com/v2.7/act_{0}/customaudiences", adAccountId);

            HttpResponseMessage response = client.PostAsync(url, requestContent).Result;

            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(responseContent.ReadAsStreamAsync().Result))
            {
                return reader.ReadToEndAsync().Result;
            }
        }

        public string AddUsersToCustomAudience(string customAudienceId, params string[] users)
        {
            return this.AddUsersToCustomAudience(customAudienceId, new string[] { "EMAIL" }, users);
        }

        public string AddUsersToCustomAudience(string customAudienceId, string[] schema, params string[] users)
        {
            var client = new HttpClient();

            var shemeContent = string.Format("[{0}]", string.Join(",", schema));
            var dataContent = string.Format("[{0}]", string.Join(",", schema.Select(x => Hash(x))));

            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("schema", shemeContent),
                new KeyValuePair<string, string>("data", dataContent)
            });

            var url = string.Format("https://graph.facebook.com/v2.7/{0}/users", customAudienceId);

            HttpResponseMessage response = client.PostAsync(url, requestContent).Result;

            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(responseContent.ReadAsStreamAsync().Result))
            {
                return reader.ReadToEndAsync().Result;
            }
        }

        private String Hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
