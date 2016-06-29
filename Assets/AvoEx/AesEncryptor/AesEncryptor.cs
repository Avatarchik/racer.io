using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

/* See the "http://avoex.com/avoex/default-license/" for the full license governing this code. */

namespace AvoEx
{
    // based on http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp
    public static class AesEncryptor
    {
        // only the 128, 192, and 256-bit key sizes are specified in the AES standard. https://en.wikipedia.org/wiki/Advanced_Encryption_Standard
        const int keySize = 16; // keySize must be 16, 24 or 32 bytes.
        const string keyString = "defaultKeyString"; // EDIT 'keyString' BEFORE RELEASE. keyString must be longer than keySize.
        // DO NOT EDIT 'keySize, keyString' AFTER RELEASE YOUR PROJECT.
        // if you change keyString, you can not decrypt saved data encrypted by old keyString.

        // The size of the IV property must be the same as the BlockSize property divided by 8.
        // https://msdn.microsoft.com/ko-kr/library/system.security.cryptography.symmetricalgorithm.iv(v=vs.110).aspx
        const int IvLength = 16;

        static readonly UTF8Encoding encoder;
        static readonly AesManaged aes;

        static AesEncryptor()
        {
            encoder = new UTF8Encoding();
            aes = new AesManaged { Key = encoder.GetBytes(keyString).Take(keySize).ToArray() };
            aes.BlockSize = IvLength * 8; // only the 128-bit block size is specified in the AES standard.
        }

        public static byte[] GenerateIV()
        {
            aes.GenerateIV();
            return aes.IV;
        }

        #region PREPEND_VECTOR
        /// <summary>
        /// encrypt bytes with random vector. prepend vector to result.
        /// </summary>
        public static byte[] Encrypt(byte[] buffer)
        {
            aes.GenerateIV();
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                byte[] inputBuffer = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                return aes.IV.Concat(inputBuffer).ToArray();
            }
        }

