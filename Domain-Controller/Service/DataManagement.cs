using Common;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Service
{


	public class DataManagement : IDataManagement
	{
		
		//Initilazition vector for 3DES.
	    public string iv = "jvz8bUAx";
		

       

		/// <summary>
		/// Creates the file for the current user if it doesn't already exist encypts the data and writes it. 
		/// If the file already exists than it just encypts the data and appends it. 
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="input"></param>
		/// <param name="token"></param>
        public void CreateFile(string fileName,string input,string token)
        {
			
			string encryptedString = "";
			IIdentity identity = Thread.CurrentPrincipal.Identity;
			WindowsIdentity windowsIdentity = (WindowsIdentity)identity;
			Console.WriteLine("Ime klijenta koji je pozvao metodu 'CreateFile' : " + windowsIdentity.Name);

			encryptedString = Crypto(input, token, iv, "encrypt");
            Console.WriteLine("Encrypted : " + encryptedString);
			using (windowsIdentity.Impersonate())
			{
                Console.WriteLine("Process idenitniy :  " + WindowsIdentity.GetCurrent().Name);
				
				try
				{
					string path = fileName;
					// This text is added only once to the file.
					if (!File.Exists(path))
					{
						// Create a file to write to.
						using (StreamWriter sw = File.CreateText(path))
						{
							sw.WriteLine(encryptedString);
							
						}
					}

					else
					{
						// This text is always added, making the file longer over time
						
						using (StreamWriter sw = File.AppendText(path))
						{
							sw.WriteLine(encryptedString);
							
						}
					}
				}

				catch (Exception e)
				{
					throw new FaultException<SecurityException>(new SecurityException(e.Message));
				}

				
			}

		}

		//Decrpts the data from the file and writes it on the console for the current user.
        public string ReadFile(string fileName,string token)
        {

			IIdentity identity = Thread.CurrentPrincipal.Identity;
			WindowsIdentity windowsIdentity = (WindowsIdentity)identity;
			Console.WriteLine("Ime klijenta koji je pozvao metodu 'ReadFile' : " + windowsIdentity.Name);

			using (windowsIdentity.Impersonate())
			{
				Console.WriteLine("Process idenitniy :  " + WindowsIdentity.GetCurrent().Name);
				string a = windowsIdentity.Name;
				string[] b = a.Split('\\');
				string message = "";
				string st = "";
				string path = fileName;

				//Read all lines from the file
				string[] lines = System.IO.File.ReadAllLines(path);

				// Display the file contents by using a foreach loop.
				foreach (string line in lines)
				{
					
					st = Crypto(line, token, iv, "decrypt");
					message += '\n' + st;
				}

				return message;
			}
		}

		
		//3DES implementation
		public string Crypto(string text, string key, string iv, string type)
		{
			byte[] results;
			System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

			MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
			TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();

			// MD5 the key
			byte[] tdeskey = hashProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(key)); //hashProvider.ComputeHash(UTF8.GetBytes(key));

			// Set Key
			tripDES.Key = tdeskey;

			// Set IV
			tripDES.IV = UTF8.GetBytes(iv);

			// Use CBC for mode
			tripDES.Mode = CipherMode.CBC;

			// Zero Padding
			tripDES.Padding = PaddingMode.Zeros;

			byte[] data = null;
			ICryptoTransform enc = null;

			switch (type)
			{
				case "encrypt":
					enc = tripDES.CreateEncryptor();

					data = UTF8.GetBytes(text);
					break;
				case "decrypt":
					enc = tripDES.CreateDecryptor();
					data = Convert.FromBase64String(text);
					break;
			}

			try
			{
				results = enc.TransformFinalBlock(data, 0, data.Length);
			}
			finally
			{
				tripDES.Clear();
				hashProvider.Clear();
			}

			if (type == "decrypt")
				return UTF8.GetString(results);
			else
			{
				return Convert.ToBase64String(results);
			}

		}


		//Challenge-Response authentication between the client and the server implemented
        public string ChallengeResponse(string message,string token)
        {
			string decryptedClientMessage = Crypto(message,token, iv, "decrypt");
            Console.WriteLine("Client sent : " + decryptedClientMessage +" : " + message);
			string messageToSend = Crypto("Dobro sam.", token, iv, "encrypt");
            Console.WriteLine("Sent to client : " +"Dobro sam : " +  messageToSend);
			return messageToSend;
		}

        
    }
}
