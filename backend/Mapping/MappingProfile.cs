using API.DTOs;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDTO>().ReverseMap();
    }
}
