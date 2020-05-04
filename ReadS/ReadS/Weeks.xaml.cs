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
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.txt");
        static List<Entry> entries = new List<Entry>();
        Random random = new Random();
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal
        };
        public Weeks()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                entries.Add(new Entry(random.Next(200, 700))
                {
                    Color = SKColor.Parse("#4285F4"),
                });
            }

            fillWeekGraph();
            scroll.Content = StatsOfReadingByDates;
            Content = new StackLayout()
            {
                Children = { scroll }
            };
        }

        public void fillWeekGraph()
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                List<int> weeks = new List<int>();
                int sum = 0;
                for (int i = 0; i < dayStats.Count; i++)
                {
                    sum += dayStats[i].Pages;
                    if (i + 1 % 7 == 0)
                    {
                        weeks.Add(sum);
                        sum = 0;
                    }
                }
                weeks.Add(sum);

                foreach (int pages in weeks)
                {
                    entries.Add(new Entry(pages)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        //Label = day.Date.ToString(),
                    });
                }

                StatsOfReadingByDates.HeightRequest = 200;
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

                //Content = new StackLayout()
                //{
                //    Children = { noStat }
                //};

            }
        }
    }
}