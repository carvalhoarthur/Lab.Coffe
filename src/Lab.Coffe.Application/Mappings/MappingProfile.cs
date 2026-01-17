using AutoMapper;
using Lab.Coffe.Application.DTOs;
using Lab.Coffe.Domain.Entities;

namespace Lab.Coffe.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Coffee, CoffeeDto>().ReverseMap();
        CreateMap<CreateCoffeeRequest, Coffee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<UpdateCoffeeRequest, Coffee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}
