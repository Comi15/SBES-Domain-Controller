using Common;
using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Authentication
{
    public class AuthenticationClass:IAuthentication
    {
        public AuthenticationClass()
        {

        }
        public  MySqlConnection GetConnection()
        {
            string sql = "datasource=127.0.0.1;port=3306;username=root;password=1969;database=sbes_projekat";
            MySqlConnection conn = new MySqlConnection(sql);

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {

                Console.WriteLine(ex.Message);
            }

            return conn;
        }
        /// <summary>
        /// Reads users from the database(used for uthentication)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Read(User user)
        {
            int count = 0;
            string sql = $"SELECT * FROM table_users where username = '{user.Username}' AND passwd = '{ComputeSHA256(user.Password)}'";
            MySqlConnection conn = GetConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {                   
                    count++;
                }
            }
            if(count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes users to the database (used for registering an account)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool WriteData(User data)
        {
            string sql = "INSERT INTO table_users VALUES (NULL,@username, @passwd)";
            MySqlConnection conn = GetConnection();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = data.Username;
            cmd.Parameters.Add("@passwd", MySqlDbType.VarChar).Value = ComputeSHA256(data.Password);

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Added successfully");
                return true;
            }
            catch (MySqlException ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static string ComputeSHA256(string s)
        {
            {
                string hash = String.Empty;

                // Initialize a SHA256 hash object
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Compute the hash of the given string
                    byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                    // Convert the byte array to string format
                    foreach (byte b in hashValue)
                    {
                        hash += $"{b:X2}";
                    }
                }

                return hash;
            }
        }

      
        public bool Authenticated(User user)
        {

            try
            {
                return Read(user);
            }
            catch (MySqlException ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
