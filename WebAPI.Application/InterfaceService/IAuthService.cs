using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Application.Payloads.RequestModels.UserRequest;
using WebAPI.Application.Payloads.ResponeModels.DataUser;
using WebAPI.Application.Payloads.Response;
using WebAPI.Domain.Entities;

namespace WebAPI.Application.InterfaceService
{
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseUser>> Register(Request_Register request);
        Task<string> ConfirmRegisterAccount(string confirmCode); 
        Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user);
        Task<ResponseObject<DataResponseLogin>> Login(Request_Login request);

        Task<ResponseObject<DataResponseUser>> ChangePassword(long userId, Request_ChangePassword request);

        Task<string> ForgotPassword(string email);
        Task<string> ConfirmCreateNewPassword(Request_CreatNewPassword request);

    } 
}
