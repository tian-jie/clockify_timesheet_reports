using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Innocellence.WeChat.Domain.Common
{
    public  class EncryptionHelper
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
            var iLen = encodedData.Length % 4;
            if (iLen > 0)
            {
                encodedData = encodedData.PadRight(encodedData.Length +4- iLen, '=');
            }
            var base64EncodedBytes = Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ConvertBase64(string encodeData)
        {
            System.Text.Encoding encode = System.Text.Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(encodeData);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }



        /// <summary>
        /// 加密字节数组
        /// </summary>
        /// <param name="source">要加密的字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] Encrypt(byte[] source)
        {
            //source.CheckNotNull("source");
            SymmetricAlgorithm provider = new DESCryptoServiceProvider { Key = System.Text.Encoding.ASCII.GetBytes("z7KXXtJ^"), Mode = CipherMode.ECB, IV = System.Text.Encoding.ASCII.GetBytes("8o-TLksM!VA8gt^b6f%+FpUiQl+w7^") };

            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(source, 0, source.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }


        public static string DecryptDES(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes("z7KXXtJ^");
                byte[] rgbIV = Encoding.UTF8.GetBytes("8o-TLksM!VA8gt^b6f%+FpUiQl+w7^");
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }

        public static byte[] Decrypt(byte[] source)
        {
            SymmetricAlgorithm provider = new DESCryptoServiceProvider { Key = System.Text.Encoding.ASCII.GetBytes("z7KXXtJ^"), Mode = CipherMode.ECB, IV = System.Text.Encoding.ASCII.GetBytes("8o-TLksM!VA8gt^b6f%+FpUiQl+w7^") };


            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(source, 0, source.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }
}