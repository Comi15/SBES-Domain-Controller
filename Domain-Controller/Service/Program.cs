using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace Service
{

	public class Program
	{
		
		
		static void Main(string[] args)
		{


			//TGS binding
			NetTcpBinding TGSbinding = new NetTcpBinding();
			string TGSaddress = "net.tcp://localhost:8055/TGS";
			TGSbinding.Security.Mode = SecurityMode.Transport;
			TGSbinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			TGSbinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

			TicketProxy proxy = new TicketProxy(TGSbinding, new EndpointAddress(new Uri(TGSaddress)));



			NetTcpBinding binding = new NetTcpBinding();

			int port = proxy.getDynamicPort();
			string address = $"net.tcp://localhost:{port}/Service";
            Console.WriteLine(address);
            Console.WriteLine(address);
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
			

            

            ServiceHost host = new ServiceHost(typeof(DataManagement));
			host.AddServiceEndpoint(typeof(IDataManagement),binding,address);
			
			host.Open();
			

			Console.WriteLine("Write/Read service has been started.");
			Console.WriteLine("User that started the service : " + WindowsIdentity.GetCurrent().Name);


			
            

				Services servis = new Services();
				servis.IpAdress = address;
				servis.IdentityService = TGS.Ticket.RandomString(16);
                Console.WriteLine("id servisa : " + servis.IdentityService);
				servis.Hostname = WindowsIdentity.GetCurrent().Name;

                string token = proxy.AddServiceTotable(servis);

			
                Console.ReadLine();

				host.Close();
				

				if (host.State == CommunicationState.Closed)
				{
					proxy.Delete(servis.IdentityService);
				}
        }
        
    }
}
