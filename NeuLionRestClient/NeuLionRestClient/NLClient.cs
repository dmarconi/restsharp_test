using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;


namespace NeuLionRestClient
{
    #region XML Deserialiser Classes

    public class Jobs
    {
        public List<Job> jobs { get; set; }
    }

    public class Job
    {
        public int id { get; set; }
        public string name { get; set; }
        public string templateName { get; set; }
        public int selectChannelId { get; set; }
        public int liveChannelId { get; set; }
        public bool canStart { get; set; }
        public bool isEncoding { get; set; }
        public bool bEnable { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public float frameRate { get; set; }
        public int channelCount { get; set; }
        public int bitsPerSample { get; set; }
        public int sampleRate { get; set; }
        public int vodMode { get; set; }
        public int vodDuration { get; set; }
    }

    #endregion

    class NLClient
    {
        string xauth_key = string.Empty;

        string xauth_user = string.Empty;
        string xauth_apikey = string.Empty;
        string xauth_time = string.Empty;

        public NLClient()
        {
            xauth_user = "admin";
            xauth_apikey = "ED8A21DBD55449559B23B15C75B71BCF";
        }

        #region Utility functions

        private Int32 getUnixTimeStamp() {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        static string GetMd5Hash(MD5 md5, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Generate MD5 hash according to the following recipe
        // xauth_key = md5(xauth_apikey + md5(xauth_user + xauth_apikey + xauth_time)) 

        //NOTES
        // 1) Parameters should be entered as string. 
        // 2) The letters in the MD5 hash should be uppercase.  
        // 3) current_unix_time should be the same as in the xauth_time header field.

        static string GetFinalMd5Hash(MD5 md5, string apikey, string user, string time)
        {
            string token = string.Empty;
            string token_tmp = string.Empty;

            token_tmp = GetMd5Hash(md5, user + apikey.ToUpper() + time);

            token = GetMd5Hash(md5, apikey.ToUpper() + token_tmp.ToUpper());

            return token.ToUpper();
        }

        // Verify if MD5 hash is generated according to the 
        // REST API recipe (DEBUG).

        [Conditional("DEBUG")]
        private void GenAuthKeyVerify(MD5 md5) {

            Debug.WriteLine("START OF ---- XAuth token generation test");
            Debug.Indent();   

            string token = string.Empty;
            string token_tmp = string.Empty;

            string targethash = "A4EC32A418AF370828183CE141C6687C";

            string apikey = "234F3B991AB142A4B64A15BCB82EF97E";
            string user = "admin";
            string time = "1454734244";

            Debug.WriteLine(string.Format("for API key: {0}", apikey));
            Debug.WriteLine(string.Format("for Login name: {0}", user));
            Debug.WriteLine(string.Format("for Current Unix Time: {0}", time));

            token = GetFinalMd5Hash(md5, apikey, user, time);

            Debug.WriteLine(string.Format("Resulting Hash: {0}", token));
            Debug.WriteLine(string.Format("Target Hash: {0}", targethash));

            if (token == targethash) {
                Debug.WriteLine("The hash was correctly generated");
            }
            else
            {
                Debug.WriteLine("WARNING: The hash is different from target");
            }

            Debug.Unindent();
            Debug.WriteLine("END OF ---- XAuth token generation test");
        }

        // Generate xauth_key according to the 
        // NeuLion rest API

        public void GenAuthKey()
        {
            string token = string.Empty;
            string token_tmp = string.Empty;

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            Debug.WriteLine("START OF ---- XAuth token generation");
            Debug.Indent();

            GenAuthKeyVerify(md5);

            xauth_time = getUnixTimeStamp().ToString();

            Debug.WriteLine(string.Format("API key: {0}", xauth_apikey));
            Debug.WriteLine(string.Format("Login name: {0}", xauth_user));
            Debug.WriteLine(string.Format("Current Unix Time: {0}", xauth_time));


            xauth_key = GetFinalMd5Hash(md5, xauth_apikey, xauth_user, xauth_time);

            Debug.WriteLine(string.Format("Final MD5 hash: {0}", xauth_key));

            Debug.Unindent();
            Debug.WriteLine("END OF ---- XAuth token generation");

        }

        #endregion

        public void makeGetRequest() {

            Debug.WriteLine("START OF ---- REST Client Get Request");
            Debug.Indent();

            var client = new RestClient("http://127.0.0.1:8080");

            

            //var request = new RestRequest("rest/api_version", Method.GET);
            //var request = new RestRequest("jobs/3", Method.GET);
            //var request = new RestRequest("jobs/3", Method.GET);
            var request = new RestRequest("rest/jobs", Method.GET);
            //var request = new RestRequest("jobs/3/start", Method.PUT);

            //var request = new RestRequest("rest/presets", Method.GET);

            GenAuthKey();

            // easily add HTTP Headers

            request.AddHeader("X-Auth-User", xauth_user);
            request.AddHeader("X-Auth-Time", xauth_time);
            request.AddHeader("X-Auth-Key", xauth_key);
            request.AddHeader("Accept", "application/vnd.mainconcept.neuencoder-2.7+xml");

            // execute the request
            IRestResponse response = client.Execute(request);

            var content = response.Content; // raw content as string

            Debug.WriteLine("---------RESPONSE CONTENT---------");
            Debug.WriteLine(content);
            Debug.WriteLine("---------END RESPONSE CONTENT---------");

            RestSharp.Deserializers.XmlDeserializer deserial = new XmlDeserializer();

            Jobs x = deserial.Deserialize<Jobs>(response);

            string jobname = x.jobs[0].name;


            Debug.WriteLine(jobname);

            Debug.Unindent();
            Debug.WriteLine("END OF ---- REST Client Get Request");

        }
    }
}
