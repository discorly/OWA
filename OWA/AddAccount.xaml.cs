using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//* non-default
using OWA.Utility;
using System.Security.Cryptography;

namespace OWA
{
    /// <summary>
    /// Interaction logic for AddAccount.xaml
    /// </summary>
    public partial class AddAccount : Window
    {
        /// <summary>
        /// Gets the account name entered in by the user
        /// </summary>
        public string Username
        {
            get { return uiAccountName.Text; }
        }

        /// <summary>
        /// Gets the account password entered in by the user
        /// </summary>
        public byte[] Password
        {
            get { return Cryptography.EncryptStringToBytes_Aes(uiPassword.Password, Key, IV); }
        }

        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        /// <summary>
        /// Gets the region selected by the user
        /// </summary>
        public Regions Region
        {
            get { return (Regions)Enum.Parse(typeof(Regions), uiRegion.Text); }
        }

        public AddAccount()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();

                Key = aes.Key;
                IV = aes.IV;
            }

            uiAccountName.Focus();
        }

        private void bAddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (uiAccountName.Text == null || uiAccountName.Text == string.Empty)
            {
                MessageBox.Show("The username cannot be empty!", "Invalid username input", MessageBoxButton.OK, MessageBoxImage.Error);
                uiAccountName.Focus();
                return;
            }

            if (uiPassword.Password == null || uiPassword.Password == string.Empty)
            {
                MessageBox.Show("The password cannot be empty!", "Invalid password input", MessageBoxButton.OK, MessageBoxImage.Error);
                uiPassword.Focus();
                return;
            }

            DialogResult = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
