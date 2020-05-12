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
        static List<DayStat> dayStats = new List<DayStat>();
        static Microcharts.Forms.ChartView StatsOfReadingByDates = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.txt");
        Random random = new Random();
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


        public Months()
        {
            InitializeComponent();
            //for (int i = 0; i < 3; i++)
            //{
            //    int ran = random.Next(200, 3000);
            //    entries.Add(new Entry(ran)
            //    {
            //        Color = SKColor.Parse("#4285F4"),
            //        Label = namesOfMonths[i + 1],
            //        ValueLabel = ran.ToString(),
            //    });
            //}

            fillMonthGraph();
            //StatsOfReadingByDates.HeightRequest = 300;
            //StatsOfReadingByDates.WidthRequest = 100 * entries.Count;
            //StatsOfReadingByDates.HorizontalOptions = LayoutOptions.End;
            //StatsOfReadingByDates.VerticalOptions = LayoutOptions.End;
            //StatsOfReadingByDates.Chart = new Microcharts.LineChart { Entries = entries, LabelTextSize = 40, BackgroundColor = SKColor.Parse("#FFFFFF") };
            Label header = new Label
            {
                Text = dayStats[0].Date.Year.ToString(),
                FontSize = 40,
                HorizontalOptions = LayoutOptions.Center,
                Padding = 20,
            };
            this.Padding = new Thickness(10, Device.OnPlatform(20, 20, 0), 10, 5);
            scroll.Content = StatsOfReadingByDates;
            Content = new StackLayout()
            {
                Children = {header, scroll }
            };
        }

        static public void fillMonthGraph()
        {
            List<Entry> entries = new List<Entry>();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                List<Classes_For_Stats.MonthStat> monthStats = new List<Classes_For_Stats.MonthStat>();

                List<DayStat> days = new List<DayStat>();
                int tempMonth = dayStats[0].Date.Month;
                for (int i = 0; i < dayStats.Count - 1; i++)
                {
                    days.Add(dayStats[i]);
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

                foreach (Classes_For_Stats.MonthStat week in monthStats)
                {
                    entries.Add(new Entry(week.PagesTotal)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = week.Month,
                        ValueLabel = week.PagesTotal.ToString(),
                    });
                }

                StatsOfReadingByDates.HeightRequest = 300;
                StatsOfReadingByDates.WidthRequest = 100 * entries.Count;
                StatsOfReadingByDates.HorizontalOptions = LayoutOptions.End;
                StatsOfReadingByDates.VerticalOptions = LayoutOptions.End;
                StatsOfReadingByDates.Chart = new Microcharts.LineChart { Entries = entries, LabelTextSize = 40, BackgroundColor = SKColor.Parse("#FFFFFF") };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
    }
}