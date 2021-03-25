using System.Threading;
using BouvetWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BouvetWebApp
{
    public class Refresh
    {
        private readonly IDbContextFactory<OrgContext> _contextFactory;

        public Refresh(IDbContextFactory<OrgContext> contextFactory)
        {
            _contextFactory = contextFactory;
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
                var dbController = new ExternalApi(_contextFactory, default);

                dbController.Initialize();
            }   
            finally
            {
                _timer?.Change(Interval, Timeout.Infinite);
            }
        }
    }
}