using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Application.Handle.HandleEmail;
using WebAPI.Application.InterfaceService;
using WebAPI.Application.Payloads.Mappers;
using WebAPI.Application.Payloads.RequestModels.UserRequest;
using WebAPI.Application.Payloads.ResponeModels.DataUser;
using WebAPI.Application.Payloads.Response;
using WebAPI.Domain.Entities;
using WebAPI.Domain.InterfaceRepositories;
using WebAPI.Domain.Validations;
using WebAPI.Domain.Enumerates;
using BCryptNet = BCrypt.Net.BCrypt;
namespace WebAPI.Application.ImplementService
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository; // chức năng chung cho các bảng trong trường hợp này là của User
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository; // Chức năng chỉ có của thằng User thôi
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<Permission> _basePermissionRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        public AuthService(IBaseRepository<User> baseUserRepository, UserConverter userConverter, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService,
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository, IBaseRepository<Permission> basePermissionRepository, IBaseRepository<Role> baseRoleRepository,
            IBaseRepository<RefreshToken> baseRefreshTokenRepository)
        {
            _baseUserRepository = baseUserRepository;
            _userConverter = userConverter;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _basePermissionRepository = basePermissionRepository;
            _baseRoleRepository = baseRoleRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;

        }
        public AuthService() { }
        public async Task<ResponseObject<DataResponseUser>> Register(Request_Register request)
        {
            try
            {
                if (!ValidateInput.IsValidEmail(request.Email))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Định dạng email không hợp lệ",
                        Data = null
                    };
                }
                if (!ValidateInput.IsValidPhoneNumber(request.PhoneNumber))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Định dạng số điện thoại không hợp lệ",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserbyEmail(request.Email) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email đã bị được đăng kí!",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserbyUserName(request.UserName) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User name đã được sử dụng!",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserbyPhone(request.PhoneNumber) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Số điện thoại đã được đăng kí!",
                        Data = null
                    };
                }
                var user = new User
                {
                    Avatar = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANkAAADoCAMAAABVRrFMAAAAV1BMVEX///" +
                    "+AgIB8fHy3t7d6enra2trv7+/8/PzJycn5+fmHh4eRkZGDg4Pl5eXCwsLFxcWXl5fV1dXt7e2fn5+MjIypqamvr6+cnJzh4eG8vLzX19fQ0NCzs7O" +
                    "m6I+UAAAGhUlEQVR4nO2d2ZLiMAxFsZ19gZAFAvT/f2fHbM0WOgTrykn5PMzDdFdN7tiWZVmWFgsAcblfR9FKKbWKorVfxoh/lJgwWKumFkJKz/Pk" +
                    "+U8h0katg5D748YTHFSRSa3kie5vs0IdJjl44X6TvBR1Ky/Z+FMbuVL9J+sirlYl98d+gF95Q2RdxDU59wcPxN8OGq4bbaLyuT96AHnzoa7zuNk+J" +
                    "2MlPtd11JbtrLYlfj1Ol8ZL7Z2SsfJG6zpqW3Ir6KEsvhPWTcnGyq3bT8bPxKu01MIN4PC9Li0ts07a7tuZeGXPLeWenZERs1Da0qAwq6SZWWNX7Fl" +
                    "rrVlhQtaWGP8gMSusk1ZY4WmFW8NDpqVtuFVpTJrFP2kRt6zO9SDQ1ZGwn2rClGLIukGruJWZ8z0e8Fa8wkqaETsSsCpr6JTx2kefcMiE4HRFKkpl" +
                    "suETRjtkQvAFRkiHjNPy56S6NFzb9Q/xZBRyxyMsNu7jP8F0nImo3I8/vAOLMoLTyyNyyyEsINel4XCxVvSTsZuOHOc0wGTkmY5BBhDWnUDx03GNGLJu0PDBR/Jt+qzsBy0sLCDChIAH6MwHGXuALzTTceFeZAtWtoIpQ0d6NjBl6HAI8aHzRhn4+BmnIGFCpNiTDMw0wo1jCROGDhnkCEf/hIcNO+6ByrCe4xqobA1VBjl2npVht2qzaRJvkdg8M6fMKXsDOOaIVIa1jUhl2LsmpDKsD3KYrXcF9EHAHnGLU5aBfX3cOqvB5zNM7FtTgM/UNUoY+s4CFiLGx64I05IelKGflCiYMqxzBbtkEvicEJjZhydOxCDjyJChBAp/M6TxgLx9ic+XA12gMdzAYxYaSyIgZK9myXTxIQcZjuwkQBIgU0YZYjoyZQECztUJT+ZmWJBn28ITeM7QB7C43iJQb2mM75lIHtXdKON7iUCbYcD6BI3ULWYcMtqVxvngZ0FrHplfepLlSXtML0euUL36seCxOJURQSdsPkPxBJ7vFdMdJcGmJiv2uaghuCUE3yz1Yvy1ODwruhfDZ1CGdxV9xEatiA11Jq4EBg+hVpjFP2JjwXB23+ORsDJjRqyaiidCE4+bZMYTrPqH6HthtTXm/p5va+Z5W0s26Gfi5ovFJi0zig8cRg+bLCydiReCn1GFUmWytKQu2Rvy6uPitlJs2ItADcIvPii0rBdYZU2JvH9pB4+bnMx4XchV+v+C0wXbV9Za+l7i9liDvleVJ5NqmVtxdv6csDyo4ijiRqHuHyBFUu0m3TtAE+f7lWq2dZ15npfUadXsIr+cuCiHw+FwOBwOh8PhcDgcDofD4XA4HA6Hw+FwOByOl4RhHARB3tH6Zw7RhcP5b1r98+7X4tDqFIo4KHN/Hy1V0xRpWtdJlmVSJ7W8Qf+8+7WkrtN022zUKurUlrbkv8Rlu17+dGqSTJy/dlgq2VMK1ul/QSR1p1JFfh6wJTwG+XrZFHUmtZxRat6o1BLT6ifyS6y+OI82RSJMK3qhUCZ1tfMx6XRBq4pk7Jwbp8/L0s2aNgkyzFfb5KN8U2PyPJGqlmhmxq1KBYusq7p645sXl+9S4AzsFSfrH6PJ7/FhOypXnQIpi8jUwAU7nrXVi5coE/YkUBmwju1AvO8TxgOV2KdLI4X6ZpcLV1++TqJEJsvRHqZP1FXbFF46roJIsLHLbrxCNiOm5L62c4Hd49WfllGNYRVev8VTH622krxIlzlk8cEG4NfTEaaN5GBDEk1Jl2ZolUdAp1/TDOuWA+uzZ5Ih73onKWxIefDJrbEL/3Va9Kcq7L++MsBWB+bJ3rzwjSe0QT8j3/RaBHX5paK/fQKuZj4Rsqe6CLCTJRU9VcJhDWPpeF3zEda9mJKXjbcmbRevpM/CgN2xKHl2+3GtbYh56uhxmMVcFM8Fs6btfdzx4In43N9jjoemQLA2S/Tcl55GtpsmR976/Mt5mPwTd5EDXFd3BMmfMGCHcATe34UvcZ18NFLNdDJ2fsgsLaPmah0nG4nr4xqhm9E2feKyWSPaEIE5Rw1a7u8g4FSFeqKR/Heco/yzW2aXSA+qyyOU48l60rH8XvSONvnI8CuO0eLVvNzhE8eTzAxCw88cTchcwnH36IjqLA2IEOEi4P4EIgKy9kPctFO+c3+H3M/lpuIRL5piLtIQOp94MomMnyHVPDfq41YN6nqOZr7KRDW7WOOF1CmbHOlijrECTbLg/gIynLLpsZBzZaHmyi8geowqMskwhgAAAABJRU5ErkJggg==",
                    IsActive = true,
                    CreateTime = DateTime.Now,
                    DateOfBirth = request.DateOfBirth,
                    Email = request.Email,
                    FullName = request.FullName,
                    Password = BCryptNet.HashPassword(request.Password),
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.UserName,
                    UserStatus = Domain.Enumerates.ConstantEnums.UserStatusEnum.UnActivated
                };
                user = await _baseUserRepository.CreateAsync(user);
                await _userRepository.AddRolesToUserAsync(user, new List<string> { "User" });
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    IsActive = true,
                    ComfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.Now.AddMinutes(1),
                    IsComfirmed = false,
                    UserId = user.Id,

                };
                confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                var message = new EmailMessage(new string[] { request.Email }, "Nhận mã xác nhận tại đây: ", $"Mã xác nhận: {confirmEmail.ComfirmCode}");
                var responseMessage = _emailService.SendEmail(message);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Bạn đã gửi yêu cầu đăng kí! Vui lòng nhận mã xác nhận tại email để đăng ký tài khoản",
                    Data = _userConverter.EntityToDTO(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Error: " + ex.Message,
                    Data = null
                };
            }
        }
        private string GenerateCodeActive()
        {
            string str = "QuangTuan_" + DateTime.Now.Ticks.ToString();
            return str;
        }

        public async Task<string> ConfirmRegisterAccount(string confirmCode)
        {
            try
            {

                var code = await _baseConfirmEmailRepository.GetAsync(x => x.ComfirmCode.Equals(confirmCode));
                if (code == null)
                {
                    return "Mã xác nhận không hợp lệ";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if (code.ExpiryTime < DateTime.Now)
                {
                    return "Mã xác nhận đã hết hạn";
                }
                user.UserStatus = Domain.Enumerates.ConstantEnums.UserStatusEnum.Activate;
                code.IsComfirmed = true;
                await _baseUserRepository.UpdateAsync(user);
                await _baseConfirmEmailRepository.UpdateAsync(code);
                return "Xác nhận đăng ký tài khoản thành công. Từ giờ bạn có thể sử dụng tài khoản này để đăng nhập";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user)
        {
            var permissions = await _basePermissionRepository.GetAllAsync(x => x.UserId == user.Id);
            var roles = await _baseRoleRepository.GetAllAsync();
            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("UserName", user.UserName.ToString()),
                new Claim("Email", user.Email.ToString()),
                new Claim("PhoneNumber", user.PhoneNumber.ToString())
            };
            foreach (var permission in permissions)
            {
                foreach (var role in roles)
                {
                    if (role.Id == permission.RoleId)
                    {
                        authClaims.Add(new Claim("Permission", role.RoleName));
                    }
                }
            }
            var userRoles = await _userRepository.GetRolesOfUserAsync(user);
            foreach (var item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }
            // Dòng này là đã có được accesstoken rồi nha =)))
            var jwtToken = GetToken(authClaims);
            // Chỗ này là refreshtoken
            var reFreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);
            RefreshToken rf = new RefreshToken
            {
                IsActive = true,
                Expirytime = DateTime.Now.AddHours(refreshTokenValidity),
                UserId = user.Id,
                Token = reFreshToken
            };
            rf = await _baseRefreshTokenRepository.CreateAsync(rf);
            return new ResponseObject<DataResponseLogin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tạo token thành công",
                Data = new DataResponseLogin
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = reFreshToken

                }
            };

        }

        public async Task<ResponseObject<DataResponseLogin>> Login(Request_Login request)
        {
            var user = await _baseUserRepository.GetAsync(x => x.UserName == request.UserName);
            if (user == null)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Sai tài khoản",
                    Data = null
                };
            }
            if (user.UserStatus.ToString().Equals(Domain.Enumerates.ConstantEnums.UserStatusEnum.UnActivated.ToString()))
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Tài khoản chưa được xác thực",
                    Data = null
                };
            }
            bool checkPass = BCryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Mật khẩu không chính xác",
                    Data = null
                };
            }
            return new ResponseObject<DataResponseLogin>
            {
                Status = StatusCodes.Status200OK,
                Message = "Đăng nhập thành công",
                Data = new DataResponseLogin
                {
                    AccessToken = GetJwtTokenAsync(user).Result.Data.AccessToken,
                    RefreshToken = GetJwtTokenAsync(user).Result.Data.RefreshToken
                }
            };
        }

        public async Task<ResponseObject<DataResponseUser>> ChangePassword(long userId, Request_ChangePassword request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(userId);
                bool checkPass = BCryptNet.Verify(request.OldPassword, user.Password);
                if (!checkPass)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu không chính xác",
                        Data = null
                    };
                }
                if (!request.NewPassword.Equals(request.NewPassword))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu không trùng khớp",
                        Data = null
                    };
                }
                user.Password = BCryptNet.HashPassword(user.Password); // cái này là gán lại mk mới nhưng dưới dạng đã mã hóa
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đổi mật khẩu thành công",
                    Data = _userConverter.EntityToDTO(user)
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    Data = null
                };
            }
        }



        public async Task<string> ForgotPassword(string email)
        {
            try
            {
                var user = await _userRepository.GetUserbyEmail(email);
                if (user == null)
                {
                    return "Email này không tồn tại trong hệ thống";
                }
                //var listCpmformCodes = await _baseConfirmEmailRepository.GetAllAsync(x => x.UserId == user.Id);
                //if (listCpmformCodes.ToList().Count() > 0)
                //{
                //    foreach (var confirmCode in listCpmformCodes)
                //    {
                //        await _baseConfirmEmailRepository.DeleteAsync(confirmCode.Id);
                //    }
                //}
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    IsActive = true,
                    ComfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.Now.AddMinutes(1),
                    UserId = user.Id,
                    IsComfirmed = false
                };
                confirmEmail = await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                var message = new EmailMessage(new string[] { user.Email }, "Nhận mã xác nhận tại đây: ", $"Mã xác nhận là: {confirmEmail.ComfirmCode}");
                var send = _emailService.SendEmail(message);
                return "Gửi mã xác nhận về email thành công! Vui lòng kiểm tra Email";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ConfirmCreateNewPassword(Request_CreatNewPassword request)
        {
            try
            {
                var confirmEmail = await _baseConfirmEmailRepository.GetAsync(x => x.ComfirmCode.Equals(request.ConfirmCode));
                if (confirmEmail == null)
                {
                    return "Mã xác nhận không hợp lệ";
                }
                if (confirmEmail.ExpiryTime < DateTime.Now)
                {
                    return "Mã xác nhậ đã hết hạn";
                }
                if (!request.NewPassword.Equals(request.ConfirmPassword))
                {
                    return "Mật khẩu không trùng khớp";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == confirmEmail.UserId);
                user.Password = BCryptNet.HashPassword(request.NewPassword);
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);
                return "Đổi mật khẩu thành công!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Private Method.2
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            _ = int.TryParse(_configuration["JWT:TokenvalidityInHours"], out int tokenValidityInHours);
            var expirationUTC = DateTime.Now.AddDays(tokenValidityInHours);
            //var localTimeZone = TimeZoneInfo.Local;
            //var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeToUtc(expirationUTC, localTimeZone);

            var token = new JwtSecurityToken
                (
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationUTC,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;

        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        #endregion
    }
}
