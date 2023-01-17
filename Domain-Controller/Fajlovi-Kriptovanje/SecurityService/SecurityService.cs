using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecurityService
{
	public class SecurityService : ISecurityService
	{
		public string key = "cEvu4MHkqz7mQgeqmB6mQEXi";
	    public string iv = "jvz8bUAx";
		public static Dictionary<string, User> UserAccountsDB = new Dictionary<string, User>();

	

        public void CreateFile(string fileName,string input)
        {
			
			string encryptedString = "";
			IIdentity identity = Thread.CurrentPrincipal.Identity;
			WindowsIdentity windowsIdentity = (WindowsIdentity)identity;
			Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);
			Console.WriteLine("Ime klijenta koji je pozvao metodu 'CreateFile' : " + windowsIdentity.Name);
			Console.WriteLine("Identitet  klijenta koji je pozvao metodu : " + windowsIdentity.User);

			encryptedString = Crypto(input, key, iv, "encrypt");
            Console.WriteLine("Encrypted : " + encryptedString);
			using (windowsIdentity.Impersonate())
			{
                Console.WriteLine("Process idenitniy :  " + WindowsIdentity.GetCurrent().Name);
				string a = windowsIdentity.Name;
				string[] b = a.Split('\\');

				try
				{
					string path = @"E:\sbes vezba 1\Vezba_1_template\Vezba_1\SecurityService\bin\Debug\" + b[1] + ".txt";
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

        public string ReadFile()
        {

			IIdentity identity = Thread.CurrentPrincipal.Identity;
			WindowsIdentity windowsIdentity = (WindowsIdentity)identity;
			Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);
			Console.WriteLine("Ime klijenta koji je pozvao metodu 'ReadFile' : " + windowsIdentity.Name);
			Console.WriteLine("Identitet  klijenta koji je pozvao metodu : " + windowsIdentity.User);

			using (windowsIdentity.Impersonate())
			{
				Console.WriteLine("Process idenitniy :  " + WindowsIdentity.GetCurrent().Name);
				string a = windowsIdentity.Name;
				string[] b = a.Split('\\');
				string message = "";
				string st = "";
				string path = @"E:\sbes vezba 1\Vezba_1_template\Vezba_1\SecurityService\bin\Debug\" + b[1] + ".txt";

				//Read all lines from the file
				string[] lines = System.IO.File.ReadAllLines(path);

				// Display the file contents by using a foreach loop.
				foreach (string line in lines)
				{
					
					st = Crypto(line, key, iv, "decrypt");
					message += '\n' + st;
				}

				return message;
			}
		}

		
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

        public string ComputeSHA256(string s)
        {
			{
				string hash = String.Empty;

				// Initialize a SHA256 hash object
				using (SHA256 sha256 = SHA256.Create())
				{
					// Compute the hash of the given string
					byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

					// Convert the byte array to string format
					foreach (byte b in hashValue)
					{
						hash += $"{b:X2}";
					}
				}

				return hash;
			}
		}

        public void Register(string password)
        {
			string a = ComputeSHA256(password);
			string path = @"C:\Users\Korisnik\Desktop\Sbes_Projekat\Fajlovi-Kriptovanje\passwords.txt";
			// This text is added only once to the file.
			if (!File.Exists(path))
			{
				// Create a file to write to.
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.WriteLine(a);

				}
			}

			else
			{
				// This text is always added, making the file longer over time

				using (StreamWriter sw = File.AppendText(path))
				{
					sw.WriteLine(a);

				}
			}
		}

        public bool LogIn(string password)
        {
			string a = ComputeSHA256(password);
			string path = @"C:\Users\Korisnik\Desktop\Sbes_Projekat\Fajlovi-Kriptovanje\passwords.txt";

			//Read all lines from the file
			string[] lines = System.IO.File.ReadAllLines(path);

			// Display the file contents by using a foreach loop.
			foreach (string line in lines)
			{
				if(line.Equals(a))
                {
					return true;
                }
				
			}

			return false;
		}
    }
}
