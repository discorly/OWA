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
using System.Windows.Navigation;
using System.Windows.Shapes;

//* non-default
using OWA.User;
using OWA.Utility;
using OWA.Web;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace OWA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //* window size adjustment values;
        private int p_Height_Item = 76;
        private int p_Height_Button = 28;
        private int p_Height_Border = 10;
        private int p_Height_Spacing = 5;
        private int p_Height_Adjustment = 39;

        private NotifyIcon p_Notification = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            p_Notification = new NotifyIcon();
            p_Notification.Click += p_Notification_Click;
            p_Notification.DoubleClick += p_Notification_DoubleClick;

            try { p_Notification.Icon = new Icon("ow.ico"); }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading icon, make sure it is in the same directory as the exe.");
            }

            p_Notification.Visible = true;

            p_Notification.BalloonTipTitle = "Overwatch Beta FOUND!";
            p_Notification.BalloonTipText = "Account blah@blah.com has access to the Overwatch beta!";

            //p_Notification.ShowBalloonTip(5);

            Status.Setup();
            Status.Accounts.CollectionChanged += Accounts_CollectionChanged;
            Status.InputRequired += Status_InputRequired;
            Status.WebError += Status_WebError;
            Status.ExceptionError += Status_ExceptionError;
            Status.BetaFound += Status_BetaFound;

            lvAccounts.ItemsSource = Status.Accounts;

            //* load accounts
            List<Account> accounts = Files.LoadAccounts();

            if (accounts == null)
                return;

            if (accounts.Count <= 0)
                return;

            foreach (Account account in accounts)
                Status.AddAccount(new Account(account.Username, account.Password, account.Region));
        }

        private void Status_BetaFound(object sender, Events.BetaFoundEventArgs e)
        {
            p_Notification.BalloonTipText = "The Overwatch beta was found on your account, " + e.Account.Username + "! Congratulations!";
            p_Notification.ShowBalloonTip(10000);
        }

        private void p_Notification_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        private void p_Notification_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Status_ExceptionError(object sender, Events.ExceptionEventArgs e)
        {
            //* an exception occurred while updating an account, alert the user
            System.Windows.MessageBox.Show("An exception occurred while trying to update an account.\n\nAccount: " + e.Account.Username + "\n\nException message: " + e.Exception.Message, "Exception occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Status_WebError(object sender, Events.WebErrorEventArgs e)
        {
            string reason = "An unknown error was returned by battle.net for account " + e.Account.Username;

            if (e.Reason == Utility.LoginResult.InvalidRegion)
                reason = "The region for the account is invalid! Somehow an invalid region was selected. Remove the account and try again.";
            else if (e.Reason == Utility.LoginResult.InvalidUsernameOrPassword)
                reason = "The username or password was invalid! Please remove the account and re-add it to try again.\n\nWarning: too many failed attempts could cause a captcha warning.";
            else if (e.Reason == Utility.LoginResult.Captcha)
                reason = "A captcha input is now required due to too many failed input attempts. Please manually log in with your web browser and then wait at least an hour.";

            System.Windows.MessageBox.Show("Battle.net returned an error.\n\nAccount: " + e.Account.Username + "\n\nReason: " + reason, "Battle.net Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Status_InputRequired(object sender, Events.InputRequiredEventArgs e)
        {
            Authentication dialog = new Authentication();

            dialog.Owner = this;
            dialog.Account = e.Account.Username;

            bool? result = dialog.ShowDialog();

            if (!result.HasValue)
                return;

            if (!result.Value)
                return;

            string code = dialog.Code;

            dialog = null;

            //* we have our code, we need to submit it
            Status.AuthenticateAccount(e.Account, e.HTML, code);
        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lvAccounts.Height = (p_Height_Item * Status.Accounts.Count) + 3 - (Status.Accounts.Count * 1.5);
            Height = lvAccounts.Height + p_Height_Spacing + p_Height_Button + p_Height_Border + p_Height_Adjustment;

            lvAccounts.Items.Refresh();
        }

        private void bAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccount dialog = new AddAccount();
            dialog.Owner = this;

            bool? result = dialog.ShowDialog();

            if (!result.HasValue)
                return;

            if (!result.Value)
                return;

            Account account = new Account(dialog.Username, dialog.Password, dialog.Region, dialog.Key, dialog.IV);

            dialog = null;

            Status.AddAccount(account);

            Files.SaveAccounts(Status.Accounts.ToList());
        }

        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Account account = (sender as FrameworkElement).DataContext as Account;
            account.UILastUpdated = "";
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                ShowInTaskbar = false;
            else
                ShowInTaskbar = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            p_Notification.Visible = false;
        }

        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAccounts.SelectedItems.Count <= 0)
                bRemoveAccount.IsEnabled = false;
            else
                bRemoveAccount.IsEnabled = true;
        }

        private void bRemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            //* copy all of the accounts that are selected
            List<Account> accounts = new List<Account>();

            //* if i try to remove from within selecteditems bad things can happen (like index out of range)
            for (int a = 0; a < lvAccounts.SelectedItems.Count; a++)
                accounts.Add(lvAccounts.SelectedItems[a] as Account);

            for (int b = 0; b < accounts.Count; b++)
                Status.Accounts.Remove(accounts[b]);

            accounts.Clear();
        }
    }
}
