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
using VersFx.Formats.Text.Epub;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Goal : ContentPage
    {
        //Создание переменных
        Label pages = new Label();
        public static int goalPages = 0;
        public static int pagesRead = 0;
        static Microcharts.Forms.ChartView StatsOfReading = new Microcharts.Forms.ChartView();
        static string filename = Path.Combine(FileSystem.AppDataDirectory, "stats.json");
        List<DayStat> dayStats = new List<DayStat>();
        DateTime choosenDayForGoal;
        static public bool MobilePages = true;

        //Слайдер для цели
        Slider slider = new Slider
        {
            Minimum = 0,
            Maximum = 500,
        };

        //Кнопка для переключения подсчета страниц
        ToolbarItem bookOrMobilePages = new ToolbarItem()
        {
            Text = "Подсчет в мобильных страницах",
            Order = ToolbarItemOrder.Secondary,
            Priority = 0,
        };

        //Выбор книги
        public static Picker bookPicker = new Picker
        {
            Title = "Выберите книгу",
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        public Goal()
        {
            InitializeComponent();

            //Добавление функций к кнопкам
            bookOrMobilePages.Clicked += ChangeCounterOfPage;
            this.ToolbarItems.Add(bookOrMobilePages);

            try
            {
                //Чтение файла с статистикой
                using (StreamReader reader = new StreamReader(filename))
                {
                    string json = reader.ReadToEnd();

                    //Если json не пустой
                    if (json != "")
                    {
                        dayStats = JsonConvert.DeserializeObject<List<DayStat>>(json);

                        //Если последний день из файла соответсвует сегодняшнему дню, то просто берутся значения goalPages и pagesRead 
                        if (DateTime.Today == dayStats[dayStats.Count - 1].Date)
                        {
                            goalPages = dayStats[dayStats.Count - 1].Goal;
                            pagesRead = dayStats[dayStats.Count - 1].Pages;
                        }
                        else
                        {
                            //Если же не соответсвует, то читается последняя информация о goalPages, а для pagesRead ставится 0
                            goalPages = dayStats[dayStats.Count - 1].Goal;
                            pagesRead = 0;

                            //Для каждого дня между последним записанным и сегодняшнем ставится pagesRead 0
                            int leftDays = (int)(DateTime.Today - dayStats[dayStats.Count - 1].Date).TotalDays;
                            for (int i = leftDays; i > 0; i--)
                            {
                                dayStats.Add(new DayStat(0, DateTime.Today.AddDays(-i), goalPages));
                            }
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

            //Настройка слайдера
            slider.Value = goalPages;
            slider.VerticalOptions = LayoutOptions.FillAndExpand;
            slider.VerticalOptions = LayoutOptions.CenterAndExpand;
            slider.ThumbColor = Color.Gray;
            slider.MinimumTrackColor = Color.CadetBlue;
            slider.Margin = 20;

            //Создание сетки
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1 , GridUnitType.Star) }
                }
            };

            //Степпер для цели
            Stepper stepper = new Stepper
            {
                Value = slider.Value,
                Minimum = 0,
                Maximum = 500,
                Increment = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            stepper.ValueChanged += OnStepperValueChanged;
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            //Настройка label с информацией о цели
            pages.VerticalOptions = LayoutOptions.CenterAndExpand;
            pages.HorizontalOptions = LayoutOptions.CenterAndExpand;
            pages.FontSize = 20;

            //Если цель равна 0
            if (goalPages == 0)
            {
                pages.Text = "Передвиньте ползунок, чтобы поставить цель или выберите книгу и дату, чтобы рассчитать цель";
            }
            else
            {
                //Если не равна, то проверяет достигнута цель или нет
                StatsOfReading.Opacity = 100;
                slider.AnchorX = goalPages;
                if (goalPages <= pagesRead)
                {
                    pages.Text = String.Format("Цель {0} страниц выполнена!", goalPages);
                    RefreshGoalGraph();
                }
                else
                {
                    pages.Text = String.Format("Цель {0} страниц в день", goalPages);
                    RefreshGoalGraph();
                }
            }

            //Если значения слайдера изменилось
            slider.ValueChanged += (sender, args) =>
            {
                //Проверяем цель
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
                stepper.Value = (int)args.NewValue;
            };

            //Создаем поле для выбора даты
            DatePicker datePicker = new DatePicker
            {
                Format = "D",
                MaximumDate = DateTime.Now.AddDays(80),
                MinimumDate = DateTime.Now.AddDays(-80),
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            datePicker.DateSelected += datePicker_DateSelected;

            //Добавляем в контейнер все загруженные книги
            foreach (string book in Books.books.Keys)
            {
                bookPicker.Items.Add(book);
            }

            //Вызывается когда bookPicker меняется
            bookPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (datePicker.Date != DateTime.Today)
                {
                    CountGoal(datePicker.Date, Books.books[bookPicker.Items[bookPicker.SelectedIndex]]);
                }
            };

            //Добавляем элементы
            grid.Children.Add(StatsOfReading, 0, 0);
            Grid.SetColumnSpan(StatsOfReading, 3);

            grid.Children.Add(pages, 0, 1);
            Grid.SetColumnSpan(pages, 3);

            grid.Children.Add(slider, 0, 2);
            Grid.SetColumnSpan(slider, 3);

            grid.Children.Add(stepper, 0, 3);
            Grid.SetColumnSpan(stepper, 3);

            grid.Children.Add(bookPicker, 0, 4);
            Grid.SetColumnSpan(bookPicker, 2);

            grid.Children.Add(datePicker, 2, 4);

            //Добавляем сетку на страницу
            Content = new StackLayout
            {
                Children =
                    {
                        grid
                    }
            };
        }

        /// <summary>
        /// Переключатель подсчета страниц
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeCounterOfPage(object sender, EventArgs e)
        {
            bookOrMobilePages.Text = "Подсчет в книжных страницах";
            MobilePages = false;
        }

        /// <summary>
        /// Меняет дату цели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            choosenDayForGoal = e.NewDate;
            if (bookPicker.SelectedIndex >= 0)
            {
                CountGoal(e.NewDate, Books.books[bookPicker.Items[bookPicker.SelectedIndex]]);
            }
        }

        /// <summary>
        /// Считает цель
        /// </summary>
        /// <param name="deadline"></param>
        /// <param name="book"></param>
        private void CountGoal(DateTime deadline, EpubBook book)
        {
            int amountOfPages = new Book(book).amountOfPages;
            int daysLeft = (deadline - DateTime.Today).Days;
            if (daysLeft > 0)
            {
                if (MobilePages)
                {
                    slider.Value = amountOfPages / daysLeft;
                }
                else
                {
                    amountOfPages /= 4;
                    slider.Value = amountOfPages / daysLeft;
                }
            }
            else
            {
                pages.Text = "Чтобы выполнить цель необходима машина времени";
                return;
            }

            pages.Text = "Чтобы выполнить цель необходимо читать по " + goalPages + " страниц";
        }

        /// <summary>
        /// Меняет значение цели через степпер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            slider.Value = e.NewValue;
        }

        /// <summary>
        /// Обновляет график
        /// </summary>
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


        /// <summary>
        /// Сохраняет цель и всю статистику. После чего обноваляет все графики
        /// </summary>
        public static void SaveGoal()
        {
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

            //Обновлят все графики
            Statics.FillGraph();
        }
    }
}