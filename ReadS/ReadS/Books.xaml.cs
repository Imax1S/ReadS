using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VersFx.Formats.Text.Epub;
using VersFx.Formats.Text.Epub.Schema.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Books : ContentPage
    {
        //public ObservableCollection<string> tems { get; set; }

        //Здесь хранятся книги и их названия
        static public Dictionary<string, EpubBook> books = new Dictionary<string, EpubBook>();

        List<Book> loadedBooks = new List<Book>();
        //List<Book2> loadedBooks = new List<Book2>();
        List<string> loadedBooksNames = new List<string>();
        List<Button> buttonsBook = new List<Button>();
        List<string> pathsToBooks = new List<string>();
        Grid library = new Grid()
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            RowDefinitions =
                {
                    //new RowDefinition { Height = GridLength.Auto },
                    //new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) }
                },
            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                   // new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
                },
            ColumnSpacing = 1,
            RowSpacing = 10
        };

        //Скролл для кнопок
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Vertical
        };

        //Инструменты
        ToolbarItem item = new ToolbarItem
        {
            Text = "Добавить книгу",
            // IconImageSource = ImageSource.FromFile("baseline_more_vert_black_48dp.png"),
            Order = ToolbarItemOrder.Secondary,
            Priority = 0
        };


        static string filenameForBooks = Path.Combine(FileSystem.AppDataDirectory, "books.txt");
        public Books()
        {
            InitializeComponent();

            try
            {
                using (StreamReader sr = new StreamReader(filenameForBooks, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        pathsToBooks.Add(line);
                    }
                }
                foreach (string item in pathsToBooks)
                {
                    string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), item);

                    EpubBook newBook = EpubReader.ReadBook(new MemoryStream(File.ReadAllBytes(item)));
                    books.Add(newBook.Title + Environment.NewLine + newBook.Author, newBook);
                    LoadNewBook(newBook);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No file");
            }


            this.ToolbarItems.Add(item);
            item.Clicked += LoadButtonClicked;


            Label noBook = new Label()
            {
                Text = "Нажмите на три точки, чтобы добавить книгу",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 16,
                Padding = 10
            };

            this.Padding = new Thickness(0, Device.OnPlatform(20, 20, 0), 10, 0);
            if (books.Count == 0)
            {
                Content = new StackLayout()
                {
                    Children = { noBook }
                };
            }

            #region How to load a text file embedded resource


            //var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Books)).Assembly;
            //Stream stream = assembly.GetManifestResourceStream("ReadS.homo_deus.epub");
            //using (var reader = new StreamReader(stream))
            //{
            //    EpubBook book = EpubReader.ReadBook(stream);
            //    books.Add(book.Title, book);
            //}
            #endregion
        }

        private async void Book_Clicked(object sender, EventArgs e)
        {
            //if (loadedBooksNames.Contains((sender as Button).Text))
            //{
            //    await Navigation.PushAsync(loadedBooks[loadedBooksNames.IndexOf((sender as Button).Text)]);
            //}
            //else
            //{
            //    loadedBooksNames.Add((sender as Button).Text);
            //    loadedBooks.Add(new Book2(books[(sender as Button).Text]));
            //    await Navigation.PushAsync(loadedBooks[loadedBooks.Count - 1]);
            //}

            if (loadedBooksNames.Contains((sender as Button).Text))
            {
                await Navigation.PushAsync(loadedBooks[loadedBooksNames.IndexOf((sender as Button).Text)]);
            }
            else
            {
                loadedBooksNames.Add((sender as Button).Text);
                loadedBooks.Add(new Book(books[(sender as Button).Text]));
                await Navigation.PushAsync(loadedBooks[loadedBooks.Count - 1]);
            }
        }
        /// <summary>
        /// Чтение книги с устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void LoadButtonClicked(object sender, EventArgs e)
        {
            try
            {
                //Допустимые форматы
                string[] types = new string[] { ".epub" };

                //Открываем проводник
                FileData fileData = await CrossFilePicker.Current.PickFile(allowedTypes: types);

                //Если ничего не выбрано, то просто возвращаем  null
                if (fileData == null)
                    return; 
                
                //Создаем объект EpubBook
                EpubBook newBook = EpubReader.ReadBook(new MemoryStream(fileData.DataArray));

                //Добавляем в словарь
                books.Add(newBook.Title + Environment.NewLine + newBook.Author, newBook);

                //Берем путь и сохраняем его для того, чтобы после закрытия программы все книги отобразились вновь
                string path = fileData.FilePath;
                using (StreamWriter sw = new StreamWriter(filenameForBooks, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(path);
                }

                foreach (string book in Books.books.Keys)
                {
                    Goal.bookPicker.Items.Add(book);
                }
                //Создаём кнопку с книгой
                LoadNewBook(newBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выборе файла: " + ex.ToString());
            }
        }

        private void LoadNewBook(EpubBook book)
        {

            Button book_button = new Button()
            {
                // ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left , 10),
                WidthRequest = 100,
                HeightRequest = 100,
                BackgroundColor = Color.WhiteSmoke
            };
            book_button.Text = book.Title + Environment.NewLine + book.Author;
            book_button.Clicked += Book_Clicked;
            //book_button.ImageSource = ImageSource.FromStream(() => new MemoryStream(book.CoverImage));

            buttonsBook.Add(book_button);
            ImageButton image = new ImageButton();
            image.Source = ImageSource.FromStream(() => new MemoryStream(book.CoverImage));
            image.BackgroundColor = Color.FromHex("#FAFAFA");
            image.CornerRadius = 90;


            library.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) });
            library.Children.Add(buttonsBook[buttonsBook.Count - 1], 1, buttonsBook.Count - 1);

            library.Children.Add(image, 0, buttonsBook.Count - 1);

            scroll.Content = library;
            Content = new StackLayout()
            {
                Children = { scroll }
            };
        }
    }
}