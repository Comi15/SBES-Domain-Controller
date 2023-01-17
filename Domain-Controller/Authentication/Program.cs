using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Authentication
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:8888/Authentication";
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;


            ServiceHost host = new ServiceHost(typeof(AuthenticationClass));
            host.AddServiceEndpoint(typeof(IAuthentication), binding, address);

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();
            Console.WriteLine("Authentication service has been started.");
            Console.WriteLine("User that started the service : " + WindowsIdentity.GetCurrent().Name);
            Console.ReadLine();
        }
    }
}
