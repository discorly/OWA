using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using OWA.Events;
using OWA.User;
using OWA.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Timers;

namespace OWA.Web
{
    /// <summary>
    /// Utilizes a client to update properties of an account
    /// </summary>
    public static class Status
    {
        /// <summary>
        /// A list of accounts to check
        /// </summary>
        public static ObservableCollection<Account> Accounts = new ObservableCollection<Account>();

        public static event EventHandler<InputRequiredEventArgs> InputRequired;
        public static event EventHandler<WebErrorEventArgs> WebError;
        public static event EventHandler<ExceptionEventArgs> ExceptionError;
        public static event EventHandler<BetaFoundEventArgs> BetaFound;

        private static string p_Login_URL_US = "https://us.battle.net/login/en/";
        private static string p_Login_URL_EU = "https://eu.battle.net/login/en/";
        private static string p_Account_URL_US = "https://us.battle.net/account/management/";
        private static string p_Account_URL_EU = "https://eu.battle.net/account/management/";
        private static string p_Authenticator_URL_US = "https://us.battle.net/login/en/authenticator";
        private static string p_Authenticator_URL_EU = "https://eu.battle.net/login/en/authenticator";

        private static Timer p_NextUpdate = new Timer();

        /// <summary>
        /// Sets up everything. Always call the first time you use Status.
        /// </summary>
        public static void Setup()
        {
            if (Accounts == null)
                Accounts = new ObservableCollection<Account>();

            if (p_NextUpdate == null)
                p_NextUpdate = new Timer();

            p_NextUpdate.Elapsed += p_NextUpdate_Elapsed;
        }

        /// <summary>
        /// Adds the specified account to the list and immediately updates it
        /// </summary>
        /// <param name="account"></param>
        public static async void AddAccount(Account account)
        {
            if (Accounts == null)
                Accounts = new ObservableCollection<Account>();

            Accounts.Add(account);

            bool result = await UpdateAccount(account);

            //* schedule next update
            if (p_NextUpdate.Enabled)
                return;

            p_NextUpdate.Interval = 5 * 60 * 1000;
            p_NextUpdate.Start();
        }

        public static async void AuthenticateAccount(Account account, string html, string code)
        {
            //* get csrf token
            LoginData data = GetLoginData(html, true);

            //* build our values
            NameValueCollection collection = CreateAuthenticationCollection(account, data, code);

            string response = string.Empty;

            try { response = await account.Client.PostData(GetRegionAuthentication(account.Region), collection); }
            catch (Exception ex)
            {
                CreateException(ex, account);

                return;
            }

            collection.Clear();
            collection = null;

            LoginResult result = GetLoginResult(response);

            if (result == LoginResult.Success)
            {
                await GetAccountGames(account, response);

                account.LastUpdated = DateTime.Now;
                account.IsUpdating = false;

                return;
            }

            CreateWebError(result, account);
        }

        private static void NotifyInputRequired(InputRequiredEventArgs e)
        {
            if (InputRequired != null)
                InputRequired(null, e);
        }

        private static void NotifyWebErrorOccurred(WebErrorEventArgs e)
        {
            if (WebError != null)
                WebError(null, e);
        }

        private static void NotifyExceptionOccurred(ExceptionEventArgs e)
        {
            if (ExceptionError != null)
                ExceptionError(null, e);
        }

        private static void NotifyBetaFound(BetaFoundEventArgs e)
        {
            if (BetaFound != null)
                BetaFound(null, e);
        }

        private static async void p_NextUpdate_Elapsed(object sender, ElapsedEventArgs e)
        {
            p_NextUpdate.Stop();

            List<Account> accounts = GetAccountsToUpdate();

            for (int a = 0; a < accounts.Count; a++)
                await UpdateAccount(accounts[a]);

            double next = GetNextUpdateInterval();

            p_NextUpdate.Interval = next;
            p_NextUpdate.Start();
        }

