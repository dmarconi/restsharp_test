using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuLionRestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            NLClient client = new NLClient();

            client.makeGetRequest();

            Console.ReadKey();
        }
    }
}
