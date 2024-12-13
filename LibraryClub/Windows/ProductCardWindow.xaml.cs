using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LibraryClub.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProductCardWindow.xaml
    /// </summary>
    public partial class ProductCardWindow : Window
    {
        private readonly LibraryClubEntities db = new LibraryClubEntities();
        private readonly Product currentProduct;

        public ProductCardWindow(Product product)
        {
            InitializeComponent();

            currentProduct = product;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Name.Text = currentProduct.Name;
            Manafacturer.Text = currentProduct.Manufacturer.Name;
            Description.Text = currentProduct.Description;
            Price.Text = $"{currentProduct.Price}";
            Preview.Source = new BitmapImage(new Uri(@"/Images/" + currentProduct.Image, UriKind.Relative));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddToCart();
        }

        private void AddToCart()
        {
            var cart = Session.CurrentUser.Carts.FirstOrDefault();
            if (cart == null)
            {
                var userId = Session.CurrentUser.Id;
                cart = db.Carts.Add(new Cart() { UserId = userId });
            }

            var cartItem = db.CartItems
                .Where(x => x.CartId == cart.Id && x.ProductId == currentProduct.Id)
                .FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                db.CartItems.AddOrUpdate(cartItem);
                db.SaveChanges();
                return;
            }

            cartItem = new CartItem()
            {
                ProductId = currentProduct.Id,
                Quantity = 1,
                CartId = cart.Id,
            };

            db.CartItems.Add(cartItem);
            db.SaveChanges();

        }
    }
}
