using System;
using System.Collections.Generic;
using System.Text;

namespace PlanManDay
{
    class Job_MilestoneModel
    {
        public string job { get; set; }
        public List<string> milestones { get; set; } = new List<string>();
    }
}
