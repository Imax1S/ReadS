using System;
using System.Collections.Generic;
using System.Text;

namespace ReadS.Classes_For_Stats
{
    class WeekStat
    {
        List<DayStat> dayStats = new List<DayStat>();

        public int PagesTotal
        {
            get
            {
                int sum = 0;
                foreach (DayStat day in dayStats)
                {
                    sum += day.Pages;
                }

                return sum;
            }
            set
            {

            }
        }
        public string Period { get; set; }

        public double Average
        {
            get
            {
                int sum = 0;
                foreach (DayStat item in dayStats)
                {
                    sum += item.Pages;
                }
                return sum * 1.0 / dayStats.Count;
            }
            set
            {

            }
        }

        public WeekStat(List<DayStat> dayStats, string period)
        {
            this.dayStats = dayStats;
            Period = period;
        }
    }
}
