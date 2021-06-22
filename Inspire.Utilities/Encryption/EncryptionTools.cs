namespace Inspire.Utilities.Encryption
{
    using System;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;

    using Inspire.Utilities.Extensions;

    [SuppressUnmanagedCodeSecurity]
    public static class EncryptionTools
    {
        private const string SaltDelimiter = "::";
        private const string Salt = "AB04B79D-1B64-41BD-8811-7D7E2E85AB79-F4C6C781-1E9B-419D-AAC8-55B5D00B3A15";

        public static string Encrypt(string toEncrypt, string key, string salt = null)
        {
            if (string.IsNullOrEmpty(toEncrypt))
            {
                throw new ArgumentNullException("toEncrypt");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            toEncrypt = $"{toEncrypt}{SaltDelimiter}{salt ?? Salt}";
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var hashProvider = new MD5CryptoServiceProvider();
            var keyArray = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));

            var tdesAlgorithm = new TripleDESCryptoServiceProvider
                                {
                                    Key = keyArray,
                                    Mode = CipherMode.ECB,
                                    Padding = PaddingMode.PKCS7
                                };

            byte[] resultArray;
            try
            {
                var transform = tdesAlgorithm.CreateEncryptor();
                resultArray = transform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            return resultArray.Length < 1 ? string.Empty : Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cryptString, string key)
        {
            if (string.IsNullOrEmpty(cryptString))
            {
                throw new ArgumentNullException("cryptString");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            // https://stackoverflow.com/questions/858761/what-causing-this-invalid-length-for-a-base-64-char-array/2453693#2453693
            cryptString = cryptString.Replace(' ', '+');

            var toEncryptArray = Convert.FromBase64String(cryptString);

            var hashProvider = new MD5CryptoServiceProvider();
            var keyArray = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));

            var tdesAlgorithm = new TripleDESCryptoServiceProvider
                                {
                                    Key = keyArray,
                                    Mode = CipherMode.ECB,
                                    Padding = PaddingMode.PKCS7
                                };

            byte[] resultArray;
            try
            {
                var cryptoTransform = tdesAlgorithm.CreateDecryptor();
                resultArray = cryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            if (resultArray.Length < 1)
            {
                return string.Empty;
            }

            var data = Encoding.UTF8.GetString(resultArray);
            return data.Substring(0, data.LastIndexOf(SaltDelimiter));
        }

        public static string Hash(string text)
        {
            if (text.IsNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentNullException("text");
            }

            var crypt = new SHA256Managed();
            var computeHash = crypt.ComputeHash(Encoding.UTF8.GetBytes(text));

            var hash = new StringBuilder();
            foreach (var theByte in computeHash)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}