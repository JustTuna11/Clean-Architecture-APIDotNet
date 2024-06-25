using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Application.Handle.HandleEmail;

namespace WebAPI.Application.InterfaceService
{
    public interface IEmailService
    {
        string SendEmail(EmailMessage emailMessage);
    }
}
