using System;
using System.Collections.Generic;
using System.Text;

namespace ReadS
{
    class DayStat
    {
        public int Pages { get; set; }
        public DateTime Date { get; set; }

        public int Goal { get; set; }
        

        public DayStat(int pages, DateTime date, int goal)
        {
            Pages = pages;
            Date = date;
            Goal = goal;
        }
    }
}
