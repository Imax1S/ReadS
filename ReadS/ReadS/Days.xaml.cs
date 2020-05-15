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
        //Создание переменных
        static List<DayStat> dayStats = new List<DayStat>();
        static Microcharts.Forms.ChartView StatsOfReadingByDates = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.json");
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Horizontal,
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

            //Если есть данные для графика, то рисует его
            if (FillDayGraph())
            {
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
            else
            {
                //Если нет, то пишет, что статистики пока что нет
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
                    Children = { noStat }
                };
            }
;
        }

        /// <summary>
        /// Заполняет граф с днями
        /// </summary>
        static public bool FillDayGraph()
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

                //Для каждого дня создается точка
                foreach (DayStat day in dayStats)
                {
                    entries.Add(new Entry(day.Pages)
                    {
                        Color = SKColor.Parse("#4285F4"),
                        Label = day.Date.Day.ToString(),
                        ValueLabel = day.Pages.ToString(),
                    });
                }

                //Определяется размер графика
                StatsOfReadingByDates.HeightRequest = 500;
                StatsOfReadingByDates.WidthRequest = 100 * entries.Count;

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