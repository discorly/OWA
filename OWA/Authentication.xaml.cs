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

namespace OWA
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        /// <summary>
        /// Gets the code entered in by the user
        /// </summary>
        public string Code
        {
            get { return UICode.Text; }
        }

        /// <summary>
        /// Gets or sets the account
        /// </summary>
        public string Account
        {
            get { return UIAccount.Text; }
            set { UIAccount.Text = value; }
        }

        public Authentication()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UICode.Focus();
        }

        private void UISubmit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void UICancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
