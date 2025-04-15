using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public int Duration { get; set; } //seconds
        public DateTime ReleaseDate { get; set; }
        public int? Restriction { get; set; }
        public string? Description { get; set; }
        public Discount? Discount { get; set; }
        public int? DiscountId { get; set; }
        public RegularDiscount? RegularDiscount { get; set; }
        public int? RegularDiscountId { get; set; }
        public ICollection<Session> Sessions { get; set; } 

    }
}
