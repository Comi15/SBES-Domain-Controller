using System.Runtime.Serialization;

namespace Common
{
	[DataContract]
	public class User
	{		
		string username = string.Empty;
		string password = string.Empty;

		public User()
		{

		}
		public User(string _username, string _password)
		{
			this.username = _username;
			this.password = _password;
		}

		[DataMember]
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		[DataMember]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}
       
	}
}
