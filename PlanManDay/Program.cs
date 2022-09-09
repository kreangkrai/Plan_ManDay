﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace PlanManDay
{
    class Program
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

            //list holiday
            List<DateTime> holidays = new List<DateTime>();
            holidays = InputHolidays();

            ////////////// END INPUT /////////////////
            #endregion INPUT


            /////////// OUTPUT //////////////////

            List<Engineers> get_engs = new List<Engineers>();
            string get_milestone = "";
            string get_job = "";
            MONTHS month = new MONTHS();
            List<OUTPUT_MILESTONE> output_milestones = new List<OUTPUT_MILESTONE>();
            OutputModel output = new OutputModel();

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
                        DateTime date_start = input_jobs[m].milestones
                                .Where(w => w.milestone == listMilestone[j])
                                .Select(s => s.date_start).FirstOrDefault(); //get date start of milestone

                        if (_month.Key.Contains(date_start.ToString("yyyy-MM")))
                        {
                            int sum_workday = output_milestones.Select(s => s.engs.Select(x => x.manday).Sum()).Sum();  //  current sum all engineer of milestone                           
                            for (int k = 0; k < engineers.Count; k++) //each engineer
                            {
                                get_job = engineers[k].job;
                                int get_manday = engineers[k].milestones.Where(w => w.milestone == listMilestone[j])
                                    .Select(s => s.manday)
                                    .FirstOrDefault();  //get man day of milestone
                                get_milestone = listMilestone[j];
                                if (get_manday > 0 && input_jobs[m].job == engineers[k].job)
                                {
                                    get_engs.Add(new Engineers()
                                    {
                                        name = engineers[k].name,
                                        job = get_job,
                                        manday = get_manday
                                    });
                                }
                            }
                            output_milestones.Add(new OUTPUT_MILESTONE()
                            {
                                milestone = get_milestone,
                                engs = get_engs
                            });
                        }
                    }
                    month = new MONTHS()
                    {
                        workday = _month.Value.workday,
                        month = _month.Value.month.ToString("yyyy-MM"),
                        milestones = output_milestones,
                        all_manday = output_milestones.Select(s => s.engs.Sum(x => x.manday)).Sum()
                    };
                }
                output.months.Add(month);
            }

            #endregion Calculate

            //Print Screen Output
            for (int i = 0; i < output.months.Count; i++)
            {
                Console.WriteLine();
                Console.WriteLine("######################################################################");
                Console.WriteLine();
                Console.WriteLine($"Month : [{output.months[i].month}] , Work Day Of Month : [{output.months[i].workday}] , ALL Man Day : [{output.months[i].all_manday}]");
                Console.WriteLine();
                for (int j = 0; j < output.months[i].milestones.Count; j++)
                {
                    Console.WriteLine($"MILESTONE ===== [{output.months[i].milestones[j].milestone}] =====");
                    for (int m = 0; m < output.months[i].milestones[j].engs.Count; m++)
                    {
                        Console.WriteLine($"Name : [{output.months[i].milestones[j].engs[m].name}] , Man Day : [{output.months[i].milestones[j].engs[m].manday}] ,Job : [{output.months[i].milestones[j].engs[m].job}]");
                    }
                }
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
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 8, 9),
                            date_stop = new DateTime(2022, 8, 15),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 8, 16),
                            date_stop = new DateTime(2022, 9, 15),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2022, 9, 16),
                            date_stop = new DateTime(2022, 9, 20),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "TEST",
                            date_start = new DateTime(2022, 9, 17),
                            date_stop = new DateTime(2022, 9, 19),
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2022, 9, 21),
                            date_stop = new DateTime(2022, 9, 25),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2022, 9, 26),
                            date_stop = new DateTime(2022, 9, 30),
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
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 9, 6),
                            date_stop = new DateTime(2022, 9, 15),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 9, 16),
                            date_stop = new DateTime(2022, 10, 15),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2022, 10, 16),
                            date_stop = new DateTime(2022, 10, 20),
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2022, 10, 21),
                            date_stop = new DateTime(2022, 10, 31),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2022, 11, 1),
                            date_stop = new DateTime(2022, 11, 5),
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
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "DOC",
                            date_start = new DateTime(2022, 11, 21),
                            date_stop = new DateTime(2022, 11, 30),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "GEN",
                            date_start = new DateTime(2022, 12, 1),
                            date_stop = new DateTime(2023, 1, 15),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "FAT",
                            date_start = new DateTime(2023, 1, 16),
                            date_stop = new DateTime(2023, 1, 22),
                        },
                        new INPUT_MILESTONE ()
                        {
                            milestone = "SITE",
                            date_start = new DateTime(2023, 1, 23),
                            date_stop = new DateTime(2023, 1, 25),
                        },
                        new INPUT_MILESTONE()
                        {
                            milestone = "HANDOVER",
                            date_start = new DateTime(2023, 1, 26),
                            date_stop = new DateTime(2023, 1, 31),
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
                        manday = 5
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "FAT",
                        manday = 2
                    },
                    new INPUTENG_MILESTONE()
                    {
                        milestone = "TEST",
                        manday = 8
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
                        manday = 1
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
                        manday = 1
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
                        manday = 2,
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
