using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Application.Payloads.ResponeModels.DataUser;
using WebAPI.Domain.Entities;

namespace WebAPI.Application.Payloads.Mappers
{
    public class UserConverter
    {
        public DataResponseUser EntityToDTO(User user)
        {
            return new DataResponseUser
            {
                Avatar = user.Avatar,
                CreateTime = user.CreateTime,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FullName = user.FullName,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                UpdateTime = user.UpdateTime,
                UserStatus = user.UserStatus.ToString()
            };
        }
    }
}
