using AutoMapper;
using KinoCMS.Server.Lib;
using KinoCMS.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KinoCMS.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFilmRepository _repoFilm;

        public AdminController(IFilmRepository repoFilm,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _repoFilm = repoFilm;
            _mapper = mapper;
            _env = env;
        }

        [HttpPost("[action]")]
        public async Task<List<string>> UploadImageStream()
        {
            var m = new List<string>();
            if (HttpContext.Request.Form.Files.Any())
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var fileName = $"{Guid.NewGuid()}.jpg";
                    var path = Path.Combine(_env.WebRootPath, $"mms/{fileName}");
                    //resize
                    await ImageSize.ResizeImageToFile(path, file);
                    //todo no resize
                    //await using var stream = new FileStream(path, FileMode.Create);
                    //await file.CopyToAsync(stream);
                    m.Add($"{Constants.Domain}/mms/{fileName}");
                }
            }
            return m;
        }

        [HttpPost("[action]")]
        public async Task Update(FilmUpdateModel m)
        {
            var f = await _repoFilm.Films
                .Where(z => z.Name == m.Name).AnyAsync();
            if (f) return;
            _repoFilm.Create(new Film
            {
                Name = m.Name,
                Src = m.Src
            });
            await _repoFilm.SaveChangesAsync();
        }
    }
}
