using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ADT.Common
{
    public class EncryptionInfo
    {
        public const string userKey = "aiyi2019";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encryption(string Text, string key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(Text);
                string md5SKey = Get32MD5One(key).Substring(0, 8);
                des.Key = ASCIIEncoding.ASCII.GetBytes(md5SKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(md5SKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch (Exception e)
            { return "error"; }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="psw"></param>
        /// <returns></returns>
        public static string Decode(string Text, string key)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int len;
                len = Text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                string md5SKey = Get32MD5One(key).Substring(0, 8);
                des.Key = ASCIIEncoding.ASCII.GetBytes(md5SKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(md5SKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());
            }
            catch { return "error"; }
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string Get32MD5One(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }
    }
}
