using System;
using System.Collections.Generic;
using System.Text;

namespace PlanManDay
{
    class JobWorkMonthModel
    {      
        public List<workMonth> workmonth { get; set; }
    }
    public class workMonth
    {
        public DateTime month { get; set; }
        public string job { get; set; }
        public int workday { get; set; }
    }
}
