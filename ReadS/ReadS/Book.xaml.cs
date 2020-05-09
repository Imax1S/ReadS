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
        WebView webView;
        Entry urlEntry;
        public Book(EpubBook book)
        {
            InitializeComponent();
            Button rightButton = new Button();
            rightButton.Clicked += rightClick;
            rightButton.HorizontalOptions = LayoutOptions.End;
            //rightButton.Opacity = 0;


            Button leftButton = new Button();
            leftButton.Clicked += leftClick;
            leftButton.HorizontalOptions = LayoutOptions.Start;
            //leftButton.Opacity = 0;

            //label = readBook(book);
            //scroll = new ScrollView()
            //{
            //    Orientation = ScrollOrientation.Vertical
            //};

            urlEntry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand };
            Button button = new Button { Text = "Go" };

            StackLayout stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { button, urlEntry }
            };

            var htmlSource = new HtmlWebViewSource();
            StringWriter myWriter = new StringWriter();

            HttpUtility.HtmlDecode(readBook(book), myWriter);
            //htmlSource.Html = book;

            webView = new WebView
            {
                //Source = new UrlWebViewSource { Url = "http://blog.xamarin.com/" },
                // или так
                Source = htmlSource,
                VerticalOptions = LayoutOptions.FillAndExpand,

            };

            


            this.Content = new StackLayout { Children = { stack, webView } };
        }

        public string readBook(EpubBook book)
        {
            string chapterHtmlContent = "";
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
            return htmlCon;
        }

        public void rightClick(object sender, EventArgs e)
        {
            positionOfBook += 770;
            Goal.pagesRead++;
            // scroll.ScrollToAsync(0, positionOfBook, false); //scrolls so that the position at 150px from the top is visible
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
}