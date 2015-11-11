using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using OWA.User;
using OWA.Utility;

namespace OWA.Events
{
    public class InputRequiredEventArgs : EventArgs
    {
        public Account Account { get; set; }
        public string HTML { get; set; }
        public AuthenticationType Type { get; set; }
    }

    public class WebErrorEventArgs : EventArgs
    {
        public Account Account { get; set; }
        public LoginResult Reason { get; set; }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public Account Account { get; set; }
    }

    public class BetaFoundEventArgs : EventArgs
    {
        public Account Account { get; set; }
    }
}
