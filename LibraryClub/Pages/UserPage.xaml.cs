using LibraryClub.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryClub.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private readonly LibraryClubEntities m_db = new LibraryClubEntities();
        private ObservableCollection<ProductInCart> cartItems;

        public UserPage()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProduct = Products.SelectedItem as Product;
            if (selectedProduct == null) return;

            var window = new ProductCardWindow(selectedProduct);
            window.Closing += Window_Closing;
            window.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateProductsInCartListBox();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateProductsListBox();
            UpdateProductsInCartListBox();
            UpdateOrdersDataGrid();
        }

        private void UpdateProductsListBox()
        {
            Products.ItemsSource = m_db.Products.ToList();
        }

        private void UpdateOrdersDataGrid()
        {
            Orders.ItemsSource = m_db.Orders
            .Where(order => order.UserId == Session.CurrentUser.Id)
            .ToList();
        }

        private void UpdateProductsInCartListBox()
        {
            var cart = Session.CurrentUser.Carts.FirstOrDefault();
            if (cart == null)
            {
                var userId = Session.CurrentUser.Id;
                cart = m_db.Carts.Add(new Cart() { UserId = userId });
            }
            cartItems = new ObservableCollection<ProductInCart>(m_db.CartItems
            .Where(item => item.CartId == cart.Id)
            .Select(e => new ProductInCart() { Product = e.Product, Cart = e.Cart, Quantity = e.Quantity }));
            ProductsInCart.ItemsSource = cartItems;
        }

        private void IncrementBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedProduct = ProductsInCart.SelectedItem as ProductInCart;
            if (selectedProduct == null) return;
            selectedProduct.Quantity += 1;
        }
        private void DecrementBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedProduct = ProductsInCart.SelectedItem as ProductInCart;
            if (selectedProduct == null) return;

            if (selectedProduct.Quantity == 1)
            {
                var result = MessageBox.Show("Удалить товар из корзины", "", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                cartItems.Remove(selectedProduct);

                var cartItem2 = m_db.CartItems
                .Where(item => item.CartId == selectedProduct.Cart.Id && item.ProductId == selectedProduct.Product.Id)
                .FirstOrDefault();

                m_db.CartItems.Remove(cartItem2);
                m_db.SaveChanges();
                return;
            }

            selectedProduct.Quantity -= 1;

            var cartItem = new CartItem()
            {
                CartId = selectedProduct.Cart.Id,
                ProductId = selectedProduct.Product.Id,
                Quantity = selectedProduct.Quantity
            };

            m_db.CartItems.AddOrUpdate(cartItem);
            m_db.SaveChanges();
        }


        private void ProductsInCart_Click(object sender, RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as FrameworkElement;
            object clicked = originalSource.DataContext;
            var lbi = ProductsInCart.ItemContainerGenerator.ContainerFromItem(clicked) as ListBoxItem;
            lbi.IsSelected = true;

            if (originalSource.Name.Equals("IncrementBtn"))
                IncrementBtn_Click(e.OriginalSource, e);

            if (originalSource.Name.Equals("DecrementBtn"))
                DecrementBtn_Click(e.OriginalSource, e);

        }

        public class ProductInCart : INotifyPropertyChanged
        {

            private int _quantity;
            public Cart Cart { get; set; }
            public Product Product { get; set; }
            public int Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged("TotalPrice");
                }
            }
            public double TotalPrice => Product.Price * Quantity;

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = Orders.SelectedItem as Order;
            if (selectedOrder == null) return;

            selectedOrder.StatusId = 4;

            m_db.Orders.AddOrUpdate(selectedOrder);
            m_db.SaveChanges();
            UpdateOrdersDataGrid();

        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems.Count < 1)
            {
                MessageBox.Show("Корзина пустая");
                return;
            }

            var orderPrice = 0d;
            foreach (var cartItem in cartItems)
            {
                orderPrice += cartItem.TotalPrice;
            }

            var order = m_db.Orders.Add(new Order()
            {
                StatusId = 1,
                Date = DateTime.Now,
                UserId = Session.CurrentUser.Id,
                Price = orderPrice
            });

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    ProductId = cartItem.Product.Id,
                    Quantity = cartItem.Quantity
                };

                var cartItem2 = m_db.CartItems
                .Where(item => item.CartId == cartItem.Cart.Id && item.ProductId == cartItem.Product.Id)
                .FirstOrDefault();

                m_db.CartItems.Remove(cartItem2);
                m_db.OrderItems.Add(orderItem);
            }



            m_db.SaveChanges();
            UpdateOrdersDataGrid();
            UpdateProductsInCartListBox();
        }
    }
}
