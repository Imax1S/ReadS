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
        string htmlCon = "";
        double startSizeText = 1.0;
        public static int positionOfBook = 0;
        int loadedContent = 10000;
        int counterPage = 1;
        string fullHtml = "";
        Grid grid;
        Button left;
        Button right;
        HtmlWebViewSource urlSource = new HtmlWebViewSource();
        Label numberOfPage;
        double width = Application.Current.MainPage.Width;
        double height = Application.Current.MainPage.Height;
        Entry searchEntry = new Entry();

        string html;
        public List<int> numberOfPages = new List<int>();
        ScrollView scroll = new ScrollView();
        WebViewer webViewer;
        public int amountOfPages;

        public Book(EpubBook book)
        {

            InitializeComponent();

            this.Title = book.Title;


            //Чтение книги
            htmlCon = ReadBook(book);
            int counterLetters = 0;
            List<string> words = htmlCon.Split().ToList();
            for (int i = 0; i < words.Count; i++)
            {

                if (counterLetters == 80)
                {
                    amountOfPages++;
                    counterLetters = 0;
                }
                counterLetters++;
            }
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
            textDown.Clicked += DecreaseTextSize;
            textUp.Clicked += IncreaseTextSize;
            search.Clicked += SearchPage;
            this.ToolbarItems.Add(textDown);
            this.ToolbarItems.Add(textUp);
            this.ToolbarItems.Add(search);

            grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
                }
            };

            numberOfPage = new Label()
            {
                Text = counterPage.ToString(),
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Padding = 10,
                FontFamily = "Kurale",
            };

            using (StreamWriter writer = new StreamWriter(Path.Combine(FileSystem.CacheDirectory, "style.css"), true, Encoding.Default))
            {
                writer.Write(GetCss(book));
            }

            html = ReadBook(book);
            html = html.Replace("<title/>", "");
            string headerString = "<header><meta name='viewport' content='width=device-width, initial-scale=0.9, maximum-scale=5.0, minimum-scale=1.0, user-scalable=no'><style>img{max-width:100%}</style></header>";

            fullHtml = headerString + html;
            urlSource.Html = fullHtml;
            urlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();

            webViewer = new WebViewer()
            {
                Source = urlSource,
            };
            webViewer.HeightRequest = 100000;
            webViewer.WidthRequest = 500;
            webViewer.Navigating += this.NavigatingEvent;
            scroll.Content = webViewer;


            right = new Button();
            right.Clicked += rightClick;
            right.Opacity = 0;

            left = new Button();
            left.Clicked += leftClick;
            left.Opacity = 0;

            grid.Children.Add(scroll, 0, 3, 1, 3);
            grid.Children.Add(right, 2, 3, 0, 4);
            grid.Children.Add(left, 0, 1, 0, 4);
            grid.Children.Add(numberOfPage, 0, 3, 3, 4);

            this.Content = grid;
        }

        private void NavigatingEvent(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.Contains("bottom") || e.Url.Contains("about:blank"))
            {
                e.Cancel = true;
            }
        }

        public string ReadBook(EpubBook book)
        {
            //Переменная в которую добавляется весь текст html
            string chapterHtmlContent = "";

            //Чтение по главам
            foreach (EpubChapter chapter in book.Chapters)
            {
                chapterHtmlContent += chapter.HtmlContent;

                //Чтение всех подглав
                List<EpubChapter> subChapters = chapter.SubChapters;
                foreach (EpubChapter item in subChapters)
                {
                    chapterHtmlContent += item.HtmlContent;
                    htmlCon = chapter.HtmlContent;
                }
            }
            return chapterHtmlContent;
        }

        public string GetCss(EpubBook book)
        {
            string cssContent = "";
            foreach (EpubTextContentFile cssFile in book.Content.Css.Values)
            {
                cssContent += cssFile.Content;
            }
            return cssContent;
        }

        public void rightClick(object sender, EventArgs e)
        {
            positionOfBook += 720;
            if (positionOfBook > loadedContent - 1000)
            {
                webViewer.Reload();
                loadedContent += 10000;
            }
            Goal.pagesRead++;
            counterPage++;
            numberOfPage.Text = counterPage.ToString();
            scroll.ScrollToAsync(0, positionOfBook, false);
            Goal.pagesRead += 1;
            Goal.SaveGoal();
            Goal.RefreshGoalGraph();
        }

        public void leftClick(object sender, EventArgs e)
        {
            if (positionOfBook > 0)
            {
                Goal.pagesRead--;
                positionOfBook -= 720;
                counterPage--;
                numberOfPage.Text = counterPage.ToString();
            }
            scroll.ScrollToAsync(0, positionOfBook, false);
        }

        public void DecreaseTextSize(object sender, EventArgs e)
        {
            html = html.Replace("<title/>", "");
            string headerString = "<header><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=5.0, minimum-scale=0.8, user-scalable=no'><style>img{max-width:100%}</style></header>";
            urlSource.Html = headerString + html;
            startSizeText -= 0.1;
            WebViewer webViewer = new WebViewer();
            webViewer.Source = urlSource;
            ScrollView scroll = new ScrollView();
            scroll.Content = webViewer;
            Button newRight = new Button();
            Button newLeft = new Button();
            newRight.Clicked += rightClick;
            newLeft.Clicked += leftClick;
            newLeft.Opacity = 0;
            newRight.Opacity = 0;


            grid.Children.Add(scroll, 0, 3, 1, 3);
            grid.Children.Add(newRight, 2, 3, 0, 4);
            grid.Children.Add(newLeft, 0, 1, 0, 4);
        }

        public void IncreaseTextSize(object sender, EventArgs e)
        {
            html = html.Replace("<title/>", "");
            string headerString = "<header><meta name='viewport' content='width=device-width, initial-scale=2.0, maximum-scale=5.0, minimum-scale=1.0, user-scalable=no'><style>img{max-width:100%}</style></header>";
            HtmlWebViewSource urlSource = new HtmlWebViewSource();
            urlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            urlSource.Html = headerString + html;
            startSizeText += 0.1;
            WebViewer webViewer = new WebViewer();
            webViewer.Source = urlSource;
            scroll = new ScrollView();
            scroll.Content = webViewer;
            Button newRight = new Button();
            Button newLeft = new Button();
            newRight.Clicked += rightClick;
            newLeft.Clicked += leftClick;
            newLeft.Opacity = 0;
            newRight.Opacity = 0;


            grid.Children.Add(scroll, 0, 3, 1, 3);
            grid.Children.Add(newRight, 2, 3, 0, 4);
            grid.Children.Add(newLeft, 0, 1, 0, 4);
        }

        public async void SearchPage(object sender, EventArgs e)
        {
            string search = await DisplayPromptAsync("Поиск по страницам", "К какой странице вы желаете перейти?");
            if (int.TryParse(search, out int findPageNum) && findPageNum > 0 && findPageNum <= amountOfPages)
            {
                positionOfBook = findPageNum * 730;
                await scroll.ScrollToAsync(0, positionOfBook, false);
                counterPage = findPageNum;
                numberOfPage.Text = findPageNum.ToString();
            }
        }
    }

    public interface IBaseUrl { string Get(); }
}