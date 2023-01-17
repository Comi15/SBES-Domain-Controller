using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;
using Authentication;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Service;

namespace Client
{
	public class Program
	{
		static readonly AuthenticationClass authentication = new AuthenticationClass();
		static bool loggedIN = false;
        const string SourceName = "Client.Program";
        const string LogName = "";
        private static EventLog customLog = null;

        static void Main(string[] args)
		{
			

            NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

			Console.WriteLine("User that started the client service : " + WindowsIdentity.GetCurrent().Name);

            //binding za authentication

            NetTcpBinding authBinding = new NetTcpBinding();
            string authAddress = "net.tcp://localhost:8888/Authentication";
            authBinding.Security.Mode = SecurityMode.Message;
            authBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            authBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            AuthenticationProxy authProxy = new AuthenticationProxy(authBinding, new EndpointAddress(new Uri(authAddress)));

            //binding za TGS
            NetTcpBinding TGSbinding = new NetTcpBinding();
            string TGSaddress = "net.tcp://localhost:8055/TGS";
            TGSbinding.Security.Mode = SecurityMode.Transport;
            TGSbinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            TGSbinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;

            TicketProxy ticketProxy = new TicketProxy(TGSbinding, new EndpointAddress(new Uri(TGSaddress)));

            

			Services servis = null;
			ClientProxy proxy = null;
			User u = new User();
			string token = "";
			

			while (true)
			{
				if (!loggedIN)
				{
						Console.WriteLine("1.Register");
					    Console.WriteLine("2.Login");
						string str = Console.ReadLine();
						int temp;
						if (!Int32.TryParse(str, out temp))
						{
						Console.WriteLine("Unable to parse the value");
						}
					//Login
					if (temp == 2)
					{
						while (servis == null)
						{
							Console.WriteLine("Username: ");
							u.Username = Console.ReadLine();
							Console.WriteLine("Password");
							u.Password = Console.ReadLine();
							if (authentication.Authenticated(u))
							{
								try
								{
                                                                       
									servis = ticketProxy.ConnectionWithTheService();
									if(servis == null)
                                    {
										Audit("Zahtevanog servisa nema u domenu");

									}

									else
                                    {
										Audit("Zahtevani servis je pronadjen u domenu");
									}
									loggedIN = true;
									Audit($"Uspesno se ulogovao {u.Username}");
									token = servis.Token;
									Console.WriteLine("id servisa: " + servis.IdentityService);
									EndpointAddress endpointAddress = new EndpointAddress(new Uri(servis.IpAdress));
                                    proxy = new ClientProxy(binding, endpointAddress);                                    
                                    string challengeResponseString = proxy.Crypto("Pozdrav,Kako si?", token, "jvz8bUAx", "encrypt");
                                    string cryptedMessage = proxy.ChallengeResponse(challengeResponseString, token);
                                    Console.WriteLine("Sent to the server : " +" Pozdrav,Kako si? : " +  challengeResponseString);
                                    Console.WriteLine("Server replied : " + proxy.Crypto(cryptedMessage, token, "jvz8bUAx", "decrypt"));
                                }
								catch (Exception)
								{
									Console.WriteLine("Trenutno ni jedan servis nije slobodan");
									loggedIN = false;
									break;
								}
								
							}
							else
							{
								Audit($"Autentifikacije nije prosla");
								Console.WriteLine("Wrong username or password.Please try again or register a new account.");
								break;
							}
						}
					}
					//Register
					else if(temp == 1)
					{

						while (servis == null)
						{
							Console.WriteLine("Username: ");
							u.Username = Console.ReadLine();
							Console.WriteLine("Password");
							u.Password = Console.ReadLine();
							try
							{
								if (authentication.WriteData(u))
								{
									try
									{
                                        loggedIN = true;
                                        servis = ticketProxy.ConnectionWithTheService();
										if (servis == null)
										{
											Audit("Zahtevanog servisa nema u domenu");

										}

										else
										{
											Audit("Zahtevani servis je pronadjen u domenu");
										}
										token = servis.Token;							                                        
                                        EndpointAddress endpointAddress = new EndpointAddress(new Uri(servis.IpAdress));
                                        proxy = new ClientProxy(binding, endpointAddress);
                                        string challengeResponseString = proxy.Crypto("Pozdrav,Kako si?", token, "jvz8bUAx", "encrypt");
                                        string cryptedMessage = proxy.ChallengeResponse(challengeResponseString, token);
                                        Console.WriteLine("Sent : " + cryptedMessage);
                                        Console.WriteLine("Server replied : " + proxy.Crypto(cryptedMessage, token, "jvz8bUAx", "decrypt"));
                                    }
									catch (Exception)
									{
										Console.WriteLine("Trenutno ni jedan servis nije slobodan");										
										break;
									}
									

								}
								else
								{
									Console.WriteLine($"Korisnik sa username {u.Username} je vec registrovan");
									break;
								}
							}
							catch (MySqlException ex)
							{
								loggedIN = false;
								Console.WriteLine(ex.Message);
							}
						}
					}

					else if(temp != 1 || temp != 2)
                    {
                        Console.WriteLine("You pressed the wrong key.");
						continue;
                    }
				}
				else
				{
						
						Console.WriteLine("1. Write to a File");
						Console.WriteLine("2. Read from a File");
						Console.WriteLine("3. Logout");
						Console.WriteLine("4. Close the app");

						string c = Console.ReadLine();

						int number = 0;
						if (!Int32.TryParse(c, out number))
						{
							Console.WriteLine("Unable to parse the value");
						}

						switch (number)
						{
							case 1:
								Console.WriteLine("Enter what would you like to write to a file : ");
								string input = Console.ReadLine();
								proxy.CreateFile(u.Username + ".txt", input,token);
								break;

							case 2:
								Console.WriteLine(proxy.ReadFile(u.Username + ".txt",token));
								break;

							case 3:
							    loggedIN = false;
								ticketProxy.UnSetBusy(servis.IdentityService);
								servis = null;							
								break;

						case 4:
							Console.WriteLine("Press any key to exit...");
							Console.ReadLine();
							ticketProxy.UnSetBusy(servis.IdentityService);
							servis = null;
							proxy.Close();
							authProxy.Close();
							ticketProxy.Close();
							return;							
								
						default:
							Console.WriteLine("You pressed the wrong key");
							break;
						}
				}
			}

			

		}

		static void Audit(string message)
		{
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
                customLog.WriteEntry(message);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }
	}
}
