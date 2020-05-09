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
    //Book View witch uses carusel view
    public partial class Book2 : CarouselPage
    {
        //creating string for text and int for counting pages
        string htmlCon = "";
        public static int positionOfBook = 0;
        List<ContentPage> pages = new List<ContentPage>();
        //CarouselView carouselView = new CarouselView();
        List<StackLayout> stacks = new List<StackLayout>();
        List<WebView> webViews = new List<WebView>();
        int index = 1;

        //Toolbar for resize text, search pages and style
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
            List<string> words = htmlCon.Split().ToList();
            List<StackLayout> layout = new List<StackLayout>();
            for (int i = 0; i < words.Count; i++)
            {
                temp += words[i] + " ";
                if (counterLetters == 125)
                {
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
                    //layout.Add(new StackLayout()
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
                    //                FontSize = 10,
                    //                Text = (counterPage++).ToString(),
                    //                VerticalOptions = LayoutOptions.Center,
                    //                HorizontalOptions = LayoutOptions.Center,
                    //                Padding = 10,
                    //                FontFamily = "Kurale"
                    //            }
                    //        }
                    //});
                    var htmlSource = new HtmlWebViewSource();
                    htmlSource.Html = temp;


                    webViews.Add(new WebView()
                    {
                        Source = htmlSource,
                        
                    });
                    counterLetters = 0;
                    temp = "";
                }
                counterLetters++;
            }


            //CarouselView carousel = new CarouselView()
            //{
            //    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            //    {
            //        ItemSpacing = 20
            //    }
            //};
            //carousel.ItemsSource = layout;
            //ContentPage contentPage = new ContentPage() { Content = carousel };
            //this.Children.Add(contentPage);


            //foreach (ContentPage item in pages)
            //{
            //    this.Children.Add(item);
            //}
            this.Children.Add(new ContentPage()
            {
                Content = new ImageButton()
                {
                    Source = image.Source
                }
            });

            foreach (WebView item in webViews)
            {
                this.Children.Add(new ContentPage() { Content = item});
            }
            //ContentPage page = new ContentPage();
            //StackLayout stackLayout = new StackLayout();
            //stackLayout.Children.Add(carouselView);
            //page.Content = stackLayout;
            //this.Children.Add(page);
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