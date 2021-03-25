using System.Threading;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;

namespace BouvetWebApp.Timer
{
    public class Refresh
    {
        private readonly ICompanyRepository _companyRepository;

        public Refresh(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        private System.Threading.Timer _timer;

        //private const int Interval = 1800000;
        private const int Interval = 120000 / 8;

        public void SetTimer()
        {
            _timer = new System.Threading.Timer(Tick, null, Interval, Timeout.Infinite);
        }

        private void Tick(object state)
        {
            try
            {
                var api = new ExternalApi();
                var updateList = api.FetchDataFromExternalApi().Result;
                if (updateList != null)
                {
                    _companyRepository.MergeUpdateList(updateList).Wait();
                }
            }   
            finally
            {
                _timer?.Change(Interval, Timeout.Infinite);
            }
        }
    }
}