using System;
using System.Text;
using Microsoft.SPOT;

using PervasiveDigital.Onboarding.DefaultSecurityProviders;

namespace DiffieHellmanTest
{
    public class Program
    {
        public static void Main()
        {
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

            BlockTea.Process(bytes, bytes.Length, serverK);
            Dump("cipher", bytes);

            // bytes sent to client 

            BlockTea.Process(bytes, -bytes.Length, clientK);
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
