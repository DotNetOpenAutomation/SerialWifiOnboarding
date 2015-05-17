using System;
using System.Text;

namespace PervasiveDigital.Onboarding.ManagedSecurityProviders
{
    public static class BlockTea
    {
        private const UInt32 Delta = 0x9e3779b9;

        public static UInt32[] KeyFromString(string key)
        {
            while (key.Length < 16)
                key += ' ';

            return StringToUIntArray(key);
        }

        public static UInt32[] StringToUIntArray(string s)
        {
            var utfBytes = Encoding.UTF8.GetBytes(s);
            return BytesToUIntArray(utfBytes);
        }

        public static UInt32[] BytesToUIntArray(byte[] sourceBytes)
        {
            var resultLen = (sourceBytes.Length + 3) / 4;
            var bytes = new byte[resultLen * 4];
            Array.Clear(bytes, 0, bytes.Length);
            Array.Copy(sourceBytes, bytes, sourceBytes.Length);

            var result = new UInt32[resultLen];
            for (int i = 0; i < resultLen; ++i)
            {
                result[i] = (UInt32)(bytes[i << 2] << 24) | (UInt32)(bytes[(i << 2) + 1] << 16) | (UInt32)(bytes[(i << 2) + 2] << 8) | (UInt32)bytes[(i << 2) + 3];
            }
            return result;
        }

        public static byte[] UIntArrayToBytes(UInt32[] source)
        {
            var result = new byte[source.Length * 4];

            int offset = 0;
            foreach (var ui in source)
            {
                result[offset] = (byte)((ui >> 24) & 0xff);
                result[offset+1] = (byte)((ui >> 16) & 0xff);
                result[offset+2] = (byte)((ui >> 8) & 0xff);
                result[offset+3] = (byte)(ui & 0x0ff);
                offset += 4;
            }

            return result;
        }

        public static void Encrypt(UInt32[] v, UInt32[] key)
        {
            if (v == null)
                throw new ArgumentNullException("v");
            if (key.Length != 4)
                throw new ArgumentException("key must be 16 bytes");
            
            int n = v.Length;
            if (n == 0)
                return;
            UInt32 y, z, sum;
            UInt32 p, rounds, e;
            // encoding
            rounds = (UInt32)(6 + 52 / n);
            sum = 0;
            z = v[n - 1];
            do
            {
                sum += Delta;
                e = (sum >> 2) & 3;
                for (p = 0; p < n - 1; p++)
                {
                    y = v[p + 1];
                    z = v[p] += (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (key[(p & 3) ^ e] ^ z))); // MX
                }
                y = v[0];
                z = v[n - 1] += (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (key[(p & 3) ^ e] ^ z))); // MX
            } while (--rounds > 0);
        }

        public static void Decrypt(UInt32[] v, UInt32[] key)
        {
            if (v == null)
                throw new ArgumentNullException("v");
            if (key.Length != 4)
                throw new ArgumentException("key must be 16 bytes");

            int n = v.Length;
            if (n == 0)
                return;
            UInt32 y, z, sum;
            UInt32 p, rounds, e;
            // decoding
            rounds = (UInt32)(6 + 52 / n);
            sum = rounds * Delta;
            y = v[0];
            do
            {
                e = (sum >> 2) & 3;
                for (p = (UInt32)n - 1; p > 0; p--)
                {
                    z = v[p - 1];
                    y = v[p] -= (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (key[(p & 3) ^ e] ^ z))); // MX
                }
                z = v[n - 1];
                y = v[0] -= (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (key[(p & 3) ^ e] ^ z))); // MX
                sum -= Delta;
            } while (--rounds > 0);
        }
    }
}
