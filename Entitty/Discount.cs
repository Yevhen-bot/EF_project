using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Discount
    {
        public int Id { get; set; }
        public Film Film { get; set; }
        public int FilmId { get; set; }
        public int DiscountPercent { get; set; }

    }
}
