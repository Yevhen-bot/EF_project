using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Session
    {
        public int Id { get; set; }
        public Film Film { get; set; }
        public int FilmId { get; set; }
        public Hall Hall { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }
        public int Price { get; set; }
        public StatusSession Status { get; set; } 
        public int StatusId { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
