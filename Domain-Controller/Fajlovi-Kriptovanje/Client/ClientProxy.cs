using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<ISecurityService>, ISecurityService, IDisposable
    {
        ISecurityService factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }


        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();

            //Credentials.Windows.AllowNtlm = false;
        }

        public string ComputeSHA256(string s)
        {
           return factory.ComputeSHA256(s);
        }

        public void CreateFile(string fileName,string input)
        {
            try
            {
                factory.CreateFile(fileName,input);
            }

            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine("Message : " + ex.Message);
            }

            catch(Exception e)
            {
                Console.WriteLine("Message : " + e.Message);

            }
        }

        public string Crypto(string text, string key, string iv, string type)
        {
            throw new NotImplementedException();
        }

        public bool LogIn(string password)
        {
           return factory.LogIn(password);
        }

        public string ReadFile()
        {
            try
            {
               return factory.ReadFile();
            }
            catch(Exception e)
            {
                return "Message : " + e.Message;
                
            }
        }

        public void Register(string password)
        {
            factory.Register(password);
        }
    }
}
