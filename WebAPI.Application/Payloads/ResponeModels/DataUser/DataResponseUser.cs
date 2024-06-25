using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Application.Payloads.ResponeModels.DataUser
{
    public class DataResponseUser : DataReponseBase
    {
 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreateTime { get; set; }
        [MaybeNull]
        public DateTime UpdateTime { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string UserStatus { get; set; }
    }
}
