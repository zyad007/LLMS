using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("Upload-Image")]
        public IActionResult UploadImage(IFormFile imageUpload)
        {
            var objFile = imageUpload;

            if (objFile == null || objFile.Length > 3000000000)
            {
                return BadRequest(new
                {
                    uploaded = false,
                    url = ""
                });
            }

            string baseUrl = _enviroment.WebRootPath + "/";

            string fileName = objFile.FileName;

            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            string newFilePath = baseUrl + newFileName;

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                objFile.CopyTo(fileStream);
            }

            var imageUrl = "https://lls-api.herokuapp.com/" + newFileName;

            return Ok(new
            {
                uploaded = true,
                url = imageUrl
            });
        }
    }
}
