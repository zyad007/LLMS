using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VrlController : ControllerBase
    {
        string root = "https://bfde-41-33-164-93.eu.ngrok.io";

        [HttpGet]
        public async Task<IActionResult> GetVRLLink(Guid expIdd)
        {
            var credntials = new Credntails()
            {
                server = "173.2.100.133",
                user = "lenovo",
                password = "nooreldeen"
            };

            string password = credntials.password;
            string url = $"{root}/myrtille/GetHash.aspx?password={password}";

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();


            var passwordHash = result;


            string rootUrl = $"{root}/myrtille/?__EVENTTARGET=&__EVENTARGUMENT=&server={credntials.server}&user={credntials.user}&passwordHash={passwordHash}&connect=Connect%21";

            return Ok(rootUrl);
        }

        [HttpPost("TESTING-ONLY")]
        public IActionResult SetRoot(string rootNew)
        {
            root = rootNew;
            return Ok();
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
