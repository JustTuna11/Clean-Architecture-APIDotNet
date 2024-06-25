using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Application.Payloads.RequestModels.UserRequest
{
    public class Request_CreatNewPassword
    {
        public string ConfirmCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
