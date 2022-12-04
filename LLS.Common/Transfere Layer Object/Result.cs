using LLS.Common.Dto.Logins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.Common.Transfere_Layer_Object
{
    public class Result
    {
        //Constructor that takes (statusCode, Message, Data?)
        public object Data { get; set; }
        public object Message { get; set; }
        public bool Status { get; set; }
        public DateTime ResponceTime { get; set; } = DateTime.Now;

        public static Result AuthToRes(AuthResult authResult)
        {
            return new Result()
            {
                Data = new
                {
                    authResult.Token,
                    authResult.RefreshToken,
                    authResult.Email,
                    authResult.Role,
                    authResult.Idd,
                    authResult.Permissions
                },
                Message = authResult.Error,
                Status = authResult.Status
            };
        }
    }
}
