using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VersFx.Formats.Text.Epub;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.StyleSheets;
using Xamarin.Forms.Xaml;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    //Book View witch uses carusel view
    public partial class Book2 : CarouselPage
    {
        //Переменные для получения html и подчета страниц
        string htmlCon = "";
        public static int positionOfBook = 0;

        //Страницы
        List<ContentPage> pages = new List<ContentPage>();

        //Страницы с webViews
        List<WebView> webViews = new List<WebView>();

        double width = Application.Current.MainPage.Width;
        double height = Application.Current.MainPage.Height;

        int index = 1;

        List<Label> labelsPage = new List<Label>();

        public Book2(EpubBook book)
        {
            InitializeComponent();
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
            textDown.Clicked += DecreaseTextSize;
            textUp.Clicked += IncreaseTextSize;

            this.ToolbarItems.Add(textUp);
            this.ToolbarItems.Add(textDown);
            this.ToolbarItems.Add(search);

            htmlCon = readBook(book);
            htmlCon = htmlCon.Replace("<title/>", "");
            string css = GetHtmlSourceWithCustomCss(book);
            Dictionary<string, string> transliter = new Dictionary<string, string>();
            #region Алфавит
            transliter.Add("а", "a");
            transliter.Add("б", "b");
            transliter.Add("в", "v");
            transliter.Add("г", "g");
            transliter.Add("д", "d");
            transliter.Add("е", "e");
            transliter.Add("ё", "yo");
            transliter.Add("ж", "zh");
            transliter.Add("з", "z");
            transliter.Add("и", "i");
            transliter.Add("й", "j");
            transliter.Add("к", "k");
            transliter.Add("л", "l");
            transliter.Add("м", "m");
            transliter.Add("н", "n");
            transliter.Add("о", "o");
            transliter.Add("п", "p");
            transliter.Add("р", "r");
            transliter.Add("с", "s");
            transliter.Add("т", "t");
            transliter.Add("у", "u");
            transliter.Add("ф", "f");
            transliter.Add("х", "h");
            transliter.Add("ц", "c");
            transliter.Add("ч", "ch");
            transliter.Add("ш", "sh");
            transliter.Add("щ", "sch");
            transliter.Add("ъ", "j");
            transliter.Add("ы", "i");
            transliter.Add("ь", "j");
            transliter.Add("э", "e");
            transliter.Add("ю", "yu");
            transliter.Add("я", "ya");
            transliter.Add("А", "A");
            transliter.Add("Б", "B");
            transliter.Add("В", "V");
            transliter.Add("Г", "G");
            transliter.Add("Д", "D");
            transliter.Add("Е", "E");
            transliter.Add("Ё", "Yo");
            transliter.Add("Ж", "Zh");
            transliter.Add("З", "Z");
            transliter.Add("И", "I");
            transliter.Add("Й", "J");
            transliter.Add("К", "K");
            transliter.Add("Л", "L");
            transliter.Add("М", "M");
            transliter.Add("Н", "N");
            transliter.Add("О", "O");
            transliter.Add("П", "P");
            transliter.Add("Р", "R");
            transliter.Add("С", "S");
            transliter.Add("Т", "T");
            transliter.Add("У", "U");
            transliter.Add("Ф", "F");
            transliter.Add("Х", "H");
            transliter.Add("Ц", "C");
            transliter.Add("Ч", "Ch");
            transliter.Add("Ш", "Sh");
            transliter.Add("Щ", "Sch");
            transliter.Add("Ъ", "J");
            transliter.Add("Ы", "I");
            transliter.Add("Ь", "J");
            transliter.Add("Э", "E");
            transliter.Add("Ю", "Yu");
            transliter.Add("Я", "Ya");
            #endregion

            string newFolder = String.Join("", book.Title.Split());
            foreach (KeyValuePair<string, string> pair in transliter)
            {
                newFolder = newFolder.Replace(pair.Key, pair.Value);
            }
            //string fileDirectory = Path.Combine(FileSystem.CacheDirectory, newFolder);
            string fileDirectory = FileSystem.CacheDirectory;

            //using (StreamWriter writer = new StreamWriter(Path.Combine(fileDirectory, "style.css")))
            //{
            //    writer.Write(css);
            //}
            Dictionary<string, EpubContentFile> allFiles = book.Content.AllFiles;

            string[] files = Directory.GetFiles(FileSystem.CacheDirectory);
            //foreach (string filePath in files)
            //    File.Delete(filePath);
            //string[] filesNew = Directory.GetFiles(FileSystem.CacheDirectory);
            //foreach (EpubContentFile item in allFiles.Values)
            //{
            //    try
            //    {
            //        string path = Path.Combine(fileDirectory, item.FileName);
            //        if (!Directory.Exists(Path.GetDirectoryName(path)))
            //        {
            //            Directory.CreateDirectory(path);
            //        }


            //        string[] directories = Directory.GetDirectories(FileSystem.CacheDirectory);

            //        using (StreamWriter writer = new StreamWriter(path))
            //        {

            //            if (File.Exists(path))
            //            {
            //                continue;
            //            }
            //            writer.Write(item.ContentMimeType);
            //        }
            //    }
            //    catch (UnauthorizedAccessException ex)
            //    {
            //        continue;
            //    }
            //}
            //filesNew = Directory.GetFiles(FileSystem.CacheDirectory);
            using (StreamWriter writer = new StreamWriter(Path.Combine(FileSystem.CacheDirectory, "style.css"), true, Encoding.Default))
            {
                writer.Write(GetHtmlSourceWithCustomCss(book));
            }

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
                if (counterLetters == 150)
                {

                    var htmlSource = new HtmlWebViewSource();
                    htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
                    temp = temp.Replace("<title/>", "");
                    htmlSource.Html = temp;
                    Label text = new Label()
                    {
                        FontSize = 18,
                        Text = htmlSource.Html,
                        TextType = TextType.Html,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Padding = 15,
                        TextColor = Color.Black,
                        FontFamily = "Georgia",
                    };
                    labelsPage.Add(text);
                    pages.Add(new ContentPage()
                    {
                        Content = new StackLayout()
                        {

                            Children = {text,

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


                    webViews.Add(new WebView()
                    {
                        Source = htmlSource,
                    });

                    counterLetters = 0;
                    temp = "";
                }
                counterLetters++;
            }


            foreach (ContentPage item in pages)
            {
                this.Children.Add(item);
            }

            //this.Children.Add(new ContentPage()
            //{
            //    Content = new ImageButton()
            //    {
            //        Source = image.Source
            //    }
            //});


            //foreach (WebView item in webViews)
            //{
            //    this.Children.Add(new ContentPage() { Content = item });
            //}


            //WebView all = new WebView();
            //UrlWebViewSource urlSource = new UrlWebViewSource();
            //urlSource.Url = "niggers.html";

            //all.Source = urlSource;
            //this.Children.Add(new ContentPage() { Content = all });

            //ContentPage page = new ContentPage();
            //StackLayout stackLayout = new StackLayout();
            //stackLayout.Children.Add(carouselView);
            //page.Content = stackLayout;
            //this.Children.Add(page);
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

        public void DecreaseTextSize(object sender, EventArgs e)
        {
      
                //this.Children.Add(pages[i]);
            
        }

        public void IncreaseTextSize(object sender, EventArgs e)
        {
            this.Children.Clear();
            foreach (Label item in labelsPage)
            {
                item.FontSize -= 2;
            }
        }
    }
}