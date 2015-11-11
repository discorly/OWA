using System;
//* using System.Collections.Generic;
//* using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace OWA.Web
{
    /// <summary>
    /// A wrapper class for accessing websites and posting data
    /// </summary>
    public class Client
    {
        private CookieContainer p_Cookies = null;

        /// <summary>
        /// Creates a new web client for accessing web pages
        /// </summary>
        public Client()
        {
            p_Cookies = new CookieContainer();
        }

        /// <summary>
        /// Gets the HTML of the specified URL
        /// </summary>
        /// <param name="url">URL of the website to access</param>
        /// <returns>The HTML of the website</returns>
        public async Task<string> GetHTML(string url)
        {
            string html = null;

            if (this.p_Cookies == null)
                this.p_Cookies = new CookieContainer();

            using (CookieAwareWebClient client = new CookieAwareWebClient(this.p_Cookies))
            {
                client.Method = "GET";

                using (Stream stream = await client.OpenReadTaskAsync(url))
                {
                    using (StreamReader reader = new StreamReader(stream))
                        html = await reader.ReadToEndAsync();
                }
            }

            return html;
        }

        /// <summary>
        /// Submits data to a website via POST
        /// </summary>
        /// <param name="url">The URL to POST to</param>
        /// <param name="collection">The data to be POSTed</param>
        /// <returns></returns>
        public async Task<string> PostData(string url, NameValueCollection collection)
        {
            if (this.p_Cookies == null)
                this.p_Cookies = new CookieContainer();

            byte[] response = null;

            using (CookieAwareWebClient client = new CookieAwareWebClient(this.p_Cookies))
            {
                client.Method = "POST";

                response = await client.UploadValuesTaskAsync(url, collection);
            }

            return Encoding.UTF8.GetString(response);
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        /// <summary>
        /// Gets or sets the method of interaction with the website (GET or POST)
        /// </summary>
        public string Method;

        /// <summary>
        /// Gets or sets the container for housing cookies
        /// </summary>
        public CookieContainer CookieContainer { get; set; }
        
        /// <summary>
        /// Gets or sets the URI of the website
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Creates a new CookieAwareWebClient with a new cookie container
        /// </summary>
        public CookieAwareWebClient() : this(new CookieContainer())
        {
        }

        /// <summary>
        /// Creates a new CookieAwareWebClient with the specified cookie container
        /// </summary>
        /// <param name="cookies">Cookie container to be used</param>
        public CookieAwareWebClient(CookieContainer cookies)
        {
            CookieContainer = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
                (request as HttpWebRequest).ServicePoint.Expect100Continue = false;
                (request as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
                (request as HttpWebRequest).Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                (request as HttpWebRequest).Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                (request as HttpWebRequest).Referer = "http://us.battle.net";
                (request as HttpWebRequest).KeepAlive = true;
                (request as HttpWebRequest).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (Method == "POST")
                    (request as HttpWebRequest).ContentType = "application/x-www-form-urlencoded";

            }

            HttpWebRequest httpRequest = (HttpWebRequest)request;

            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            string setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            if (setCookieHeader != null)
            {
                //do something if needed to parse out the cookie.
                try
                {
                    if (setCookieHeader != null)
                    {
                        Cookie cookie = new Cookie(); //create cookie
                        CookieContainer.Add(cookie);
                    }
                }
                catch (Exception)
                {

                }
            }

            return response;
        }
    }
}
