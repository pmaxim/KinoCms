using AutoMapper;
using Domain.Entities;
using KinoCMS.Shared.Models;

namespace KinoCMS.Server.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Film, FilmItem>();
        }
    }
}
