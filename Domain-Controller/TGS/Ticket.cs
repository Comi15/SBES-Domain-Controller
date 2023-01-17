using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TGS
{
    public class Ticket:ITicket
    {
        static Dictionary<string, Services> services = new Dictionary<string, Services>();
 
        static int port = 9999;


        /// <summary>
        /// Function that creates a random string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
             Random random = new Random();
             const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
             return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        /// <summary>
        /// Adds the started service to the "DNS" table and returns the session key(token)
        /// </summary>
        /// <param name="servis"></param>
        /// <returns></returns>
        public string AddServiceTotable(Services servis)
        {
            servis.Token = servis.IdentityService + RandomString(16);
            services.Add(servis.IdentityService, servis);
            return servis.Token; 
        }


        //Checks if the servers is avaliable to connect with the client
        public Services ConnectionWithTheService()
        {
            foreach (Services s in services.Values)
            {
                
                
                    if (s.Busy == false)
                    {
                        s.Busy = true;                      
                        return s;                    
                    }                

            }
            
            return null;
        }


        //Gets the dynamic port for the server
        public int getDynamicPort()
        {
            return port++;
        }


        /// <summary>
        /// Used to free the server when the client logs out or when the client app is closed
        /// </summary>
        /// <param name="IdentityService"></param>
        public void UnSetBusy(string IdentityService)
        {
            services[IdentityService].Busy = false;
        }


        /// <summary>
        /// Used to delete the server from the DNS table when it closes.
        /// </summary>
        /// <param name="IdentityService"></param>
        public void Delete(string IdentityService)
        {
            services.Remove(IdentityService);
        }
    }
}
