using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class RequestMappingProfile : Profile
    {
        public RequestMappingProfile()
        {
            CreateMap<ChatDto, Chat>();
        }
    }
}
