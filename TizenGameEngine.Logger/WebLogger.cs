using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TizenGameEngine.Logger
{
    public static class WebLogger
    {
        public static void LogAsync(String message, bool encode = false)
        {
            try
            {
                var client = new HttpClient();

                var content = new StringContent($"{{ \"message\":\"{message}\" }}");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                client.PostAsync("http://93.84.105.94:4004/log", content);
            }
            catch (Exception)
            {
                
            }
        }
    }
}

