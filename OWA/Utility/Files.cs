using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using OWA.User;
using System.IO;
using System.Xml.Serialization;

namespace OWA.Utility
{
    public static class Files
    {
        private static string p_Location = AppDomain.CurrentDomain.BaseDirectory;

        public static bool SaveAccounts(List<Account> accounts)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Account>));
                    serializer.Serialize(stream, accounts);

                    File.WriteAllBytes(p_Location + "\\accounts.dat", stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static List<Account> LoadAccounts()
        {
            List<Account> accounts = null;

            if (!File.Exists(p_Location + "\\accounts.dat"))
                return null;

            using (Stream stream = File.OpenRead(p_Location + "\\accounts.dat"))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<Account>));

                accounts = deserializer.Deserialize(stream) as List<Account>;
            }

            return accounts;
        }
    }
}
