using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();

            Console.WriteLine("Test starting.");

            watch.Start();

            TestMethods.Post(5000);

            watch.Stop();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Test completed in {watch.Elapsed.TotalSeconds} seconds.");
            Console.ReadLine();
        }
    }
    
    public static class TestMethods
    {
        public static bool BypassValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public static void Post(int count)
        {
            Parallel.For(0, count, x =>
            {
                var client = new RestClient("http://localhost:5000/api/account");

                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");

                request.AddParameter("application/json", @"{
	                    ""name"":""Test Account""
                    }", ParameterType.RequestBody);

                ServicePointManager.ServerCertificateValidationCallback += BypassValidateServerCertificate;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                IRestResponse result = null;

                try
                {
                    result = client.Execute(request);
                    Console.WriteLine(result.StatusCode.ToString());
                    Console.WriteLine(result.Content.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }               
            });
        }
    }    
}
