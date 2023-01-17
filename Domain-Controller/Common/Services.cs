using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Services
    {
        string ipAdress = string.Empty;
        string hostname = string.Empty;
        string identityService = string.Empty;
        string token = string.Empty;

        bool busy;

        public Services()
        {

        }
        public Services(string _ipAdress, string _hostname, string _identityService, bool _busy = false)
        {
            this.IpAdress = _ipAdress;
            this.Hostname = _hostname;
            this.IdentityService = _identityService;
            this.busy = _busy;
             
        }
        [DataMember]
        public string IpAdress { get => ipAdress; set => ipAdress = value; }
        [DataMember]
        public string Hostname { get => hostname; set => hostname = value; }
        [DataMember]
        public string IdentityService { get => identityService; set => identityService = value; }

        [DataMember]
        public bool Busy { get => busy; set => busy = value; }

        [DataMember]
        public string Token { get => token; set => token = value; }

    }
}
