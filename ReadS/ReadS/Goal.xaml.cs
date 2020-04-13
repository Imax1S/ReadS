using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;
using SkiaSharp;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Goal : ContentPage
    {
        Label pages = new Label();
        public static int goalPages = 6;
        public static int pagesRead = 0;
        static Microcharts.Forms.ChartView StatsOfReading = new Microcharts.Forms.ChartView();
        List<Entry> entries = new List<Entry>
        {
            new Entry(Goal.goalPages)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Осталось",
                ValueLabel = Goal.goalPages.ToString()
            },

            new Entry(Goal.goalPages - pagesRead)
            {
                Color = SKColor.Parse("#0F9D58"),
                Label = "Прочитано",
                ValueLabel = (Goal.goalPages - 1).ToString()
            }
        };
        public Goal()
        {
            InitializeComponent();

            Slider slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
            };
            slider.VerticalOptions = LayoutOptions.FillAndExpand;
            slider.VerticalOptions = LayoutOptions.CenterAndExpand;
            slider.ThumbColor = Color.Gray;
            slider.MinimumTrackColor = Color.CadetBlue;
            //slider = 100;

            pages.Text = "Передвиньте ползунок, чтобы поставить цель";
            pages.VerticalOptions = LayoutOptions.CenterAndExpand;
            pages.HorizontalOptions = LayoutOptions.CenterAndExpand;
            pages.FontSize = 20;


            slider.ValueChanged += (sender, args) =>
            {
                if (slider.Value == 0)
                {
                    pages.Text = "Передвиньте ползунок, чтобы поставить цель";
                    goalPages = (int)slider.Value;
                    StatsOfReading.Opacity = 0;
                }
                else
                {
                    StatsOfReading.Opacity = 100;
                    goalPages = (int)slider.Value;
                    pages.Text = String.Format("Цель {0} страниц", (int)args.NewValue);
                    RefreshGoalGraph();
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
            new Entry(Goal.goalPages - pagesRead)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Осталось",
                ValueLabel = Goal.goalPages.ToString()
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
    }
}