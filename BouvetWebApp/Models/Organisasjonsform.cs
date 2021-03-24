using System.Text.Json.Serialization;

namespace BouvetWebApp.Models
{
    public class Organisasjonsform
    {
        public int Id { get; set; }
        public string Kode { get; set; }
        
        public string Beskrivelse { get; set; }
    }
}