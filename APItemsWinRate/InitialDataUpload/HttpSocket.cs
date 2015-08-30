using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InitialDataUpload
{
    public class HttpSocket
    {
        public static int RequestCount = 0;

        public Task<string> GetAsync(string url)
        {
            var tcs = new TaskCompletionSource<string>();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";
            request.Proxy = System.Net.WebRequest.DefaultWebProxy;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 3000;

            try
            {
                request.BeginGetResponse(iar =>
                {
                    HttpWebResponse response = null;
                    try
                    {
                        Console.WriteLine(string.Format("Request {0} arrived succesfully", ++RequestCount));
                        response = (HttpWebResponse)request.EndGetResponse(iar);
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            tcs.SetResult(reader.ReadToEnd());
                        }
                    }
                    catch (Exception exc) 
                    { 
                        tcs.SetResult(""); 
                    }
                    finally { if (response != null) response.Close(); }
                }, null);
            }
            catch (Exception exc) 
            { 
                tcs.SetResult(""); 
            }

            return tcs.Task;
        }
    }
}
