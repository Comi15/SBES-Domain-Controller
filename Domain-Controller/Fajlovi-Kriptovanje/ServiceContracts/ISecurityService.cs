using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	[ServiceContract]
	public interface ISecurityService
	{
	
		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		void CreateFile(string fileName,string input);

		[OperationContract]
		string ReadFile();

		 string Crypto(string text, string key, string iv, string type);

		[OperationContract]
		string ComputeSHA256(string s);

		[OperationContract]

		void Register(string password);


		[OperationContract]

		bool LogIn(string password);


	}
}
