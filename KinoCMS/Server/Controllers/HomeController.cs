using System;
using System.Linq;
using AutoMapper;
using Domain.Repositories;
using KinoCMS.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hangfire;
using KinoCMS.Server.HangfireJobs;
using Microsoft.EntityFrameworkCore;

namespace KinoCMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IFilmRepository _repoFilm;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public HomeController(IFilmRepository repoFilm,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _repoFilm = repoFilm;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet("[action]")]
        public async Task<FilmModelView> GetTable()
        {
            var m = new FilmModelView();
            var list = await _repoFilm.Films.AsNoTracking().OrderByDescending(z => z.Id).ToListAsync();
            if (list == null)
            {
                BackgroundJob.Enqueue<IHangfireKinoKopilkaParser>(z => z.Run());
                return m;
            }
            foreach (var p in list)
            {
                m.List.Add(_mapper.Map<FilmItem>(p));
            }
            BackgroundJob.Enqueue<IHangfireKinoKopilkaParser>(z => z.Run());
            return m;
        }
    }
}
