using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* non-default
using System.IO;
using System.Security.Cryptography;

namespace OWA.Utility
{
    /// <summary>
    /// Regions for login
    /// </summary>
    public enum Regions : int
    {
        /// <summary>
        /// No region (defaults to US)
        /// </summary>
        None = 0,
        /// <summary>
        /// US region
        /// </summary>
        US,
        /// <summary>
        /// Europe region
        /// </summary>
        EU
    }

    /// <summary>
    /// Pages of battle.net
    /// </summary>
    public enum Pages : int
    {
        /// <summary>
        /// Default invalid pages
        /// </summary>
        None = 0,
        /// <summary>
        /// Unknown page
        /// </summary>
        Unknown,
        /// <summary>
        /// Root page of battle.net
        /// </summary>
        Root,
        /// <summary>
        /// Account management page of battle.net
        /// </summary>
        AccountManagement,
        /// <summary>
        /// Login page of battle.net
        /// </summary>
        Login,
    }

    /// <summary>
    /// Login result from attemping to log in to battle.net
    /// </summary>
    public enum LoginResult : int
    {
        /// <summary>
        /// Default invalid result
        /// </summary>
        None = 0,
        /// <summary>
        /// An unknown error occurred
        /// </summary>
        Unknown,
        /// <summary>
        /// Invalid username or password
        /// </summary>
        InvalidUsernameOrPassword,
        /// <summary>
        /// Account requires SMS or Authenticator code
        /// </summary>
        AuthenticationRequired,
        /// <summary>
        /// Battle.net requires captcha input
        /// </summary>
        Captcha,
        /// <summary>
        /// Successfully logged into battle..net
        /// </summary>
        Success,
        /// <summary>
        /// Invalid region
        /// </summary>
        InvalidRegion
    }

    /// <summary>
    /// Type of authentication required
    /// </summary>
    public enum AuthenticationType : int
    {
        /// <summary>
        /// No authentication required
        /// </summary>
        None = 0,
        /// <summary>
        /// Authenticator code required
        /// </summary>
        Authenticator,
        /// <summary>
        /// SMS code required
        /// </summary>
        SMS
    }

    /// <summary>
    /// Login data from the login page
    /// </summary>
    public struct LoginData
    {
        /// <summary>
        /// CSRF token input
        /// </summary>
        public string CSRFToken { get; set; }
        
        /// <summary>
        /// Session timeout
        /// </summary>
        public long SessionTimeout { get; set; }

        /// <summary>
        /// Creates a new LoginData struct with the specified values
        /// </summary>
        /// <param name="token">CSRF token</param>
        /// <param name="timeout">Session timeout</param>
        public LoginData(string token, long timeout)
        {
            CSRFToken = token;
            SessionTimeout = timeout;
        }
    }

    public static class Cryptography
    {
        public static byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();

                return aes.Key;
            }
        }

        public static byte[] GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();

                return aes.IV;
            }
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }
    }
}
