using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanManDay
{
    class InputJobNewModel
    {
        public string job { get; set; }
        public string milestone { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_stop { get; set; }
        public List<DateTime> duration_months { get; set; }
    }
}
