using System;
using System.Collections.Generic;
using System.Text;

namespace PlanManDay
{
    public class OutputModel
    {
        public List<MONTHS> months { get; set; } = new List<MONTHS>();
    }
    public class MONTHS
    {
        public int workday { get; set; }
        public string month { get; set; }
        public List<OUTPUT_MILESTONE> milestones { get; set; }
    }
    public class OUTPUT_MILESTONE
    {
        
        public string milestone { get; set; }
        public List<Engineers> engs { get; set; }
    }
    public class Engineers
    {
        public string name { get; set; } = "";
        public string job { get; set; }
        public int manday { get; set; }
    }
}
