namespace Inspire.Common.Mvc.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using Inspire.Utilities.Encryption;
    using Inspire.Utilities.Extensions;

    public class Cryptography
    {
        private const char ParameterDelimiter = '&';
        private const char ValueDelimiter = '=';

        private static readonly Regex Regex = new Regex(@"^[\w+\+\/]*$", RegexOptions.IgnoreCase);

        public static string GenerateToken(IDictionary<string, string> data, string encryptKey)
        {
            if (data.Keys.IsNullOrEmpty())
            {
                throw new ArgumentNullException("data");
            }

            var token = string.Join(
                ParameterDelimiter.ToString(),
                data.Select(item => $"{item.Key}{ValueDelimiter}{item.Value}"));
            var encryptedToken = EncryptionTools.Encrypt(token, encryptKey);
            return HttpUtility.UrlEncode(encryptedToken);
        }

        public static Dictionary<string, string> GetTokenData(string token, string encryptKey)
        {
            token = Decode(token);
            var decryptedToken = EncryptionTools.Decrypt(token, encryptKey);

            return decryptedToken.Split(ParameterDelimiter)
                                 .Select(
                                     item =>
                                     {
                                         var temp = item.Split(ValueDelimiter);
                                         return new KeyValuePair<string, string>(
                                             temp[0],
                                             temp.Length > 1 ? temp[1] : null);
                                     })
                                 .ToDictionary(key => key.Key, val => val.Value);
        }

        /// <summary>
        ///     Generate id token
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="encryptKey">Encrypt key</param>
        /// <returns>Token</returns>
        public static string GenerateTokenById(Guid id, string encryptKey)
        {
            var data = new Dictionary<string, string>
                       {
                           { "id", id.ToString() },
                           { "date", DateTime.Now.Ticks.ToString() }
                       };

            return GenerateToken(data, encryptKey);
        }

        /// <summary>
        ///     Deserialize id token
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="encryptedKey">Encrypted key</param>
        /// <returns>Key value pair: key = guid, val = generate time tick</returns>
        public static KeyValuePair<Guid, long> DeserializeIdToken(string token, string encryptedKey)
        {
            var data = GetTokenData(token, encryptedKey);
            return new KeyValuePair<Guid, long>(Guid.Parse(data["id"]), long.Parse(data["date"]));
        }

        public static string Decode(string token)
        {
            if (!Regex.IsMatch(token))
            {
                token = HttpUtility.UrlDecode(token);
            }

            // http://stackoverflow.com/a/2453693/1099945
            return token.Replace(' ', '+');
        }
    }
}