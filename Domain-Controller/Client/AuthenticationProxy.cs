using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class AuthenticationProxy : ChannelFactory<IAuthentication>, IAuthentication, IDisposable
    {
        IAuthentication factory;

        public AuthenticationProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();

        }
        public bool Authenticated(User user)
        {
            try
            {
                return factory.Authenticated(user);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
