using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_project.Entitty
{
    internal class Hall
    {
        public int Id { get; set; }
        public int Seats { get; set; }
        public bool IsVip { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}
