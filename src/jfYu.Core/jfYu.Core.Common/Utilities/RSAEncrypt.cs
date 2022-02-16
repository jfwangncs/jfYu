using System;
using System.IO;
using System.Security.Cryptography;

namespace jfYu.Core.Common.Utilities
{
    public class RSAEncrypt
    {

        private const int RsaKeySize = 2048;
        private const string publicKeyFileName = "RSA.Pub";
        private const string privateKeyFileName = "RSA.Private";


        /// <summary>
        ///在给定路径中生成XML格式的私钥和公钥。
        /// </summary>
        public void GenerateKeys(string path)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    // 获取私钥和公钥。
                    var publicKey = rsa.ToXmlString(false);
                    var privateKey = rsa.ToXmlString(true);

                    // 保存到磁盘
                    File.WriteAllText(Path.Combine(path, publicKeyFileName), publicKey);
                    File.WriteAllText(Path.Combine(path, privateKeyFileName), privateKey);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// 用给定路径的RSA公钥文件加密纯文本。
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="pathToPublicKey">用于加密的公钥路径.</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        public string Encrypt(string plainText, string pathToPublicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    //加载公钥
                    var publicXmlKey = File.ReadAllText(pathToPublicKey);
                    rsa.FromXmlString(publicXmlKey);

                    var bytesToEncrypt = System.Text.Encoding.Unicode.GetBytes(plainText);

                    var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);

                    return Convert.ToBase64String(bytesEncrypted);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// 给定路径的RSA私钥文件解密 加密文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="pathToPrivateKey">用于加密的私钥路径.</param>
        /// <returns>未加密数据的字符串</returns>
        public string Decrypt(string encryptedText, string pathToPrivateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    var privateXmlKey = File.ReadAllText(pathToPrivateKey);
                    rsa.FromXmlString(privateXmlKey);

                    var bytesEncrypted = Convert.FromBase64String(encryptedText);

                    var bytesPlainText = rsa.Decrypt(bytesEncrypted, false);

                    return System.Text.Encoding.Unicode.GetString(bytesPlainText);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
