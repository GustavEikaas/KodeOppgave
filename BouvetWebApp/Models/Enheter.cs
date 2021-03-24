using System;
using System.ComponentModel.DataAnnotations;

namespace BouvetWebApp.Models
{
    public class Enheter
    {
        [Key]
        public int Organisasjonsnummer { get; set; }
        public string Navn { get; set; }
        public Organisasjonsform Organisasjonsform { get; set; }
        
        public int Vurdering { get; set; }
    }
}