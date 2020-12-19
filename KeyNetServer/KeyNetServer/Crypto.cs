using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Numerics;

namespace KeyNetClient
{
    static class Crypto
    {
        private static RSACryptoServiceProvider rsa;

        private static DSACryptoServiceProvider dsa;

        public static string PublicKey { get { return rsa.ToXmlString(false); } }

        public static BigInteger[] DSAdata { get { return GetDSA(); } }

        public static void DsaGen()
        {
            dsa = new DSACryptoServiceProvider { PersistKeyInCsp = true };
        }

        private static BigInteger[] GetDSA()
        {
            BigInteger[] data = new BigInteger[5];

            DSAParameters param = dsa.ExportParameters(true);
            
            byte[] temp = new byte[param.P.Length];
            Array.Copy(param.P, temp, param.P.Length);
            Array.Reverse(temp);
            byte[] temp1 = new byte[param.P.Length + 1];
            Array.Copy(temp, temp1, param.P.Length);
            data[0] = new BigInteger(temp1);

            temp = new byte[param.Q.Length];
            Array.Copy(param.Q, temp, param.Q.Length);
            Array.Reverse(temp);
            temp1 = new byte[param.Q.Length + 1];
            Array.Copy(temp, temp1, param.Q.Length);
            data[1] = new BigInteger(temp1);

            temp = new byte[param.G.Length];
            Array.Copy(param.G, temp, param.G.Length);
            Array.Reverse(temp);
            temp1 = new byte[param.G.Length + 1];
            Array.Copy(temp, temp1, param.G.Length);
            data[2] = new BigInteger(temp1);

            temp = new byte[param.Y.Length];
            Array.Copy(param.Y, temp, param.Y.Length);
            Array.Reverse(temp);
            temp1 = new byte[param.Y.Length + 1];
            Array.Copy(temp, temp1, param.Y.Length);
            data[3] = new BigInteger(temp1);

            temp = new byte[param.X.Length];
            Array.Copy(param.X, temp, param.X.Length);
            Array.Reverse(temp);
            temp1 = new byte[param.X.Length + 1];
            Array.Copy(temp, temp1, param.X.Length);
            data[4] = new BigInteger(temp1);

           

            return data;
        }

        private static BigInteger GetRND(int length)
        {
            Random rnd = new Random();
            byte[] temp = new byte[length];
            rnd.NextBytes(temp);
            return new BigInteger(temp);
        }

        private static BigInteger EuclidAlgoritm(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0; y = 1;
                return b;
            }
            BigInteger x1 = new BigInteger();
            BigInteger y1 = new BigInteger();
            BigInteger d = EuclidAlgoritm(b % a, a, ref x1, ref y1);

            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        public static BigInteger FindInverse(BigInteger a, BigInteger n)
        {
            BigInteger x = new BigInteger();
            BigInteger y = new BigInteger();
            BigInteger g = EuclidAlgoritm(a, n, ref x, ref y);
            if (g != 1)
            {
                return 0;
            }
            else
            {
                if (x < 0)
                    return n + x;
                else
                    return x % n;
            }
        }

        public static void RsaKeyGen()
        {
            rsa = new RSACryptoServiceProvider { PersistKeyInCsp = true };
        }

        public static byte[] RsaEncrypt(byte[] data)
        {
            return rsa.Encrypt(data, true);
        }

        public static byte[] ForeignRsaEncrypt(byte[] data, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.Encrypt(data, true);
        }

        public static byte[] RsaDecrypt(byte[] data)
        {
            return rsa.Decrypt(data, true);
        }

        public static byte[] RsaSign(byte[] data)
        {
            return rsa.SignData(data, SHA1.Create());
        }

        public static bool RsaVerify(byte[] data, byte[] sign, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider.VerifyData(data, SHA1.Create(), sign);
        }

        public static byte[] AesKeyPartGen()
        {
            Aes aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();

            byte[] IV = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                IV[i] = aes.IV[i];
            }

            return aes.Key.Concat(IV).ToArray();
        }

        public static byte[] UniteAesKey(byte[] key1, byte[] key2, byte[] iv1, byte[] iv2)
        {          
            byte[] key = key1.Concat(key2).ToArray();

            byte[] iv = iv1.Concat(iv2).ToArray();

            return key.Concat(iv).ToArray();
        }

        private static byte[] EncryptAes(string plainText, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }

        }

        private static string DecryptAes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
