using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Innocellence.WeChatMain.Common
{
    public static class EncryptionHelper
    {
        public static string Encrypt(string toEncrypt)
        {
            var keyArray = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["EncryptKey"]);
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Encrypt(string toEncrypt, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string toDecrypt)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["EncryptKey"]);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        public static string Decrypt(string toDecrypt, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Convert.FromBase64String(toDecrypt);

            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

    }
}