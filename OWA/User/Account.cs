using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using OWA.Utility;
using OWA.Web;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;

namespace OWA.User
{
    /// <summary>
    /// A class representing a user account
    /// </summary>
    public class Account : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region PROPERTIES
        /// <summary>
        /// Gets or sets the username of the account
        /// </summary>
        public string Username
        {
            get { return p_Username; }
            set
            {
                if (p_Username != value)
                {
                    p_Username = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password of the account
        /// </summary>
        public string Password
        {
            get { return Cryptography.DecryptStringFromBytes_Aes(p_Password, p_Key, p_IV); }
            set
            {
                if (p_Key == null || p_IV == null)
                {
                    p_Key = Cryptography.GenerateKey();
                    p_IV = Cryptography.GenerateIV();
                }

                p_Password = Cryptography.EncryptStringToBytes_Aes(value, p_Key, p_IV);
            }
        }

        /// <summary>
        /// Gets or sets the region of the account
        /// </summary>
        public Regions Region { get; set; }

        /// <summary>
        /// Gets or sets when this account was last updated
        /// </summary>
        public DateTime LastUpdated
        {
            get { return p_LastUpdated; }
            set
            {
                if (p_LastUpdated != value)
                {
                    p_LastUpdated = value;
                    PushNotifications();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this account is being updated
        /// </summary>
        public bool IsUpdating
        {
            get { return p_IsUpdating; }
            set
            {
                if (p_IsUpdating != value)
                {
                    p_IsUpdating = value;
                    PushNotifications();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this account has beta access
        /// </summary>
        public bool HasOverwatchBeta
        {
            get { return p_HasOverwatchBeta; }
            set
            {
                if (p_HasOverwatchBeta != value)
                {
                    p_HasOverwatchBeta = value;
                    PushNotifications();
                }
            }
        }

        public byte[] Key
        {
            get { return p_Key; }
        }

        public byte[] IV
        {
            get { return p_IV; }
        }

        /// <summary>
        /// Gets or sets the web client for this account
        /// </summary>
        public Client Client { get; set; }
        #endregion

        #region PRIVATE VARIABLES
        private string p_Username = null;
        private DateTime p_LastUpdated = DateTime.Now;
        private bool p_IsUpdating = false;
        private bool p_HasOverwatchBeta = false;

        private byte[] p_Password = null;
        private byte[] p_Key = null;
        private byte[] p_IV = null;
        #endregion

        #region UI SPECIFIC PROPERTIES
        /// <summary>
        /// UI specific property that updates the listview with how long it has been since the last update - GET returns how long, SET throws the property changed event
        /// </summary>
        public string UILastUpdated
        {
            get
            {
                TimeSpan span = DateTime.Now - p_LastUpdated;

                int time = (span.Minutes > 0) ? span.Minutes : span.Seconds;
                string msg = string.Format("Last checked {0} {1}{2} ago", time, (span.Minutes > 0) ? "minute" : "second", (time != 1) ? "s" : string.Empty);

                return msg;
            }
            set { NotifyPropertyChanged(); }
        }

        /// <summary>
        /// UI specific property that updates the listvie with the status of the account - GET returns status, SET throws the property changed event
        /// </summary>
        public string UIStatus
        {
            get
            {
                if (p_IsUpdating)
                    return "Updating...";

                return (p_HasOverwatchBeta) ? ("Overwatch found!").ToUpper() : ("Overwatch NOT FOUND").ToUpper();
            }
            set { NotifyPropertyChanged(); }
        }

        /// <summary>
        /// UI specific property that updates the listview with the opacity of the overwatch image - GET returns the opacity, SET throws the property changed event
        /// </summary>
        public double UIOpacity
        {
            get { return (p_HasOverwatchBeta) ? 1 : 0.25; }
            set { NotifyPropertyChanged(); }
        }

        /// <summary>
        /// UI specific property that updates the listview with the visibility of the checkmark image - GET returns the visibility, SET throws the property changed event
        /// </summary>
        public Visibility UIVisibility
        {
            get { return (p_HasOverwatchBeta) ? Visibility.Visible : Visibility.Hidden; }
            set { NotifyPropertyChanged(); }
        }
        #endregion

        #region CONSTRUCTORS
        public Account()
        {

        }

        public Account(string username, string password, Regions region)
        {
            Username = username;

            //* encrypt password
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();

                p_Key = aes.Key;
                p_IV = aes.IV;
            }

            p_Password = Cryptography.EncryptStringToBytes_Aes(password, p_Key, p_IV);

            Region = region;
        }

        public Account(string username, byte[] password, Regions region, byte[] key, byte[] iv)
        {
            Username = username;
            p_Password = password;
            Region = region;
            p_Key = key;
            p_IV = iv;
        }
        #endregion

        #region EVENT SPECIFIC METHODS
        private void PushNotifications()
        {
            UIStatus = "";
            UILastUpdated = "";
            UIOpacity = 0;
            UIVisibility = Visibility.Visible;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
