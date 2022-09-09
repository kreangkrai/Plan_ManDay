using System;
using System.Collections.Generic;
using System.Text;

namespace PlanManDay
{
    class InputJobModel
    {
        public string job { get; set; }
        public List<INPUT_MILESTONE> milestones { get; set; }
    }
    public class INPUT_MILESTONE
    {
        public string milestone { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_stop { get; set; }
    }
}
