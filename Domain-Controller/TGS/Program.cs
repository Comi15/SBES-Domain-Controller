using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace TGS
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8055/TGS";
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;


            ServiceHost host = new ServiceHost(typeof(Ticket));
            host.AddServiceEndpoint(typeof(ITicket), binding, address);

            host.Open();
            Console.WriteLine("TGS service has been started.");
            Console.WriteLine("User that started the service : " + WindowsIdentity.GetCurrent().Name);


            Console.ReadLine();
        }

    }
}
