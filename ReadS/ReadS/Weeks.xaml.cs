using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;
using SkiaSharp;
using System.IO;
using Xamarin.Essentials;
using Newtonsoft.Json;



namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Weeks : ContentPage
    {
        static List<DayStat> dayStats = new List<DayStat>();
        static Microcharts.Forms.ChartView StatsOfReadingByDates = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.json");
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal
        };
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
        public Weeks()
        {
            InitializeComponent();

            //Если есть информация о статистике, то отрисовывает график 
            if (FillWeekGraph())
            {
                scroll.Content = StatsOfReadingByDates;

                Label header = new Label
                {
                    Text = namesOfMonths[dayStats[0].Date.Month],
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
            else
            {
                //Если нет, то сообщает об этом
                Label noStat = new Label()
                {
                    Text = "Пока что ещё нет статистики. Начните читать и она будет здесь)",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 16,
                    Padding = 10
                };
            }

        }

        /// <summary>
        /// Орисовывает график для недель
        /// </summary>
        /// <returns></returns>
        static public bool FillWeekGraph()
        {
            List<Entry> entries = new List<Entry>();
            try
            {

                entries.Add(new Entry(300)
                {
                    Color = SKColor.Parse("#4285F4"),
                    Label = "26 - 2",
                    ValueLabel = (300).ToString(),
                });

                entries.Add(new Entry(290)
                {
                    Color = SKColor.Parse("#4285F4"),
                    Label = "3 - 9",
                    ValueLabel = (290).ToString(),
                });

                //Чтения файла
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                List<Classes_For_Stats.WeekStat> weekStats = new List<Classes_For_Stats.WeekStat>();

                List<DayStat> days = new List<DayStat>();
                string period;

                //Создание объектов WeekStat
                for (int i = 0; i < dayStats.Count; i++)
                {
                    days.Add(dayStats[i]);
                    if (dayStats[i].Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        period = days[0].Date.Day + "-" + days[days.Count - 1].Date.Day;
                        weekStats.Add(new Classes_For_Stats.WeekStat(days, period));
                        days = new List<DayStat>();
                    }
                }

                if (days.Count != 0)
                {
                    period = days[0].Date.Day + "-" + days[days.Count - 1].Date.Day;
                    weekStats.Add(new Classes_For_Stats.WeekStat(days, period));
                }

                foreach (Classes_For_Stats.WeekStat week in weekStats)
                {
                    entries.Add(new Entry(week.PagesTotal)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = week.Period,
                        ValueLabel = week.PagesTotal.ToString(),
                    });
                }

                StatsOfReadingByDates.HeightRequest = 500;
                StatsOfReadingByDates.WidthRequest = 100 * entries.Count;
                StatsOfReadingByDates.HorizontalOptions = LayoutOptions.End;
                StatsOfReadingByDates.VerticalOptions = LayoutOptions.End;
                StatsOfReadingByDates.Chart = new Microcharts.LineChart { Entries = entries, LabelTextSize = 40, BackgroundColor = SKColor.Parse("#FAFAFA") };
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}