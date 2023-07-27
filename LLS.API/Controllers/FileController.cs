using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace LLS.API.Controllers
{
    public class FileController : BaseController
    {
        private static IWebHostEnvironment _enviroment;
        public FileController(IWebHostEnvironment enviroment)
        {
            _enviroment = enviroment;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Policy = "AddDeleteEdit_Exp")]
        [HttpPost("Image")]
        public IActionResult UploadImage(IFormFile imageUpload)
        {
            var objFile = imageUpload;

            string host = HttpContext.Request.Host.Value;

            if (objFile == null || objFile.Length > 50000000000)
            {
                return BadRequest(new
                {
                    uploaded = false,
                    url = ""
                });
            }

            if (!Directory.Exists(_enviroment.WebRootPath + "/Images/"))
            {
                Directory.CreateDirectory(_enviroment.WebRootPath + "/Images/");
            }

            string baseUrl = _enviroment.WebRootPath + "/Images/";

            string fileName = objFile.FileName;

            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            string newFilePath = baseUrl + newFileName;

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                objFile.CopyTo(fileStream);
            }

            var imageUrl = "https://" + host + "/Images/" + newFileName;

            return Ok(new
            {
                uploaded = true,
                url = imageUrl
            });
        }

        [HttpPost("File")]
        public IActionResult FileUpload(IFormFile imageUpload)
        {
            var objFile = imageUpload;

            string host = HttpContext.Request.Host.Value;


            if (objFile == null || objFile.Length > 100000000000)
            {
                return BadRequest(new
                {
                    uploaded = false,
                    url = ""
                });
            }

            if (!Directory.Exists(_enviroment.WebRootPath + "/Files/"))
            {
                Directory.CreateDirectory(_enviroment.WebRootPath + "/Files/");
            }

            string baseUrl = _enviroment.WebRootPath + "/Files/";

            string fileName = objFile.FileName;

            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            string newFilePath = baseUrl + newFileName;

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                objFile.CopyTo(fileStream);
            }

            var imageUrl = "https://" + host + "/Files/" + newFileName;

            return Ok(new
            {
                uploaded = true,
                url = imageUrl
            });
        }
    }
}
