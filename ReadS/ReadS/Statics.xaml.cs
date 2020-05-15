using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Statics : TabbedPage
    {
        public Statics()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вызов всех функций для отрисовки    
        /// </summary>
        static public void FillGraph()
        {
            Days.FillDayGraph();
            Weeks.FillWeekGraph();
            Months.FillMonthGraph();
        }
    }
}