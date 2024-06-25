using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Constants;
using WebAPI.Application.InterfaceService;
using WebAPI.Application.Payloads.RequestModels.UserRequest;
using WebAPI.Application.Payloads.ResponeModels.DataUser;
using WebAPI.Application.Payloads.Response;
namespace WebAPI.Controllers
{
    [Route(Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Request_Register request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmRegisterAccount(string confirmCode)
        {
            return Ok(await _authService.ConfirmRegisterAccount(confirmCode));
        }
        [HttpPost]
        public async Task<IActionResult> Login(Request_Login request)
        {
            return Ok(await _authService.Login(request));
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] Request_ChangePassword request)
        {
            long id = long.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _authService.ChangePassword(id, request));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return Ok(await _authService.ForgotPassword(email));
        }
        [HttpPut]
        public async Task<IActionResult> ConfirmCreateNewPassword(Request_CreatNewPassword request)
        {
            return Ok(await _authService.ConfirmCreateNewPassword(request));
        }
    }
}
