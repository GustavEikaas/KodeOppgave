using System.Threading;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BouvetWebApp
{
    public class Refresh
    {
        private readonly ICompanyRepository _companyRepository;

        public Refresh(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        private Timer _timer;

        //private const int Interval = 1800000;
        private const int Interval = 120000 / 4;

        public void SetTimer()
        {
            _timer = new Timer(Tick, null, Interval, Timeout.Infinite);
        }

        private void Tick(object state)
        {
            try
            {
                var apiController = new ExternalApi(_companyRepository);
                apiController.Initialize();
            }   
            finally
            {
                _timer?.Change(Interval, Timeout.Infinite);
            }
        }
    }
}