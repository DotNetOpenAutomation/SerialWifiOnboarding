using System;
using System.Text;
using Microsoft.SPOT;

using PervasiveDigital.Onboarding.ManagedSecurityProviders;

namespace ManagedProviderTest
{
    public class Program
    {
        public static void Main()
        {
            // The managed providers exist to help out NETMF deployments where the security
            //   routines are not in the firmware (e.g., early versions of Molecule.Net)
            // These routines will provide the necessary security support for onboarding,
            //   with important caveats:
            //   1) These routines are very slow
            //   2) There are known weaknesses - for instance, the RNG is the default .net RNG and not the crypto RNG. There may be other algorithmic weaknesses
            //   3) These routines are very, very slow - expect 30-40 seconds for each Diffie-Hellman call on and STM32F11
            //   

            Debug.Print("Starting...");

            DiffieHellman server = new DiffieHellman(128).GenerateRequest();
            Debug.Print("Server sends:");
            Debug.Print(server.ToString());

            DiffieHellman client = new DiffieHellman(128).GenerateResponse(
                                                    server.ToString());
            Debug.Print("Client responds:");
            Debug.Print(client.ToString());

            server.HandleResponse(client.ToString());

            Debug.Print("Server key:");
            Debug.Print(Convert.ToBase64String(server.Key));

            Debug.Print("Client key:");
            Debug.Print(Convert.ToBase64String(client.Key));
            // server uses this key
            var serverK = BlockTea.BytesToUIntArray(server.Key);
            // client has calculated this key
            var clientK = BlockTea.BytesToUIntArray(client.Key);

            var cleartext = "CloudGate,mypassword";

            var bytes = BlockTea.StringToUIntArray(cleartext);
            Dump("cleartext", bytes);

            BlockTea.Encrypt(bytes, serverK);
            Dump("cipher", bytes);

            // bytes sent to client 

            BlockTea.Decrypt(bytes, clientK);
            Dump("cleartext recovered", bytes);

            var deciphered = BlockTea.UIntArrayToBytes(bytes);

            Debug.Print("The transmitted secret is " + new string(Encoding.UTF8.GetChars(deciphered)));
        }
        private static void Dump(string header, UInt32[] buffer)
        {
            Debug.Print("---- " + header + " ------------------------------------");
            var sb = new StringBuilder();
            foreach (var v in buffer)
            {
                sb.Append(v + " ");
            }
            Debug.Print(sb.ToString());
            Debug.Print("");
        }
    }
}
