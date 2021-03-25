using System.Collections.Generic;

namespace BouvetWebApp.Models
{
    public class QueryResult
    {
        public int Pages { get; set; }
        
        public List<Enheter> Companies { get; set; }
    }
}