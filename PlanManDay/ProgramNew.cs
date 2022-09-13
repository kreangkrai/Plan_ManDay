using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace PlanManDay
{
    class ProgramNew
    {
        static void Main(string[] args)
        {
            #region INPUT
            /////////////// INPUT ////////////////

            // input jobs
            List<InputJobModel> input_jobs = new List<InputJobModel>();
            input_jobs = InputJobs();

            // input assign engineer to each job
            List<InputEngineerModel> engineers = new List<InputEngineerModel>();
            engineers = InputEngineer();

            //get list milestone from server
            List<string> listMilestone = new List<string>();
            listMilestone = InputMilestones();

            //list holiday from server
            List<DateTime> holidays = new List<DateTime>();
            holidays = InputHolidays();

            ////////////// END INPUT /////////////////
            #endregion INPUT


            /////////// OUTPUT //////////////////

            List<OutputNewModel> outputs_new = new List<OutputNewModel>();

            List<Engineers> get_engs = new List<Engineers>();
            string get_milestone = "";
            string get_job = "";

            List<OUTPUT_MILESTONE> output_milestones = new List<OUTPUT_MILESTONE>();

            Dictionary<string, WorkDayOfMonthModel> has_months = new Dictionary<string, WorkDayOfMonthModel>();
            has_months = DataWorkDayOfMonth(input_jobs, holidays);

            #region Calculate

            ///////   Calculate //////////////
            foreach (var _month in has_months)//each month
            {
                output_milestones = new List<OUTPUT_MILESTONE>();
                for (int j = 0; j < listMilestone.Count; j++)  // each milestone; have engineer? , manday ? 
                {
                    get_engs = new List<Engineers>();
                    get_milestone = "";
                    get_job = "";
                    for (int m = 0; m < input_jobs.Count; m++)
                    {
                        List<DateTime> month_starts = input_jobs[m].milestones
                            .Where(w => w.milestone == listMilestone[j])
                            .Select(s => s.duration_months
                            .ToList())
                            .FirstOrDefault();
                        if (month_starts != null)
                        {
                            foreach (var month_ in month_starts)  //month in each milestone
                            {
                                if (_month.Key.Contains(month_.ToString("yyyy-MM")))
                                {
                                    for (int k = 0; k < engineers.Count; k++) //each engineer
                                    {
                                        get_job = engineers[k].job;
                                        int get_manday = engineers[k].milestones.Where(w => w.milestone == listMilestone[j])
                                            .Select(s => s.manday)
                                            .FirstOrDefault();  //get man day of milestone
                                        get_milestone = listMilestone[j];

                                        if (get_manday > 0 && input_jobs[m].job == engineers[k].job) // check match job and more manday 
                                        {
                                            int current_manday = output_milestones
                                                .Select(s => s.engs
                                                    .Where(w => w.name == engineers[k].name)
                                                    .Sum(s1 => s1.manday))
                                                .Sum();

                                            int remain_manday = _month.Value.workday - current_manday;

                                            Engineers eng = new Engineers();
                                            eng.name = engineers[k].name;
                                            eng.job = get_job;

                                            int last_manday = 0;
                                            if (month_starts.Count > 1) // multi month in each milestone
                                            {
                                                last_manday = outputs_new
                                                    .Where(w => w.milestone == get_milestone && w.job == engineers[k].job && w.name == engineers[k].name)
                                                    .Select(s => s.manday)
                                                    .Sum();

                                            }
                                            if (remain_manday >= get_manday)
                                            {
                                                eng.manday = get_manday - last_manday;
                                            }
                                            else
                                            {
                                                eng.manday = _month.Value.workday - current_manday - last_manday;
                                            }
                                            get_engs.Add(eng);

                                            // output
                                            outputs_new.Add(new OutputNewModel()
                                            {
                                                month = _month.Value.month.ToString("yyyy-MM"),
                                                workday = _month.Value.workday,
                                                job = eng.job,
                                                name = eng.name,
                                                manday = eng.manday,
                                                milestone = get_milestone

                                            });
                                        }
                                    }

                                    bool check_milestone = output_milestones.Any(w => w.milestone == get_milestone);
                                    if (!check_milestone)
                                    {
                                        output_milestones.Add(new OUTPUT_MILESTONE()
                                        {
                                            milestone = get_milestone,
                                            engs = get_engs
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion Calculate

            //Print Screen Output
            for (int i = 0; i < outputs_new.Count; i++)
            {
                Console.WriteLine($"{(i + 1).ToString().PadLeft(3,' ')} Month {outputs_new[i].month} , Work Day {outputs_new[i].workday} , Milestone {(outputs_new[i].milestone).PadLeft(10,' ')} , Job {outputs_new[i].job} , Name {(outputs_new[i].name).PadLeft(10, ' ')} , Man Day {(outputs_new[i].manday).ToString().PadLeft(2, ' ')}");
            }
            Console.ReadLine();
        }
        public static List<InputJobModel> InputJobs()
        {
            List<InputJobModel> input_jobs = new List<InputJobModel>()
            {
                new InputJobModel()
                {
                    job = "J22-0001",
                    milestones = new List<INPUT_MILESTONE>()
                    {
                        new INPUT_MILESTONE()
                        {
                            milestone = "KOM",
                            date_start = new DateTime(2022, 8, 6),
                            date_stop = new DateTime(2022, 8, 8),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,8,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 8, 9),
                            date_stop = new DateTime(2022, 8, 15),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,8,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 8, 16),
                            date_stop = new DateTime(2022, 9, 15),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,8,1),
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2022, 9, 16),
                            date_stop = new DateTime(2022, 9, 20),duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "TEST",
                            date_start = new DateTime(2022, 9, 17),
                            date_stop = new DateTime(2022, 9, 19),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2022, 9, 21),
                            date_stop = new DateTime(2022, 9, 25),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2022, 9, 26),
                            date_stop = new DateTime(2022, 9, 30),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        }
                    },
                },
                new InputJobModel()
                {
                    job = "J22-9999",
                    milestones = new List<INPUT_MILESTONE>()
                    {
                        new INPUT_MILESTONE()
                        {
                            milestone = "KOM",
                            date_start = new DateTime(2022, 9, 4),
                            date_stop = new DateTime(2022, 9, 5),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 9, 6),
                            date_stop = new DateTime(2022, 9, 15),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 9, 16),
                            date_stop = new DateTime(2022, 10, 15),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,9,1),
                                new DateTime(2022,10,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2022, 10, 16),
                            date_stop = new DateTime(2022, 10, 20),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,10,1)
                            }
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2022, 10, 21),
                            date_stop = new DateTime(2022, 10, 31),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,10,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2022, 11, 1),
                            date_stop = new DateTime(2022, 11, 5),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,11,1)
                            }
                        }
                    },
                },
                new InputJobModel()
                {
                    job = "J22-8888",
                    milestones = new List<INPUT_MILESTONE>()
                    {
                        new INPUT_MILESTONE()
                        {
                            milestone = "KOM",
                            date_start = new DateTime(2022, 11, 15),
                            date_stop = new DateTime(2022, 11, 20),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,11,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 11, 21),
                            date_stop = new DateTime(2022, 11, 30),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,11,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 12, 1),
                            date_stop = new DateTime(2023, 1, 15),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2022,12,1),
                                new DateTime(2023,1,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2023, 1, 16),
                            date_stop = new DateTime(2023, 1, 22),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2023,1,1)
                            }
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2023, 1, 23),
                            date_stop = new DateTime(2023, 1, 25),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2023,1,1)
                            }
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2023, 1, 26),
                            date_stop = new DateTime(2023, 1, 31),
                            duration_months = new List<DateTime>()
                            {
                                new DateTime(2023,1,1)
                            }
                        }
                    },
                },
            };
            return input_jobs;
        }
        public static List<InputEngineerModel> InputEngineer()
        {
            List<InputEngineerModel> engs = new List<InputEngineerModel>();
            engs.Add(new InputEngineerModel()
            {
                name = "A",
                job = "J22-0001",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 5
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 15
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "TEST",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 2
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "A",
                job = "J22-9999",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 1
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 5
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 3
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 2
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "B",
                job = "J22-0001",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 4
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "TEST",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 0
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "C",
                job = "J22-0001",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 3
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "TEST",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 1
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 1
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "Mee",
                job = "J22-8888",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 0
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 15
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 3
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 2
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "Kriangkrai",
                job = "J22-8888",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 2,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 5
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 3
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 2
                    },
                }
            });
            engs.Add(new InputEngineerModel()
            {
                name = "Kriangkrai",
                job = "J22-9999",
                milestones = new List<INPUTENG_MILESTONE>()
                {
                    new INPUTENG_MILESTONE ()
                    {
                        milestone = "KOM",
                        manday = 1,
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "DOC",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "GEN",
                        manday = 10
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "SITE",
                        manday = 3
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "HANDOVER",
                        manday = 2
                    },
                }
            });
            return engs;
        }
        public static List<string> InputMilestones()
        {
            List<string> listMilestone = new List<string>()
            {
                "KOM","DOC","GEN","FAT","TEST","SITE","HANDOVER"
            };
            return listMilestone;
        }
        public static List<DateTime> InputHolidays()
        {
            List<DateTime> holidays = new List<DateTime>()
            {
                new DateTime(2022, 8, 15),
                new DateTime(2022, 9, 15),
                new DateTime(2022, 10, 15),
                new DateTime(2022, 11, 15),
                new DateTime(2023, 1, 1),
                new DateTime(2023, 1, 2),
                new DateTime(2023, 1, 3),
            };
            return holidays;
        }
        public static List<InputEngineerModel> DataEngineer(List<InputJobModel> input_jobs, List<InputEngineerModel> engs)
        {
            List<string> jobs = new List<string>();
            for (int i = 0; i < input_jobs.Count; i++)
            {
                jobs.Add(input_jobs[i].job);
            }

            List<InputEngineerModel> engineers = new List<InputEngineerModel>();
            for (int i = 0; i < jobs.Count; i++)
            {
                for (int j = 0; j < engs.Count; j++)
                {
                    if (jobs[i] == engs[j].job)
                    {
                        engineers.Add(new InputEngineerModel()
                        {
                            job = engs.Where(w => w.job == jobs[i] && w.name == engs[j].name)
                                .Select(s => s.job)
                                .FirstOrDefault(),
                            milestones = engs.Where(w => w.job == jobs[i] && w.name == engs[j].name)
                                .Select(s => s.milestones.ToList())
                                .FirstOrDefault(),
                            name = engs.Where(w => w.job == jobs[i] && w.name == engs[j].name)
                                .Select(s => s.name)
                                .FirstOrDefault()
                        });
                    }
                }
            }
            return engineers;
        }
        public static Dictionary<string, WorkDayOfMonthModel> DataWorkDayOfMonth(List<InputJobModel> input_jobs, List<DateTime> holidays)
        {
            List<JobWorkMonthModel> job_workmonths = new List<JobWorkMonthModel>();
            for (int j = 0; j < input_jobs.Count; j++)
            {
                List<workMonth> workMonths = new List<workMonth>();
                DateTime start = input_jobs[j].milestones.Select(s => s.date_start).OrderBy(o => o.Date).FirstOrDefault();
                DateTime stop = input_jobs[j].milestones.Select(s => s.date_stop).OrderByDescending(o => o.Date).FirstOrDefault();
                int diffMonth = ((stop.Year - start.Year) * 12) + stop.Month - start.Month;
                for (int i = 0; i <= diffMonth; i++)
                {
                    if (i == 0)
                    {
                        var lastDayOfMonth = DateTime.DaysInMonth(start.Year, start.Month);
                        int workday = Weekdays(start, new DateTime(start.Year, start.Month, lastDayOfMonth), holidays);
                        workMonths.Add(new workMonth()
                        {
                            month = new DateTime(start.Year, start.Month, 1),
                            workday = workday,
                            job = input_jobs[j].job,
                        });
                    }
                    else
                    {
                        var lastDayOfMonth = DateTime.DaysInMonth(start.AddMonths(i).Year, start.AddMonths(i).Month);
                        int workday = Weekdays(
                            new DateTime(start.AddMonths(i).Year, start.AddMonths(i).Month, 1),
                            new DateTime(start.AddMonths(i).Year, start.AddMonths(i).Month, lastDayOfMonth),
                            holidays);
                        workMonths.Add(new workMonth()
                        {
                            month = new DateTime(start.AddMonths(i).Year, start.AddMonths(i).Month, 1),
                            workday = workday,
                            job = input_jobs[j].job,
                        });
                    }
                }
                JobWorkMonthModel jobWorkMonth = new JobWorkMonthModel()
                {
                    workmonth = workMonths
                };
                job_workmonths.Add(jobWorkMonth);
            }

            // order month , workday
            var _job_workmonths = job_workmonths.SelectMany(s => s.workmonth).OrderBy(o => o.month).ThenByDescending(t => t.workday).ToList();

            //distinct month
            Dictionary<string, WorkDayOfMonthModel> has_months = new Dictionary<string, WorkDayOfMonthModel>();
            for (int i = 0; i < _job_workmonths.Count; i++)
            {
                if (!has_months.ContainsKey(_job_workmonths[i].month.ToString("yyyy-MM")))
                {
                    has_months.Add(
                        key: _job_workmonths[i].month.ToString("yyyy-MM"),
                        value: new WorkDayOfMonthModel()
                        {
                            month = _job_workmonths[i].month,
                            workday = _job_workmonths[i].workday
                        });
                }
            }
            return has_months;
        }
        public static int Weekdays(DateTime dtmStart, DateTime dtmEnd, List<DateTime> holidays)
        {
            int count_holiday = 0;
            for (DateTime d = dtmStart; d != dtmEnd; d = d.AddDays(1))
            {
                for (int j = 0; j < holidays.Count; j++)
                {
                    if (d == holidays[j])
                    {
                        count_holiday++;
                        break;
                    }
                }
            }
            int dowStart = ((int)dtmStart.DayOfWeek == 0 ? 7 : (int)dtmStart.DayOfWeek);
            int dowEnd = ((int)dtmEnd.DayOfWeek == 0 ? 7 : (int)dtmEnd.DayOfWeek);
            TimeSpan tSpan = dtmEnd - dtmStart;
            if (dowStart <= dowEnd)
            {
                return (((tSpan.Days / 7) * 5) + Math.Max((Math.Min((dowEnd + 1), 6) - dowStart), 0)) - count_holiday;
            }
            return (((tSpan.Days / 7) * 5) + Math.Min((dowEnd + 6) - Math.Min(dowStart, 6), 5)) - count_holiday;
        }
    }
}
