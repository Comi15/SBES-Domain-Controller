using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface ITicket
    {
        [OperationContract]
        string AddServiceTotable(Services servis);
        [OperationContract]
        Services ConnectionWithTheService();

        [OperationContract]
        void UnSetBusy(string IdentityService);

        [OperationContract]
        int getDynamicPort();

        [OperationContract]

        void Delete(string IdentityService);
        
        

    }
}
