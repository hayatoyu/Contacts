using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace Contacts
{
    class AESEncoder
    {
        public static string AESEncryptBase64(string origin)
        {
            string result = string.Empty;
            if(!string.IsNullOrEmpty(origin))
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                byte[] Key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
                byte[] IV = { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F };
                byte[] dataByteArray = Encoding.UTF8.GetBytes(origin);

                aes.Key = Key;
                aes.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        result = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            return result;
        }

        public static string AESDecrptBase64(string input)
        {
            string origin = string.Empty;

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] Key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            byte[] IV = { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F };

            aes.Key = Key;
            aes.IV = IV;

            byte[] dataByteArray = Convert.FromBase64String(input);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    origin = Encoding.UTF8.GetString(ms.ToArray());
                }
            }

                return origin;
        }
    }
}
