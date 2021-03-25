using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BouvetWebApp.Models
{
    public class Organisasjonsform
    {
        public int Id { get; set; }
        
        [Required]
        public string Kode { get; set; }
        
        [Required]
        public string Beskrivelse { get; set; }
        
        public List<Enheter> Enheter { get; set; }
    }
}