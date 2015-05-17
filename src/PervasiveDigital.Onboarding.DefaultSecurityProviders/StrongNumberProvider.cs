using System;
using System.Text;
using System.Security.Cryptography;

namespace PervasiveDigital.Onboarding.DefaultSecurityProviders
{
    internal class StrongNumberProvider
    {
        // Weak RNG!
        private static Random csp = new Random();

        public uint NextUInt32()
        {
            byte[] res = new byte[4];
            csp.NextBytes(res);
            return BitConverter.ToUInt32(res, 0);
        }

        public int NextInt()
        {
            byte[] res = new byte[4];
            csp.NextBytes(res);
            return BitConverter.ToInt32(res, 0);
        }

        public Single NextSingle()
        {
            float numerator = NextUInt32();
            float denominator = uint.MaxValue;
            return numerator / denominator;
        }
    }
}
