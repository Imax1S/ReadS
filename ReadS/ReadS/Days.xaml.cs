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
        static List<Entry> entries = new List<Entry>();
        Random random = new Random();
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal
        };
        public Days()
        {
            InitializeComponent();
            for (int i = 0; i < 7; i++)
            {
                entries.Add(new Entry(random.Next(0, 100))
                {
                    Color = SKColor.Parse("#4285F4"),
                    Label = new DateTime(2020, 5, i + 1).ToString(),
                });
            }
            
            fillDayGraph();
            scroll.Content = StatsOfReadingByDates;
            Content = new StackLayout()
            {
                Children = { scroll }
            };
        }

        static public void fillDayGraph()
        {

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                }

                foreach (DayStat day in dayStats)
                {
                    entries.Add(new Entry(day.Pages)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = day.Date.ToString(),
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