using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restsharp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string response = string.Empty;

            Console.WriteLine("Hello World!");
            TestRestClient myclient = new TestRestClient();

            response = myclient.makeTestRequest();

            Console.WriteLine(response);

            Console.ReadKey();
        }
    }
}
