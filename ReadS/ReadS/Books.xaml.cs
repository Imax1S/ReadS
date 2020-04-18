using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VersFx.Formats.Text.Epub;
using VersFx.Formats.Text.Epub.Schema.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReadS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Books : ContentPage
    {
        public ObservableCollection<string> tems { get; set; }
        public Label label1;
        Dictionary<string, EpubBook> books = new Dictionary<string, EpubBook>();
        List<Book2> loadedBooks = new List<Book2>();
        List<string> loadedBooksNames = new List<string>();
        List<Button> buttonsBook = new List<Button>();
        //Button load = new Button();
        Grid library = new Grid()
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) }
                },
            ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
                }
        };
        ScrollView scroll = new ScrollView()
        {
            Orientation = ScrollOrientation.Vertical
        };
        ToolbarItem item = new ToolbarItem
        {
            Text = "Добавить книгу",
           // IconImageSource = ImageSource.FromFile("baseline_more_vert_black_48dp.png"),
            Order = ToolbarItemOrder.Secondary,
            Priority = 0
        };

        public Books()
        {
            InitializeComponent();
             
            this.ToolbarItems.Add(item);
            item.Clicked += LoadButtonClicked;
            #region How to load a text file embedded resource


            //var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Books)).Assembly;
            //Stream stream = assembly.GetManifestResourceStream("ReadS.homo_deus.epub");
            //using (var reader = new StreamReader(stream))
            //{
            //    EpubBook book = EpubReader.ReadBook(stream);
            //    books.Add(book.Title, book);
            //}
            #endregion
            //load.Text = "Загрузить книгу";
            //load.Clicked += LoadButtonClicked;

            //foreach (string book in books.Keys)
            //{
            //    Button book_button = new Button();
            //    book_button.Text = book;
            //    book_button.Clicked += Book_Clicked;

            //    library.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            //    library.Children.Add(book_button);
            //}

            //scroll.Content = library;
            Content = new StackLayout()
            {
                Children = { scroll }
            };
        }

        private async void Book_Clicked(object sender, EventArgs e)
        {
            if (loadedBooksNames.Contains((sender as Button).Text))
            {
                await Navigation.PushAsync(loadedBooks[loadedBooksNames.IndexOf((sender as Button).Text)]);
            }
            else
            {
                loadedBooksNames.Add((sender as Button).Text);
                loadedBooks.Add(new Book2(books[(sender as Button).Text]));
                await Navigation.PushAsync(loadedBooks[loadedBooks.Count - 1]);
            }
        }
        async void LoadButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string[] types = new string[] { ".epub" };
                FileData fileData = await CrossFilePicker.Current.PickFile(allowedTypes: types);
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                Console.WriteLine("File name chosen: " + fileName);
                Console.WriteLine("File data: " + contents);
                EpubBook newBook = EpubReader.ReadBook(new MemoryStream(fileData.DataArray));
                books.Add(newBook.Title, newBook);
                LoadNewBook(newBook);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        public void LoadNewBook(EpubBook book)
        {
            Button book_button = new Button();
            book_button.Text = book.Title;
            book_button.Clicked += Book_Clicked;

            //library.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            //library.Children.Add(book_button);
            buttonsBook.Add(book_button);
            for (int i = 0; i < buttonsBook.Count; i++)
            {
                library.Children.Add(buttonsBook[i], 0, i);
            }
            scroll.Content = library;
            Content = new StackLayout()
            {
                Children = { scroll }
            };
        }

    }
}