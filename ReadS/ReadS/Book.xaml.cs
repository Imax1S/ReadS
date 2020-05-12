using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VersFx.Formats.Text.Epub;
using VersFx.Formats.Text.Epub.Schema.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Book : ContentPage
    {
        //ScrollView scroll;
        //Label label;
        string htmlCon = "";
        public static int positionOfBook = 0;

        //Страницы с webViews
        List<WebView> webViews = new List<WebView>();

        public List<int> numberOfPages = new List<int>();
        ScrollView scroll = new ScrollView();
        public Book(EpubBook book)
        {
            InitializeComponent();
            this.Title = book.Title;

            ToolbarItem search = new ToolbarItem
            {

                IconImageSource = ImageSource.FromFile("outline_search_white_48dp.png"),
                Order = ToolbarItemOrder.Default,
                Priority = 0,

            };
            ToolbarItem textUp = new ToolbarItem
            {

                IconImageSource = ImageSource.FromFile("round_text_format_black_48dp.png"),
                Order = ToolbarItemOrder.Default,
                Priority = 0,

            };
            ToolbarItem textDown = new ToolbarItem
            {

                IconImageSource = ImageSource.FromFile("round_text_format_white_48dp.png"),
                Order = ToolbarItemOrder.Default,
                Priority = 0,

            };
            Stepper sizeText = new Stepper()
            {
                Value = 12,
                Minimum = 0,
                Maximum = 100,
                Increment = 1,
            };

            this.ToolbarItems.Add(textUp);
            this.ToolbarItems.Add(textDown);
            this.ToolbarItems.Add(search);

            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
                }
            };
            #region Some Grid elements
            //grid.Children.Add(new Label
            //{
            //    Text = book.Title,
            //    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            //    HorizontalOptions = LayoutOptions.Center
            //}, 0, 3, 0, 1);

            //grid.Children.Add(new Label
            //{
            //    Text = "Fixed 100x100",
            //    TextColor = Color.Aqua,
            //    BackgroundColor = Color.Red,
            //    HorizontalTextAlignment = TextAlignment.Center,
            //    VerticalTextAlignment = TextAlignment.Center
            //}, 2, 3);

            //grid.Children.Add(new Label
            //{
            //    Text = "Span two rows (or more if you want)",
            //    TextColor = Color.Yellow,
            //    BackgroundColor = Color.Navy,
            //    HorizontalTextAlignment = TextAlignment.Center,
            //    VerticalTextAlignment = TextAlignment.Center
            //}, 2, 3, 1, 3);

            //grid.Children.Add(new Label
            //{
            //    Text = "Autosized cell",
            //    TextColor = Color.White,
            //    BackgroundColor = Color.Blue
            //}, 0, 1);

            //grid.Children.Add(new BoxView
            //{
            //    Color = Color.Silver,
            //    HeightRequest = 0
            //}, 1, 1);

            //grid.Children.Add(new BoxView
            //{
            //    Color = Color.Teal
            //}, 0, 2);

            // Accomodate iPhone status bar.
            //this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
            #endregion


            //Чтение книги
            htmlCon = readBook(book);
            string temp = "";
            int counterPage = 1;
            int counterLetters = 0;
            List<string> words = htmlCon.Split().ToList();
            for (int i = 0; i < words.Count; i++)
            {
                temp += words[i] + " ";
                if (counterLetters == 150)
                {

                    var htmlSource = new HtmlWebViewSource();
                    temp = temp.Replace("<title/>", "");
                    htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
                    htmlSource.Html = temp;

                    webViews.Add(new WebView()
                    {
                        Source = htmlSource,
                    });
                    numberOfPages.Add(counterPage++);
                    counterLetters = 0;
                    temp = "";
                }
                counterLetters++;
            }

            CarouselView carousel = new CarouselView()
            {
                ItemsSource = numberOfPages,
            };
            carousel.ItemTemplate = new DataTemplate(() =>
            {
                Label nameLabel = new Label { Text = "Kek", FontSize = 20 };
                //nameLabel.SetBinding(Label.TextProperty, "Name");

                Image image = new Image { Source = ImageSource.FromStream(() => new MemoryStream(book.CoverImage)) };
                //image.SetBinding(Image.SourceProperty, "ImageUrl");

                Label locationLabel = new Label { Text = "Kek" };
                //locationLabel.SetBinding(Label.TextProperty, "Location");

                Label detailsLabel = new Label { Text = "Kek" };
                //detailsLabel.SetBinding(Label.TextProperty, "Details");

                StackLayout stackLayout = new StackLayout
                {
                    Children = { nameLabel, image, locationLabel, detailsLabel }
                };


                Frame frame = new Frame { Content = stackLayout };
                StackLayout rootStackLayout = new StackLayout
                {
                    Children = { frame }
                };

                return rootStackLayout;
            });

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Books)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("ReadS.XMLFile1.html");


            using (StreamWriter writer = new StreamWriter(Path.Combine(FileSystem.CacheDirectory, "style.css"), true, Encoding.Default))
            {
                writer.Write(GetHtmlSourceWithCustomCss(book));
            }

            var urlSource = new HtmlWebViewSource();
            string html = readBook(book);
            html = html.Replace("<title/>", "");
            urlSource.Html = html;
            urlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            WebView webViewAll = new WebView()
            {
                Source = urlSource,
            };
            WebViewer webViewer = new WebViewer()
            {
                Source = urlSource,
            };
            webViewer.HeightRequest = 1000;
            webViewer.WidthRequest = 500;
            webViewer.Navigating += this.NavigatingEvent;
            scroll.Content = webViewer;
            Button right = new Button();
            right.Clicked += rightClick;
            //right.Opacity = 0;

            grid.Children.Add(right, 0, 3, 1, 3);
            grid.Children.Add(scroll, 0, 3, 1, 3);


            Slider slider = new Slider
            {
                Minimum = 0,
                Maximum = 100,
                Value = 10,
                ThumbColor = Color.Gray,
                MinimumTrackColor = Color.CadetBlue,
                Margin = 20,
            };
            slider.VerticalOptions = LayoutOptions.FillAndExpand;
            slider.VerticalOptions = LayoutOptions.CenterAndExpand;
            slider.ValueChanged += (sender, args) =>
            {
                
            };

          //grid.Children.Add(new Slider
          // {
          //     Minimum = 0,
          //     Maximum = 100,
          //     ThumbColor = Color.Gray,
          //     MinimumTrackColor = Color.CadetBlue,
          // }, 0, 3, 3, 4);

            // Build the page.
            this.Content = grid;
            //this.Content = new StackLayout { Children = { webView } };
        }

        private void NavigatingEvent(object sender, WebNavigatingEventArgs e)
        {
           
        }

        public string readBook(EpubBook book)
        {
            string chapterHtmlContent = "";

            //Читаем по главам
            foreach (EpubChapter chapter in book.Chapters)
            {
                // Title of chapter
                //chapterHtmlContent += chapter.Title;

                // HTML content of current chapter
                chapterHtmlContent += chapter.HtmlContent;

                // Nested chapters
                List<EpubChapter> subChapters = chapter.SubChapters;
                foreach (EpubChapter item in subChapters)
                {
                    chapterHtmlContent += item.HtmlContent;
                    htmlCon = chapter.HtmlContent;
                }
            }

            //Читаем весь html
            string htmlContent = "";
            Dictionary<string, EpubTextContentFile> htmlFiles = book.Content.Html;
            foreach (EpubTextContentFile htmlFile in htmlFiles.Values)
            {
                htmlContent += htmlFile.Content;
            }


            //label = new Label();
            //label.Text = chapterHtmlContent;
            //label.FontSize = 15;
            //label.TextType = TextType.Html;

            //return htmlContent;
            return chapterHtmlContent;
        }

        public string GetHtmlSourceWithCustomCss(EpubBook book)
        {
            string cssContent = "";
            foreach (EpubTextContentFile cssFile in book.Content.Css.Values)
            {
                cssContent += cssFile.Content;
            }

            // Replace css
            //var customCss = book.Content.Css;
            //htmlCode = htmlCode.Replace("<head>", cssContent);

            //htmlSource.Html = htmlCode;
            return cssContent;
        }

        public void rightClick(object sender, EventArgs e)
        {
            positionOfBook += 770;
            Goal.pagesRead++;
            scroll.ScrollToAsync(0, positionOfBook, false); //scrolls so that the position at 150px from the top is visible
            Goal.pagesRead += 1;
            Goal.RefreshGoalGraph();
        }

        public void leftClick(object sender, EventArgs e)
        {
            if (positionOfBook > 0)
            {
                positionOfBook -= 770;
            }
            //scroll.ScrollToAsync(0, positionOfBook, false); //scrolls so that the position at 150px from the top is visible
        }
    }

    public interface IBaseUrl { string Get(); }
}