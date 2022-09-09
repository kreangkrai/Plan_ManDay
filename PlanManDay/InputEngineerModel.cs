using System;
using System.Collections.Generic;
using System.Text;

namespace PlanManDay
{
    class InputEngineerModel
    {
        public string job { get; set; }
        public string name { get; set; }
        public List<INPUTENG_MILESTONE> milestones { get; set; }
    }
    public class INPUTENG_MILESTONE
    {
        public string milestone { get; set; }
        public int manday { get; set; }
    }
}
