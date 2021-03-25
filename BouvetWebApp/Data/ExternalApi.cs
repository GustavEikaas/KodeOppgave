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
        private readonly ICompanyRepository _companyRepository;
        private const string PingUrl = @"https://data.brreg.no/enhetsregisteret/api/";
        private const string Query = @"https://data.brreg.no/enhetsregisteret/api/enheter?size=1000";

        public ExternalApi(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public void Initialize()
        {
            UpdateFromApi().Wait();
        }
        
        private async Task UpdateFromApi()
        {
            var updateList = PingApi() ? await FetchDataFromExternalApi() : null;

            if (updateList != null)
            {
                await _companyRepository.MergeUpdateList(updateList);
            }
        }

        private async Task<IEnumerable<Enheter>> FetchDataFromExternalApi()
        {
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

                using (var response = request.GetResponse())
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