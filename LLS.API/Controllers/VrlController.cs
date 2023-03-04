using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LLS.BLL.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VrlController : ControllerBase
    {
        //public string root = "https://5fdc-193-227-34-5.eu.ngrok.io";

        private readonly VrlRoot vrlRoot;

        public VrlController(IOptionsMonitor<VrlRoot> optionsMonitor)
        {
            vrlRoot = optionsMonitor.CurrentValue;
        }

        [HttpGet]
        public async Task<IActionResult> GetVRLLink(Guid expIdd, string resource)
        {
            var credntials = new Credntails()
            {
                server = "173.2.100.134",
                user = "lenovo",
                password = "nooreldeen"
            };

            string password = credntials.password;
            string url = $"{vrlRoot.value}/myrtille/GetHash.aspx?password={password}";
            
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                var passwordHash = result.Trim();
                Console.WriteLine(passwordHash + "&connect=Connect%21");

                string rootUrl = $"{vrlRoot.value}/myrtille/?__EVENTTARGET=&__EVENTARGUMENT=&server={credntials.server}&user={credntials.user}&passwordHash="+passwordHash +"&connect=Connect%21";
                Console.WriteLine(rootUrl);
                return Ok(new
                {
                    url = rootUrl
                });

            } catch(Exception e)
            {
                return Ok(new
                {
                    url = "",
                    error = "VRL Server is dwon"
                });
            }
        }


        public class Credntails
        {
            public string server { get; set; }
            public string domain { get; set; }
            public string user { get; set; }
            public string password { get; set; }
            public string program { get; set; }
            public string width { get; set; }
            public string height { get; set; }
        }
    }
}
