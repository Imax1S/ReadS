using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;
using SkiaSharp;
using Newtonsoft.Json;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Months : ContentPage
    {
        //Создание переменных
        static List<DayStat> dayStats = new List<DayStat>();
        static Microcharts.Forms.ChartView StatsOfReadingByDates = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.json");

        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal
        };

        //Словарь с месяцами
        static Dictionary<int, string> namesOfMonths = new Dictionary<int, string> {
            {1, "Январь" },
            {2, "Февраль" } ,
            {3, "Март" },
            {4, "Апрель" },
            {5, "Май" },
            {6, "Июнь" },
            {7, "Июль" },
            {8, "Август" },
            {9, "Сентябрь" },
            {10, "Октябрь" },
            {11, "Ноябрь" },
            {12, "Декабрь" },
        };


        public Months()
        {
            InitializeComponent();

            //Если нет данных о статистике, то сообщает об этом 
            if (!FillMonthGraph())
            {
                Label noStat = new Label()
                {
                    Text = "Пока что ещё нет статистики. Начните читать и она будет здесь)",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 16,
                    Padding = 10
                };
                Content = new StackLayout()
                {
                    Children = {noStat }
                };
            }
            else
            {
                //Если есть, то отрисовывает
                scroll.Content = StatsOfReadingByDates;

                Label header = new Label
                {
                    Text = "2020",
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = 20,
                };
                this.Padding = new Thickness(10, Device.OnPlatform(20, 20, 0), 10, 5);
                Content = new StackLayout()
                {
                    Children = { header, scroll }
                };
            }
        }

        /// <summary>
        /// Отрисовывает график по месяцам
        /// </summary>
        /// <returns></returns>
        static public bool FillMonthGraph()
        {
            //Лист с точками на графике
            List<Entry> entries = new List<Entry>();
            try
            {
                //Чтение файла
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                List<Classes_For_Stats.MonthStat> monthStats = new List<Classes_For_Stats.MonthStat>();

                List<DayStat> days = new List<DayStat>();
                int tempMonth = dayStats[0].Date.Month;

                //По дням создаются объекты MonthStat
                for (int i = 0; i < dayStats.Count - 1; i++)
                {
                    days.Add(dayStats[i]);

                    //Если день уже принадлежит другому месяцу, то создает новый MonthStat объект
                    if (dayStats[i + 1].Date.Month != tempMonth)
                    {
                        monthStats.Add(new Classes_For_Stats.MonthStat(days, namesOfMonths[dayStats[i].Date.Month]));
                        days = null;
                    }
                }

                if (days != null)
                {
                    monthStats.Add(new Classes_For_Stats.MonthStat(days, namesOfMonths[days[0].Date.Month]));
                }

                //Создание точек
                foreach (Classes_For_Stats.MonthStat week in monthStats)
                {
                    entries.Add(new Entry(week.PagesTotal)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = week.Month,
                        ValueLabel = week.PagesTotal.ToString(),
                    });
                }

                //Настройка графика
                StatsOfReadingByDates.HeightRequest = 500;
                StatsOfReadingByDates.WidthRequest = 100 * entries.Count;
                StatsOfReadingByDates.HorizontalOptions = LayoutOptions.End;
                StatsOfReadingByDates.VerticalOptions = LayoutOptions.End;
                StatsOfReadingByDates.Chart = new Microcharts.LineChart { Entries = entries, LabelTextSize = 40, BackgroundColor = SKColor.Parse("#FAFAFA") };
                //После успешного чтения возращает true
                return true;
            }
            catch (Exception ex)
            {
                //Если ловится исключение, то возвращает false
                return false;
            }
        }
    }
}