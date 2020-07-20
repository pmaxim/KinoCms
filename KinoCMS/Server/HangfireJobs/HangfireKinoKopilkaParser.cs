using System.Linq;
using AutoMapper;
using Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Threading.Tasks;
using Domain.Entities;
using HtmlAgilityPack;
using KinoCMS.Server.Lib;
using KinoCMS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoCMS.Server.HangfireJobs
{
    public interface IHangfireKinoKopilkaParser
    {
        Task Run();
    }

    public class HangfireKinoKopilkaParser : IHangfireKinoKopilkaParser
    {
        private readonly IFilmRepository _repoFilm;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public HangfireKinoKopilkaParser(IFilmRepository repoFilm,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _repoFilm = repoFilm;
            _mapper = mapper;
            _env = env;
        }
        public async Task Run()
        {
            var f = Ping();
            if (!f) return;
            var t = await HttpLib.Get(Constants.BaseUrl);
            if(string.IsNullOrEmpty(t)) return;
            await ParseFilm(t);
        }

        private async Task ParseFilm(string s)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(s);
            var section = doc.DocumentNode.SelectSingleNode("//div[@id='catalog-content']");
            var films = section.SelectNodes("//div[@class='movie']");
            foreach (var p in films)
            {
                var src = p.Descendants("img").First().GetAttributeValue("src", string.Empty);
                var name = p.Descendants("span").First().InnerText;
                if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(name)) continue;
                var f = await _repoFilm.Films
                    .Where(z => z.Name == name).AnyAsync();
                if(f) continue;
                _repoFilm.Create(new Film
                {
                    Name = name, Src = src
                });
                await _repoFilm.SaveChangesAsync();
            }
        }

        private static bool Ping()
        {
            var request = (HttpWebRequest)WebRequest.Create(Constants.Domain);
            request.AllowAutoRedirect = false;
            request.Timeout = 3000;
            request.Method = "HEAD";
            try
            {
                using var response = request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
