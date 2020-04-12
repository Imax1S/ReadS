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
        List<Book> loadedBooks = new List<Book>();
        List<string> loadedBooksNames = new List<string>();

        public Books()
        {
            InitializeComponent();

            Grid library = new Grid();

            #region How to load a text file embedded resource
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Books)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("ReadS.homo_deus.epub");
            using (var reader = new StreamReader(stream))
            {
                EpubBook book = EpubReader.ReadBook(stream);
                books.Add(book.Title, book);
            }
            #endregion

            foreach (string book in books.Keys)
            {
                Button book_button = new Button();
                book_button.Text = book;
                book_button.Clicked += Book_Clicked;

                library.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                library.Children.Add(book_button);
            }

            ScrollView scroll = new ScrollView()
            {
                Orientation = ScrollOrientation.Vertical
            };

            scroll.Content = library;
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
                loadedBooks.Add(new Book(books[(sender as Button).Text]));
                await Navigation.PushAsync(loadedBooks[loadedBooks.Count - 1]);
            }
        }
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            ContentPage page = new ContentPage();

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}