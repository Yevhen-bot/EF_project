using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Ticket
    {
        public int Id { get; set; }
        public Session Session { get; set; }
        public int SessionId { get; set; }
        public int SeatNumber { get; set; }
        public int Price { get; set; }
        public StatusTicket Status { get; set; }
        public int StatusId { get; set; } 
    }
}
