using System;
using System.Text;

namespace Cipher_CLI
{
    class Cipher
    {
        public string Key { get; set; }

        public Cipher(string key)
        {
            Key = key;
        }

        public string Encrypt(string value)
        {
            return Encrypt(value, Key);
        }

        public string Decrypt(string value)
        {
            return Decrypt(value, Key);
        }

        public static string Encrypt(string value, string key)
        {
            return Convert.ToBase64String(XOR(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(key)));
        }

        public static string Decrypt(string value, string key)
        {
            return Encoding.UTF8.GetString(XOR(Convert.FromBase64String(value), Encoding.UTF8.GetBytes(key)));
        }

        private static byte[] XOR(byte[] buffer, byte[] key)
        {
            for (int i = 0, j = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= key[j];
                if (++j == key.Length) j = 0;
            }

            return buffer;
        }
    }
}