        private static double GetNextUpdateInterval()
        {
            DateTime now = DateTime.Now;
            TimeSpan longest = TimeSpan.Zero;

            for (int a = 0; a < Accounts.Count; a++)
            {
                TimeSpan time = now - Accounts[a].LastUpdated;

                if (time > longest)
                    longest = time;
            }

            double diff = (5 * 60 * 1000) - longest.TotalMilliseconds;

            if (diff < 100)
                diff = 100;

            return diff;
        }

        private static List<Account> GetAccountsToUpdate()
        {
            List<Account> accounts = new List<Account>();
            DateTime now = DateTime.Now;

            for (int a = 0; a < Accounts.Count; a++)
            {
                Account account = Accounts[a];

                TimeSpan time = now - account.LastUpdated;

                if (time.Minutes >= 5)
                    accounts.Add(account);
            }

            return accounts;
        }

        private static async Task<bool> UpdateAccount(Account account)
        {
            if (!AccountOK(account))
                return false;

            string html = string.Empty, url, response = string.Empty;
            NameValueCollection post;
            Pages page;
            LoginResult result;

            //* flag we are updating
            account.IsUpdating = true;

            //* lets try to log in
            url = GetRegionURL(account.Region, true);

            if (url == null)
            {
                account.IsUpdating = false;

                CreateWebError(LoginResult.InvalidRegion, account);

                return false;
            }

            try { html = await account.Client.GetHTML(url); }
            catch (Exception ex)
            {
                CreateException(ex, account);

                return false;
            }

            //* we wanted to go to the login page, if it redirected us, we are probably already logged in
            page = GetPage(html);

            if (page == Pages.AccountManagement || page == Pages.Root)
            {
                //* navigate to account management and read available games

                try { await GetAccountGames(account, html); }
                catch (Exception ex)
                {
                    CreateException(ex, account);

                    return false;
                }

                account.LastUpdated = DateTime.Now;
                account.IsUpdating = false;

                return true;
            }

            //* we got a page we didn't expect; for now just return false
            if (page != Pages.Login)
                return false;

            post = CreateLoginCollection(account, GetLoginData(html, false));

            //* now log in
            try { response = await account.Client.PostData(url, post); }
            catch (Exception ex)
            {
                CreateException(ex, account);

                return false;
            }

            post.Clear();
            post = null;

            result = GetLoginResult(response);

            if (result == LoginResult.Success)
            {
                //* navigate to account management and read available games
                try { await GetAccountGames(account, response); }
                catch (Exception ex)
                {
                    CreateException(ex, account);

                    return false;
                }

                account.LastUpdated = DateTime.Now;
                account.IsUpdating = false;

                return true;
            }
            else if (result == LoginResult.AuthenticationRequired)
            {
                //* everything went fine BUT we need to authenticate (either authenticator or sms)
                InputRequiredEventArgs e = new InputRequiredEventArgs();

                e.Account = account;
                e.HTML = response;
                e.Type = GetAuthenticationType(response);

                NotifyInputRequired(e);

                return true;
            }
            else
            {
                CreateWebError(result, account);

                return false;
            }
        }

        private static void CreateException(Exception ex, Account account)
        {
            ExceptionEventArgs e = new ExceptionEventArgs();

            e.Exception = ex;
            e.Account = account;

            NotifyExceptionOccurred(e);
        }

        private static void CreateWebError(LoginResult result, Account account)
        {
            WebErrorEventArgs e = new WebErrorEventArgs();

            e.Reason = result;
            e.Account = account;

            NotifyWebErrorOccurred(e);
        }

        private static AuthenticationType GetAuthenticationType(string html)
        {
            int index = html.IndexOf("<option value=\"authenticator\"");

            string authenticator = html.Substring(index, html.IndexOf("</option>", index) - index);

            if (authenticator.Contains("selected"))
                return AuthenticationType.Authenticator;
            else
                return AuthenticationType.SMS;
        }

