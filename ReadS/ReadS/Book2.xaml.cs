using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersFx.Formats.Text.Epub;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Book2 : CarouselPage
    {
        string htmlCon = "";
        public static int positionOfBook = 0;
        List<ContentPage> pages = new List<ContentPage>();
        //CarouselView carouselView = new CarouselView();
        List<StackLayout> stacks = new List<StackLayout>();
        int index = 1;
        ToolbarItem item = new ToolbarItem
        {
            Text = "Шрифт",
            // IconImageSource = ImageSource.FromFile("baseline_more_vert_black_48dp.png"),
            Order = ToolbarItemOrder.Secondary,
            Priority = 0
        };

        public Book2(EpubBook book)
        {
            InitializeComponent();
            this.ToolbarItems.Add(item);

            htmlCon = readBook(book);
            string temp = "";
            Label label = new Label()
            {
                Text = htmlCon,
                TextType = TextType.Html,
                FontSize = 20
            };

            Image image = new Image();
            image.Source = ImageSource.FromStream(() => new MemoryStream(book.CoverImage));
            pages.Add(new ContentPage()
            {
                Content = new ImageButton()
                {
                    Source = image.Source
                }
            });

            int counterPage = 1;
            int counterLetters = 0;
            for (int i = 0; i < htmlCon.Length; i++)
            {
                temp += htmlCon[i];
                if (counterLetters == 950)
                {
                    //Перенос по словам
                    //if (htmlCon[i] != ' ' && i < htmlCon.Length - 2 )
                    //{
                    //    temp = "";
                    //    while (htmlCon[i] != ' ')
                    //    {
                    //        i--;
                    //    }
                    //    for (int j = 0; j <= i; j++)
                    //    {
                    //        temp += htmlCon[j];
                    //    }
                    //}
                    //stacks.Add(new StackLayout()
                    //{
                    //    Children = {new Label(){
                    //            FontSize = 18,
                    //            TextType = TextType.Html,
                    //            Text = temp,
                    //            VerticalOptions = LayoutOptions.CenterAndExpand,
                    //            HorizontalOptions = LayoutOptions.CenterAndExpand,
                    //            Padding = 15,
                    //            TextColor = Color.Black,
                    //            FontFamily = "Kurale"},
                    //            new Label()
                    //            {
                    //            FontSize = 10,
                    //            Text = (counterPage++).ToString(),
                    //            VerticalOptions = LayoutOptions.Center,
                    //            HorizontalOptions = LayoutOptions.Center,
                    //            Padding = 10,
                    //            FontFamily = "Kurale"
                    //            }
                    //        }
                    //});
                    pages.Add(new ContentPage()
                    {
                        Content = new StackLayout()
                        {
                            Children = {new Label(){
                                FontSize = 18,
                                TextType = TextType.Html,
                                Text = temp,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                Padding = 15,
                                TextColor = Color.Black,
                                FontFamily = "Kurale"},
                                new Label()
                                {
                                FontSize = 10,
                                Text = (counterPage++).ToString(),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                                Padding = 10,
                                FontFamily = "Kurale"
                                }
                            }
                        }
                    });
                    counterLetters = 0;
                    temp = "";
                }
                counterLetters++;
            }

            foreach (ContentPage item in pages)
            {
                //carouselView.AddLogicalChild(item);
                this.Children.Add(item);
            }
            //ContentPage page = new ContentPage();
            //StackLayout stackLayout = new StackLayout();
            //stackLayout.Children.Add(carouselView);
            //page.Content = stackLayout;
            //this.Children.Add(page);
        }

        public string readBook(EpubBook book)
        {
            Label label;
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
            //label = new Label();
            //label.Text = chapterHtmlContent;
            //label.FontSize = 15;
            //label.TextType = TextType.Html;
            return chapterHtmlContent;
        }


        protected override void OnCurrentPageChanged()
        {
            if (index < Children.IndexOf(CurrentPage))
            {
                Goal.pagesRead++;
                Goal.SaveGoal();
                Goal.RefreshGoalGraph();
                index = Children.IndexOf(CurrentPage);
            }
        }
    }
}