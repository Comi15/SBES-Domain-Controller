using Common;
using System;
using System.ServiceModel;

namespace Service
{
    public class TicketProxy : ChannelFactory<ITicket>, ITicket, IDisposable
    {
        ITicket factory;
        

        public TicketProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();

        }

        public string AddServiceTotable(Services servis)
        {
            try
            {
                string res = factory.AddServiceTotable(servis);
                return res;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public Services ConnectionWithTheService()
        {
            try
            {
                return factory.ConnectionWithTheService();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public void Delete(string IdentityService)
        {
            factory.Delete(IdentityService);
        }

        public int getDynamicPort()
        {
           return factory.getDynamicPort();
        }

        public void UnSetBusy(string IdentityService)
        {
            factory.UnSetBusy(IdentityService);
        }
    }
}
