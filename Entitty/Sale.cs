using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Sale
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int TicketsCount { get; set; } 
        public int TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
