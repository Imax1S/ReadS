using System;
using System.Collections.Generic;
using System.Text;

namespace ReadS.Classes_For_Stats
{
    class MonthStat
    {
        List<DayStat> days = new List<DayStat>();
        public int PagesTotal
        {
            get
            {
                int sum = 0;
                foreach (DayStat day in days)
                {
                    sum += day.Pages;
                }
                return sum;
            }
            set
            {

            }
        }

        public double Average
        {
            get
            {
                int sum = 0;
                foreach (DayStat day in days)
                {
                    sum += day.Pages;
                }
                return sum * 1.0 / days.Count;
            }
            set
            {

            }
        }

        public string Month { get; set; }

        public MonthStat(List<DayStat> days, string month)
        {
            this.days = days;
            Month = month;
        }
    }
}