        private static async Task<bool> GetAccountGames(Account account, string html)
        {
            if (!html.Contains("<title>Battle.net Account</title>"))
                html = await account.Client.GetHTML(GetRegionURL(account.Region, false));
                
            html = html.Remove(0, html.IndexOf("Your Game Accounts"));
            html = html.Remove(html.IndexOf("Add a Game Key"));

            if (html.Contains("Overwatch® Beta"))
            {
                account.HasOverwatchBeta = true;

                BetaFoundEventArgs e = new BetaFoundEventArgs();
                e.Account = account;

                NotifyBetaFound(e);
            }

            return true;
        }

        private static LoginResult GetLoginResult(string html)
        {
            Pages page = GetPage(html);

            //* if we aren't on the login page, what happened?
            if (page != Pages.Login)
            {
                //* if we got t
                if (page == Pages.AccountManagement || page == Pages.Root)
                    return LoginResult.Success;

                return LoginResult.Unknown;
            }

            if (html.Contains("The username or password is incorrect. Please try again."))
                return LoginResult.InvalidUsernameOrPassword;
            else if (html.Contains("captcha"))
                return LoginResult.Captcha;
            else if (html.Contains("authenticator"))
                return LoginResult.AuthenticationRequired;

            return LoginResult.None;
        }

        private static LoginData GetLoginData(string html, bool authentication)
        {
            LoginData data = new LoginData();

            int index = html.IndexOf("csrftoken");
            int value = html.IndexOf("value", index) + 7;
            int end = html.IndexOf("\" />", value);

            data.CSRFToken = html.Substring(value, end - value);

            if (authentication)
                return data;

            index = html.IndexOf("sessionTimeout");
            value = html.IndexOf("value", index) + 7;
            end = html.IndexOf("\" />", value);

            data.SessionTimeout = long.Parse(html.Substring(value, end - value));

            return data;
        }

        private static NameValueCollection CreateLoginCollection(Account account, LoginData data)
        {
            NameValueCollection collection = new NameValueCollection();

            collection.Add("accountName", account.Username);
            collection.Add("password", account.Password);
            collection.Add("useSrp", "false");
            collection.Add("publicA", string.Empty);
            collection.Add("clientEvidenceM1", string.Empty);
            collection.Add("persistLogin", "false");
            collection.Add("submit", "submit");
            collection.Add("csrftoken", data.CSRFToken);
            collection.Add("sessionTimeout", data.SessionTimeout.ToString());

            return collection;
        }

        private static NameValueCollection CreateAuthenticationCollection(Account account, LoginData data, string code)
        {
            NameValueCollection collection = new NameValueCollection();

            collection.Add("authValue", code);
            collection.Add("persistAuthenticator", "true");
            collection.Add("csrftoken", data.CSRFToken);

            return collection;
        }

        private static Pages GetPage(string html)
        {
            /*
            root =                  <title>Battle.net</title>
            login =                 <title>Battle.net Account Login</title>
            account management =    <title>Battle.net Account</title>
            */

            if (html.Contains("<title>Battle.net</title>"))
                return Pages.Root;
            else if (html.Contains("<title>Battle.net Account Login</title>"))
                return Pages.Login;
            else if (html.Contains("<title>Battle.net Account</title>"))
                return Pages.AccountManagement;

            return Pages.Unknown;
        }

        private static bool AccountOK(Account account)
        {
            if (account == null)
                return false;

            if (account.Username == null || account.Password == null || account.Username == string.Empty || account.Password == string.Empty)
                return false;

            if (account.Client == null)
                account.Client = new Client();

            if (account.Region == Regions.None)
                account.Region = Regions.US;

            return true;
        }

        private static string GetRegionURL(Regions region, bool login)
        {
            if (region == Regions.US)
                return (login) ? p_Login_URL_US : p_Account_URL_US;
            else if (region == Regions.EU)
                return (login) ? p_Login_URL_EU : p_Account_URL_EU;

            return null;
        }

        private static string GetRegionAuthentication(Regions region)
        {
            if (region == Regions.US)
                return p_Authenticator_URL_US;
            else
                return p_Authenticator_URL_EU;
        }
    }
}
