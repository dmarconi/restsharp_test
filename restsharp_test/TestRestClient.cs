using System;
using RestSharp;
using RestSharp.Authenticators;

namespace restsharp_test
{
    public class TestRestClient
    {

        RestClient client;

        public TestRestClient()
        {
            client = new RestClient();
            client.BaseUrl = new Uri("http://dry-cliffs-19849.herokuapp.com/");

        }

        public string makeTestRequest()
        {
            string responsetxt = string.Empty;

            responsetxt = "Zio Banana";

            var request = new RestRequest("users.json", Method.GET);

            // execute the request
            IRestResponse response = client.Execute(request);
            responsetxt = response.Content; // raw content as string

            return responsetxt;
        }
    }

}
