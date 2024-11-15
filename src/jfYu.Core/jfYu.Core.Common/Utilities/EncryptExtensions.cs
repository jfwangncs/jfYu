using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace jfYu.Core.Common.Utilities
{
    public static class EncryptExtensions
    {
        //AES加密字符
        private const string encryptKey = "4h!@w$rng,i#$@x1%)5^3(7*5P31/Ee0";

        //DES密钥
        private static readonly byte[] _KEY = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        //DES向量
        private static readonly byte[] _IV = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };

        #region Aes
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(this string str, string key = null)
        {

            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {

                Key = Encoding.UTF8.GetBytes(key ?? encryptKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(this string str, string key = null)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key ?? encryptKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region Des
        /// <summary>
        /// DES加密操作
        /// </summary>
        /// <param name="normalTxt"></param>
        /// <returns></returns>
        public static string DesEncrypt(this string normalTxt)
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(_KEY, _IV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(normalTxt);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            return strRet;
        }

        /// <summary>
        /// DES解密操作
        /// </summary>
        /// <param name="securityTxt">加密字符串</param>
        /// <returns></returns>
        public static string DesDecrypt(this string securityTxt)//解密  
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
            byte[] byEnc;
            try
            {
                securityTxt.Replace("_%_", "/");
                securityTxt.Replace("-%-", "#");
                byEnc = Convert.FromBase64String(securityTxt);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(_KEY, _IV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        #endregion

        #region SHA

        public static string SHAmd5Encrypt(this string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组  
            var Md5 = new MD5CryptoServiceProvider();
            var encryptbytes = Md5.ComputeHash(bytes);//求哈希值  
            return Base64To16(encryptbytes);//将Byte[]数组转为净荷明文(其实就是字符串)  
        }
        public static string SHA1Encrypt(this string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA = new SHA1CryptoServiceProvider();
            var encryptbytes = SHA.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        public static string SHA256Encrypt(this string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA256 = new SHA256CryptoServiceProvider();
            var encryptbytes = SHA256.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        public static string SHA384Encrypt(this string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA384 = new SHA384CryptoServiceProvider();
            var encryptbytes = SHA384.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        public static string SHA512Encrypt(this string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);
            var SHA512 = new SHA512CryptoServiceProvider();
            var encryptbytes = SHA512.ComputeHash(bytes);
            return Base64To16(encryptbytes);
        }
        private static string Base64To16(byte[] buffer)
        {
            string md_str = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                md_str += buffer[i].ToString("x2");
            }
            return md_str;
        }
        #endregion
    }
}
