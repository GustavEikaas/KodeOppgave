using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BouvetWebApp.Models
{

    public record Root(Embedded _embedded);
    public record Embedded(List<Enheter> Enheter);
    public class Enheter
    {
        [Key]
        public int Organisasjonsnummer { get; set; }
        [Required]
        public string Navn { get; set; }
        [Required]
        public Organisasjonsform Organisasjonsform { get; set; }
        
        public int Vurdering { get; set; }
    }
}