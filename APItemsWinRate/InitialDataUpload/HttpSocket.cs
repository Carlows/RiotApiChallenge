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
            request.Proxy = null;
            try
            {
                request.BeginGetResponse(iar =>
                {
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.EndGetResponse(iar);
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            Console.WriteLine(string.Format("Request {0} arrived succesfully", ++RequestCount));
                            tcs.SetResult(reader.ReadToEnd());
                        }
                    }
                    catch (Exception exc) { tcs.SetResult(""); }
                    finally { if (response != null) response.Close(); }
                }, null);
            }
            catch (Exception exc) { tcs.SetResult(""); }
            return tcs.Task;
        }
    }
}
