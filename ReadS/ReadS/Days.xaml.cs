using Newtonsoft.Json;
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

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Days : ContentPage
    {
        static List<DayStat> dayStats = new List<DayStat>();
        static Microcharts.Forms.ChartView StatsOfReadingByDates = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.txt");
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal,
            //FlowDirection = FlowDirection.RightToLeft,
        };
        Dictionary<int, string> namesOfMonths = new Dictionary<int, string> {
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
        public Days()
        {
            InitializeComponent();

            fillDayGraph();
            scroll.Content = StatsOfReadingByDates;
            scroll.VerticalOptions = LayoutOptions.Center;
            scroll.HorizontalOptions = LayoutOptions.Center;
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

        static public void fillDayGraph()
        {
            Random random = new Random();
            List<Entry> entries = new List<Entry>();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                //for (int i = 0; i < 3; i++)
                //{
                //    int ran = random.Next(0, 100);
                //    entries.Add(new Entry(ran)
                //    {
                //        Color = SKColor.Parse("#4285F4"),
                //        Label = new DateTime(2020, 5, i + 1).ToString(),
                //        ValueLabel = ran.ToString(),
                //    });
                //}

                foreach (DayStat day in dayStats)
                {
                    entries.Add(new Entry(day.Pages)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = day.Date.Day.ToString(),
                        ValueLabel = day.Pages.ToString(),
                    });
                }

                StatsOfReadingByDates.HeightRequest = 300;
                StatsOfReadingByDates.WidthRequest = 100 * entries.Count;

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