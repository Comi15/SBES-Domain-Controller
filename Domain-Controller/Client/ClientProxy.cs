using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<IDataManagement>, IDataManagement, IDisposable
    {
        IDataManagement factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }


        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();
        }

        public string ChallengeResponse(string message, string token)
        {
            return factory.ChallengeResponse(message, token);
        }
       

        public void CreateFile(string fileName,string input,string token)
        {
            try
            {
                factory.CreateFile(fileName,input,token);
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
            return factory.Crypto(text, key, iv, type);
        }

        public string ReadFile(string fileName,string token)
        {
            try
            {
               return factory.ReadFile(fileName,token);
            }
            catch(Exception e)
            {
                return "Message : " + e.Message;
                
            }
        }
    }
}
