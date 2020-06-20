using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace qyAPITest.Controllers
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

        public static string SHA256(string str)
        {
            //如果str有中文，不同Encoding的sha是不同的！！
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);

            SHA256Managed Sha256 = new SHA256Managed();
            byte[] by = Sha256.ComputeHash(SHA256Data);

            return BitConverter.ToString(by); //64
            //return Convert.ToBase64String(by);                         //44
        }

        public static string DecodeFrom64(string encodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ConvertBase64(string encodeData)
        {
            System.Text.Encoding encode = System.Text.Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(encodeData);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }

        

    }
}