using System;
using System.Collections.Generic;
using System.Text;

namespace ReadS
{
    class DayStat
    {
        //Количество страниц, прочитанных за день
        public int Pages { get; set; }

        //Хранит дату дня
        public DateTime Date { get; set; }

        //Цель, которая была поставлена на этот день
        public int Goal { get; set; }
        

        public DayStat(int pages, DateTime date, int goal)
        {
            Pages = pages;
            Date = date;
            Goal = goal;
        }
    }
}
