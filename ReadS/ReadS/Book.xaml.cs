using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VersFx.Formats.Text.Epub;
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

            var htmlWebView = new HtmlWebViewSource();
            htmlWebView.Html = @readBook(book);

            //htmlWebView.Html = htmlCon;

            WebView webView = new WebView();
            webView.Source = htmlWebView;
           // scroll.Content = label;
            Content = new StackLayout()
            {
                Children = { webView }
            };
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
            return chapterHtmlContent;
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