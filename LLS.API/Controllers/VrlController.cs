using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private VrlRoot vrlRoot;

        public VrlController(IOptionsMonitor<VrlRoot> optionsMonitor)
        {
            vrlRoot = optionsMonitor.CurrentValue;
        }

        [HttpPost("set-vrl-link-BACKEND-ONLY")]
        public IActionResult setVRL([FromBody] string url)
        {
            vrlRoot.value = url;

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetVRLLink(Guid expIdd, string resource)
        {
            var pc_03 = new Credntails()
            {
                server = "173.2.100.133",
                user = "LLMS-03",
                password = "LLMS-03"
            };

            var pc_01 = new Credntails()
            {
                server = "173.2.100.131",
                user = "LLMS-01",
                password = "LLMS-01"
            };

            var pc_10 = new Credntails()
            {
                server = "173.2.100.121",
                user = "LLMS-10",
                password = "LLMS-10"
            };

            var pc_11 = new Credntails()
            {
                server = "173.2.100.122",
                user = "LLMS-11",
                password = "LLMS-11"
            };

            var pc_06 = new Credntails()
            {
                server = "173.2.100.136",
                user = "LLMS-06",
                password = "LLMS-06"
            };

            Credntails credntials;

            if(String.IsNullOrEmpty(resource))
            {
                credntials = pc_11;
            }
            else if (resource == "1" || resource == "matlab")
            {
                credntials = pc_11;
            }
            else if (resource == "2" || resource == "labview")
            {
                credntials = pc_01;
            }
            else if (resource == "3" || resource == "multisim")
            {
                credntials = pc_10;
            }
            else if (resource == "4" || resource == "packet_tracer")
            {
                credntials = pc_03;
            }
            else if (resource == "5" || resource == "remote_controller_lab")
            {
                credntials = pc_06;
            }
            else if(resource == "6" || resource == "emona")
            {
                return Ok(new { url = "http://60.242.163.58:2525/login/helwanueg" });
            }else
            {
                credntials = pc_11;
            }

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

        [HttpGet("resources")]
        public async Task<IActionResult> GetRes()
        {

            var resources = new List<res>();

            resources.Add(new res()
            {
                id = 1,
                name = "matlab"
            });

            resources.Add(new res()
            {
                id = 2,
                name = "labview"
            });

            resources.Add(new res()
            {
                id = 3,
                name = "multisim"
            });

            resources.Add(new res()
            {
                id = 4,
                name = "packet_tracer"
            });

            resources.Add(new res()
            {
                id = 5,
                name = "remote_controller_lab"
            });

            resources.Add(new res()
            {
                id = 6,
                name = "emona"
            });

            return Ok(new
            {
                resources
            });
        }

        public class res
        {
            public int id { get; set; }
            public string name { get; set; }
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
