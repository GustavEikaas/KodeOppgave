using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BouvetWebApp.Interfaces;
using BouvetWebApp.Models;
using Newtonsoft.Json;

namespace BouvetWebApp.Data
{
    public class ExternalApi
    {
        private const string PingUrl = @"https://data.brreg.no/enhetsregisteret/api/";
        private const string Query = @"https://data.brreg.no/enhetsregisteret/api/enheter?size=1000";

        public async Task<IEnumerable<Enheter>> FetchDataFromExternalApi()
        {
            if (!PingApi()) return null;
            
            using var client = new HttpClient();
            using var res = await client.GetAsync(Query);
            using var content = res.Content;
                
            var data = await content.ReadAsStringAsync();
            var updateList = JsonConvert.DeserializeObject<Root>(data);
            data = null;

            if (updateList != null && VerifyParse(updateList._embedded.Enheter.FirstOrDefault()))
            {
                return updateList._embedded.Enheter;
            }
            Console.WriteLine("External API returned no data or parse failed");
            return null;
        }
        
        private bool VerifyParse(Enheter enhet)
        {
            return !string.IsNullOrEmpty(enhet.Navn) && enhet.Organisasjonsnummer != 0 && !string.IsNullOrEmpty(enhet.Organisasjonsform.Kode);
        }

        private bool PingApi()
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(PingUrl);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false;
                request.Method = "HEAD";

                using (request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("External API unreachable");
                return false;
            }
        }
    }
}