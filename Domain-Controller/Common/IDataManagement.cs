using System.Security;
using System.ServiceModel;

namespace Common
{
	[ServiceContract]
	public interface IDataManagement
	{
	
		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		void CreateFile(string fileName,string input,string token);

		[OperationContract]
		string ReadFile(string fileName,string token);

		[OperationContract]
		string Crypto(string text, string key, string iv, string type);

		[OperationContract]
		string ChallengeResponse(string message,string token);

		
	}
}
