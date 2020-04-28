using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;
using SkiaSharp;
using SQLite;

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
                ValueLabel = "900 стр"
            },

            new Entry(1500)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Февраль",
                ValueLabel = "1500 стр"
    },

            new Entry(3000)
            {
                Color = SKColor.Parse("#4285F4"),
                Label = "Апрель",
                ValueLabel ="3000 стр"
    },
        };
        public Statics()
        {
            InitializeComponent();

            StatsOfReadingByDates.HorizontalOptions = LayoutOptions.Fill;
            StatsOfReadingByDates.VerticalOptions = LayoutOptions.FillAndExpand;
            StatsOfReadingByDates.Margin = 30;
            //StatsOfReading.Chart = new Microcharts.RadialGaugeChart {Entries = entries };
            //StatsOfReading.Chart = new Microcharts.DonutChart { Entries = entries };
            
            StatsOfReadingByDates.Chart = new Microcharts.BarChart { Entries = entries, LabelTextSize = 40, BackgroundColor = SKColor.Parse("#FFFFFF")};
        }
    }
}