        /// <summary>
        /// decrypt bytes, encrypted by Encrypt(byte[]).
        /// </summary>
        public static byte[] Decrypt(byte[] buffer)
        {
            byte[] iv = buffer.Take(IvLength).ToArray();
            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv))
            {
                return decryptor.TransformFinalBlock(buffer, IvLength, buffer.Length - IvLength);
            }
        }
        #endregion PREPEND_VECTOR

        #region CUSTOM_KEY
        /// <summary>
        /// not prepend vector to result. you must use DecryptIV(byte[], byte[]) to decrypt.
        /// </summary>
        public static byte[] EncryptIV(byte[] buffer, byte[] IV)
        {
            return EncryptKeyIV(buffer, aes.Key, IV);
        }

        /// <summary>
        /// decrypt bytes, encrypted by EncryptIV(byte[], byte[]).
        /// </summary>
        public static byte[] DecryptIV(byte[] buffer, byte[] IV)
        {
            return DecryptKeyIV(buffer, aes.Key, IV);
        }

        public static byte[] EncryptKeyIV(byte[] buffer, byte[] key, byte[] IV)
        {
            using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
            {
                return encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        public static byte[] DecryptKeyIV(byte[] buffer, byte[] key, byte[] IV)
        {
            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
            {
                return decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }
        #endregion CUSTOM_KEY

        #region ENCRYPT_TO_STRING
        // string
        /// <summary>
        /// encrypt string with random vector. prepend vector to result.
        /// </summary>
        public static string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        /// <summary>
        /// decrypt string, encrypted by Encrypt(string).
        /// </summary>
        [Obsolete("Decrypt(string) has been made obsolete. Please use the DecryptString(string).")]
        public static string Decrypt(string encrypted)
        {
            return DecryptString(encrypted);
        }
        public static string DecryptString(string encrypted)
        {
            return DecryptString(Convert.FromBase64String(encrypted));
        }
        public static string DecryptString(byte[] encrypted)
        {
            byte[] bytesDecrypted = Decrypt(encrypted);
            return encoder.GetString(bytesDecrypted, 0, bytesDecrypted.Length);
        }

        /// <summary>
        /// not prepend vector to result. you must use DecryptIV(string, byte[]) to decrypt.
        /// </summary>
        public static string EncryptIV(string unencrypted, byte[] vector)
        {
            return Convert.ToBase64String(EncryptIV(encoder.GetBytes(unencrypted), vector));
        }

        /// <summary>
        /// decrypt string, encrypted by EncryptIV(string, byte[]).
        /// </summary>
        public static string DecryptIV(string encrypted, byte[] vector)
        {
            byte[] bytesDecrypted = DecryptIV(Convert.FromBase64String(encrypted), vector);
            return encoder.GetString(bytesDecrypted, 0, bytesDecrypted.Length);
        }

        // bool
        public static string Encrypt(bool unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static bool DecryptBool(string encrypted)
        {
            return DecryptBool(Convert.FromBase64String(encrypted));
        }
        public static bool DecryptBool(byte[] encrypted)
        {
            return BitConverter.ToBoolean(Decrypt(encrypted), 0);
        }

        // char
        public static string Encrypt(char unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static char DecryptChar(string encrypted)
        {
            return DecryptChar(Convert.FromBase64String(encrypted));
        }
        public static char DecryptChar(byte[] encrypted)
        {
            return BitConverter.ToChar(Decrypt(encrypted), 0);
        }

        // double
        public static string Encrypt(double unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static double DecryptDouble(string encrypted)
        {
            return DecryptDouble(Convert.FromBase64String(encrypted));
        }
        public static double DecryptDouble(byte[] encrypted)
        {
            return BitConverter.ToDouble(Decrypt(encrypted), 0);
        }

        // float
        public static string Encrypt(float unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static float DecryptFloat(string encrypted)
        {
            return DecryptFloat(Convert.FromBase64String(encrypted));
        }
        public static float DecryptFloat(byte[] encrypted)
        {
            return BitConverter.ToSingle(Decrypt(encrypted), 0);
        }

        // int
        public static string Encrypt(int unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }
        public static int DecryptInt(string encrypted)
        {
            return DecryptInt(Convert.FromBase64String(encrypted));
        }
        public static int DecryptInt(byte[] encrypted)
        {
            return BitConverter.ToInt32(Decrypt(encrypted), 0);
        }

        // long
        public static string Encrypt(long unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static long DecryptLong(string encrypted)
        {
            return DecryptLong(Convert.FromBase64String(encrypted));
        }
        public static long DecryptLong(byte[] encrypted)
        {
            return BitConverter.ToInt64(Decrypt(encrypted), 0);
        }

        // short
        public static string Encrypt(short unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static short DecryptShort(string encrypted)
        {
            return DecryptShort(Convert.FromBase64String(encrypted));
        }
        public static short DecryptShort(byte[] encrypted)
        {
            return BitConverter.ToInt16(Decrypt(encrypted), 0);
        }

        // uint
        public static string Encrypt(uint unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static uint DecryptUInt(string encrypted)
        {
            return DecryptUInt(Convert.FromBase64String(encrypted));
        }
        public static uint DecryptUInt(byte[] encrypted)
        {
            return BitConverter.ToUInt32(Decrypt(encrypted), 0);
        }

        // ulong
        public static string Encrypt(ulong unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static ulong DecryptULong(string encrypted)
        {
            return DecryptULong(Convert.FromBase64String(encrypted));
        }
        public static ulong DecryptULong(byte[] encrypted)
        {
            return BitConverter.ToUInt64(Decrypt(encrypted), 0);
        }

        // ushort
        public static string Encrypt(ushort unencrypted)
        {
            return Convert.ToBase64String(Encrypt(BitConverter.GetBytes(unencrypted)));
        }

        public static ushort DecryptUShort(string encrypted)
        {
            return DecryptUShort(Convert.FromBase64String(encrypted));
        }
        public static ushort DecryptUShort(byte[] encrypted)
        {
            return BitConverter.ToUInt16(Decrypt(encrypted), 0);
        }
        #endregion ENCRYPT_TO_STRING
    }
}