using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class StatusSession
    {
        public int Id { get; set; }
        public string Status { get; set; } 
        public ICollection<Session> Sessions { get; set; }
    }
}
