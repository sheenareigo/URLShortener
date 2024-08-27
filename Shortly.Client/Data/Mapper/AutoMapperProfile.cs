
using AutoMapper;
using Shortly.Client.Data.ViewModels;
using Shortly.Data.Models;

namespace Shortly.Client.Data.Mapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() {

            CreateMap<Url, GetUrlVM>()
            .ForMember(dest => dest.UserVM, opt => opt.MapFrom(src => new GetUserVM
            {
                Id = src.User.Id,
                Name = src.User.UserName
            }))
            .ReverseMap();
            CreateMap<User, GetUserVM>().ReverseMap();

        }
    }
}
