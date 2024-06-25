using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Entities;
using WebAPI.Domain.Enumerates;

namespace WebAPI.Application.Payloads.RequestModels.UserRequest
{
    public class Request_Register
    {
        [Required(ErrorMessage ="UserName is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "FullName is Required")]
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
