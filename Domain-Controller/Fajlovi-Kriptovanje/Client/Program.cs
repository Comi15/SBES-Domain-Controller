using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/SecurityService";
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

			Console.WriteLine("Korisnik koji je pokrenuo servis je : " + WindowsIdentity.GetCurrent().Name);

			string a = WindowsIdentity.GetCurrent().Name;
			string[] b  = a.Split('\\');


			EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("wcfservice"));
			using (ClientProxy proxy = new ClientProxy(binding, endpointAddress))
			{

				proxy.Register("pera123");
				proxy.Register("mika43");
				proxy.Register("laza108");

				bool o = proxy.LogIn("pera1234");

				if(o)
                {
                    Console.WriteLine("Uspesno ste se ulogovaili");
                }

				else
                {
                    Console.WriteLine("Niste uneli dobru sifru");
                }

				while (true)
				{
					Console.WriteLine("1. Write to a File");
					Console.WriteLine("2. Read from a File");
                    Console.WriteLine("3. Close the app");

					string c = Console.ReadLine();
					
					int number = 0;
					if (!Int32.TryParse(c, out number))
					{
						Console.WriteLine("Unable to parse the value");
					}


					if (number == 1)
					{
						Console.WriteLine("Input what would you like to write to a file : ");
						string input = Console.ReadLine();
						proxy.CreateFile(b[1] + ".txt", input);
						
					}

					else if (number == 2)
					{
                        Console.WriteLine(proxy.ReadFile());
						
					}
                    else if(number == 3)
					{
						break;
					}
					else
                    {
                        Console.WriteLine("You pressed the wrong key");
                    }


				}

			}
            Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}
	}
}
