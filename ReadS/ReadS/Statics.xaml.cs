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
    public partial class Statics : ContentPage
    {
        List<Entry> entries = new List<Entry>
        {
            new Entry(900)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Январь",
                ValueLabel = Goal.goalPages.ToString()
            },

            new Entry(1500)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Февраль",
                ValueLabel = Goal.goalPages.ToString()
            },

            new Entry(3000)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Апрель",
                ValueLabel = Goal.goalPages.ToString()
            },
        };
        public Statics()
        {
            InitializeComponent();

            StatsOfReadingByDates.HorizontalOptions = LayoutOptions.FillAndExpand;
            StatsOfReadingByDates.VerticalOptions = LayoutOptions.FillAndExpand;
            //StatsOfReading.Chart = new Microcharts.RadialGaugeChart {Entries = entries };
            //StatsOfReading.Chart = new Microcharts.DonutChart { Entries = entries };
            StatsOfReadingByDates.Chart = new Microcharts.BarChart { Entries = entries };
        }
    }
}