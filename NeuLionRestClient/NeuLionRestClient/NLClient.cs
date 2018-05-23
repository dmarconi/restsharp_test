using System;
using System.Diagnostics;

using RestSharp;
using RestSharp.Authenticators;

namespace NeuLionRestClient
{
    class NLClient
    {
        string xAuthKey = string.Empty;
        string loginName = string.Empty;
        string apiKey = string.Empty;

        public NLClient()
        {
            loginName = "admin";
            apiKey = "234F3B991AB142A4B64A15BCB82EF97E";
        }

        // Generate X-Auth key according to the following recipe
        //  X-Auth-Key = md5(api_key + md5(login_name + api_key + current_unix_time)) 
        public void GenAuthKey()
        {
            string token = string.Empty;
            Int32 unixTimestamp = 0;

            Debug.WriteLine("START OF ---- XAuth token generation");
            Debug.Indent();

            unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Debug.WriteLine(string.Format("API key: {0}", apiKey));
            Debug.WriteLine(string.Format("Login name: {0}", loginName));
            Debug.WriteLine(string.Format("Current Unix Time: {0}", unixTimestamp));

            Debug.Unindent();
            Debug.WriteLine("END OF ---- XAuth token generation");

        }

    }
}
