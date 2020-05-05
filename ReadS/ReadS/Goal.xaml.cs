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
    public partial class Goal : ContentPage
    {
        Label pages = new Label();
        public static int goalPages = 0;
        public static int pagesRead = 0;
        static Microcharts.Forms.ChartView StatsOfReading = new Microcharts.Forms.ChartView();
        //static string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        static string filenameGoal = Path.Combine(FileSystem.AppDataDirectory, "goals.txt");
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.txt");
        List<DayStat> dayStats = new List<DayStat>();
        
        public Goal()
        {
            InitializeComponent();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    if (json != "")
                    {
                        dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                        if (DateTime.Today == dayStats[dayStats.Count - 1].Date)
                        {
                            goalPages = dayStats[dayStats.Count - 1].Goal;
                            pagesRead = dayStats[dayStats.Count - 1].Pages;
                        }
                        else
                        {
                            goalPages = 0;
                            pagesRead = 0;
                        }
                    }
                    else
                    {
                        goalPages = 0;
                        pagesRead = 0;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong");
            }

            Slider slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
            };
            slider.VerticalOptions = LayoutOptions.FillAndExpand;
            slider.VerticalOptions = LayoutOptions.CenterAndExpand;
            slider.ThumbColor = Color.Gray;
            slider.MinimumTrackColor = Color.CadetBlue;
            slider.Margin = 20;
            //slider = 100;
            pages.VerticalOptions = LayoutOptions.CenterAndExpand;
            pages.HorizontalOptions = LayoutOptions.CenterAndExpand;
            pages.FontSize = 20;
            if (goalPages == 0)
            {
                pages.Text = "Передвиньте ползунок, чтобы поставить цель";
            }
            else
            {
                StatsOfReading.Opacity = 100;
                slider.AnchorX = goalPages;
                if (goalPages <= pagesRead)
                {
                    pages.Text = String.Format("Цель {0} страниц выполнена!", goalPages);
                    RefreshGoalGraph();
                }
                else
                {
                    pages.Text = String.Format("Цель {0} страниц", goalPages);
                    RefreshGoalGraph();
                }
            }


            slider.ValueChanged += (sender, args) =>
            {
                if ((int)slider.Value == 0)
                {
                    pages.Text = "Передвиньте ползунок, чтобы поставить цель";
                    goalPages = (int)slider.Value;
                    StatsOfReading.Opacity = 0;
                }
                else
                {
                    StatsOfReading.Opacity = 100;
                    goalPages = (int)slider.Value;
                    if (goalPages <= pagesRead)
                    {
                        pages.Text = String.Format("Цель {0} страниц выполнена!", (int)args.NewValue);
                        RefreshGoalGraph();
                    }
                    else
                    {
                        pages.Text = String.Format("Цель {0} страниц", (int)args.NewValue);
                        RefreshGoalGraph();
                    }
                    SaveGoal();
                }

            };

            Content = new StackLayout
            {
                Children =
            {
                    StatsOfReading,
                    pages,
                    slider
            }
            };
        }

        public static void RefreshGoalGraph()
        {
            List<Entry> entries = new List<Entry>
        {
            new Entry(goalPages - pagesRead)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Осталось",
                ValueLabel = (goalPages - pagesRead).ToString()
            },

            new Entry(pagesRead)
            { 
                Color = SKColor.Parse("#0F9D58"),
                Label = "Прочитано",
                ValueLabel = (pagesRead).ToString(),
            }
        };
            StatsOfReading.HorizontalOptions = LayoutOptions.FillAndExpand;
            StatsOfReading.VerticalOptions = LayoutOptions.FillAndExpand;
            StatsOfReading.Chart = new Microcharts.DonutChart { Entries = entries, LabelTextSize = 40 };
        }

        public static void SaveGoal()
        {
            //using (var streamWriter = new StreamWriter(filenameGoal, false))
            //{
            //    streamWriter.WriteLine(goalPages + ":" + pagesRead);
            //}

            List<DayStat> dayStats = new List<DayStat>();
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();
                    if (json != "")
                    {
                        dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Не прочиталось");
            }

            if (dayStats.Count == 0)
            {
                dayStats.Add(new DayStat(pagesRead, DateTime.Today, goalPages));
            }
            else if (dayStats[dayStats.Count - 1].Date == DateTime.Today)
            {
                dayStats[dayStats.Count - 1].Pages = pagesRead;
                dayStats[dayStats.Count - 1].Goal = goalPages;
            }
            else
            {
                dayStats.Add(new DayStat(pagesRead, DateTime.Today, goalPages));
            }

            using (StreamWriter writer = new StreamWriter(filename))
            {
                string json = JsonConvert.SerializeObject(dayStats);
                writer.Write(json);
            }

            Days.fillDayGraph();
        }
    }
}