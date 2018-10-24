using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcquireSystem.Model
{
    public class PopulationModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IDNumber { get; set; }
        public DateTime Birthday { get; set; }
        public bool Sex { get; set; }
        public string Nation { get; set; }
        public string Address { get; set; }
        public string Memo { get; set; }

    }
}
