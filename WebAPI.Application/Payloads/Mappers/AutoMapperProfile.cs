using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Application.Payloads.RequestModels.UserRequest;
using WebAPI.Application.Payloads.ResponeModels.DataUser;
using WebAPI.Domain.Entities;

namespace WebAPI.Application.Payloads.Mappers
{
    internal class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Request_Register, User>();
            CreateMap<User, DataResponseUser>();

        }
    }
}
