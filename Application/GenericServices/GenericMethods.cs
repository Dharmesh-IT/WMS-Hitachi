using System.IO;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Application.GenericServices
{
    public static class GenericMethods
    {
        #region Encryption
        static byte[] GetBytes(String keyBytes, int length)
        {
            byte[] keyBytes1 = new byte[length];
            byte[] parameterKeyBytes = System.Text.Encoding.UTF8.GetBytes(keyBytes);
            Array.Copy(parameterKeyBytes, 0, keyBytes1, 0, Math.Min(parameterKeyBytes.Length, keyBytes1.Length));

            return keyBytes1;
        }

        static string Array2String<T>(IEnumerable<T> list)
        {
            return "[" + string.Join(",", list) + "]";
        }

        public static string Encrypt(string PlainText, string key, string iv)
        {
            byte[] keyBytes = GetBytes(key, 32);
            byte[] ivBytes = GetBytes(key, 16);
            RijndaelManaged aes = new RijndaelManaged();

            aes.Mode = CipherMode.CBC;
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = Encoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return BitConverter.ToString(CipherText).Replace("-", string.Empty);
        }

        public static string Decrypt(string encryptedText, string key, string iv)
        {
            try
            {
                int length = encryptedText.Length;
                byte[] keyBytes = GetBytes(key, 32);
                byte[] ivBytes = GetBytes(key, 16);

                string encrytedTextNew = "";
                char[] encrytArray = encryptedText.ToCharArray(0, encryptedText.Length);
                for (int i = 0; i < encryptedText.Length; i++)
                {
                    if (i != 0)
                    {
                        int j = i + 1;
                        if (j % 2 == 0)
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i] + "-";
                        }
                        else
                        {
                            encrytedTextNew = encrytedTextNew + encrytArray[i];
                        }
                    }
                    else if (i == 0)
                    {
                        encrytedTextNew = encrytedTextNew + encrytArray[i];
                    }
                }

                encrytedTextNew = encrytedTextNew.Remove(encrytedTextNew.Length - 1);

                RijndaelManaged aes = new RijndaelManaged();
                aes.Mode = CipherMode.CBC;
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                ICryptoTransform encrypto = aes.CreateDecryptor();

                byte[] plainTextByte = Array.ConvertAll<string, byte>(encrytedTextNew.Split('-'), s => Convert.ToByte(s, 16));
                byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                return ASCIIEncoding.UTF8.GetString(CipherText);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string DecryptClassObject(string cipherData, string keyString, string ivString)
        {
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = Encoding.UTF8.GetBytes(ivString);

            try
            {
                using (var rijndaelManaged =
                       new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(cipherData)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
            // You may want to catch more exceptions here...
        }
        public static string AESDecrypt(string cipherText, string key, string IV)
        {
            var keybytes = Encoding.UTF8.GetBytes(key)
    ;
            var iv = Encoding.UTF8.GetBytes(IV);

            //var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            //var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        private static byte[] AESEncrypt(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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

        #endregion
        public static string GetMD5Hash(string input)
        {
            //this method is obsolete
            // System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.Security.Cryptography.MD5 x = System.Security.Cryptography.MD5.Create();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string md5 = s.ToString();
            return md5;
        }

        public static string GetHMACHash(string plainText, string key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            HMACSHA256 hmacsha256 = new HMACSHA256(encoding.GetBytes(key));
            byte[] bs = hmacsha256.ComputeHash(encoding.GetBytes(plainText));

            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("X2").ToLower());
            }
            string hmachash = s.ToString();
            return hmachash;
        }

        public static string GetSha256Hash(string plainText)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsValidSHA1(string s)
        {
            var regex = @"^[a-fA-F0-9]{40}$";
            var match = Regex.Match(s, regex, RegexOptions.IgnoreCase);
            return (match.Success);
        }

        public static string GenerateSHA512String(string inputString)
        {
            //depricated code
            //SHA512 sha512 = SHA512Managed.Create();
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        public static string TripleDESEncrypt(string plainText, string key, string IV)
        {
            byte[] keyArray;
            byte[] IVArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(plainText);
            keyArray = UTF8Encoding.UTF8.GetBytes((key.Length > 24 ? key.Substring(0, 24) : key));
            IVArray = UTF8Encoding.UTF8.GetBytes((IV.Length > 8 ? IV.Substring(0, 8) : IV));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.IV = IVArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string TripleDESDecrypt(string cipherText, string key, string IV)
        {
            byte[] keyArray;
            byte[] IVArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherText);
            keyArray = UTF8Encoding.UTF8.GetBytes((key.Length > 24 ? key.Substring(0, 24) : key));
            IVArray = UTF8Encoding.UTF8.GetBytes((IV.Length > 8 ? IV.Substring(0, 8) : IV));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.IV = IVArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        //public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        //{
        //    // Check arguments.
        //    if (cipherText == null || cipherText.Length <= 0)
        //    {
        //        throw new ArgumentNullException("cipherText");
        //    }

        //    if (key == null || key.Length <= 0)
        //    {
        //        throw new ArgumentNullException("key");
        //    }

        //    if (iv == null || iv.Length <= 0)
        //    {
        //        throw new ArgumentNullException("key");
        //    }

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    // Create an RijndaelManaged object
        //    // with the specified key and IV.
        //    using (var rijAlg = new RijndaelManaged())
        //    {
        //        //Settings
        //        rijAlg.Mode = CipherMode.CBC;
        //        rijAlg.Padding = PaddingMode.PKCS7;
        //        rijAlg.FeedbackSize = 128;

        //        rijAlg.Key = key;
        //        rijAlg.IV = iv;

        //        // Create a decrytor to perform the stream transform.
        //        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        //        // Create the streams used for decryption.
        //        using (var msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (var srDecrypt = new StreamReader(csDecrypt))
        //                {
        //                    // Read the decrypted bytes from the decrypting stream
        //                    // and place them in a string.
        //                    plaintext = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }

        //    return plaintext;

        //}
        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            byte[] encrypted;

            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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
    }
}
