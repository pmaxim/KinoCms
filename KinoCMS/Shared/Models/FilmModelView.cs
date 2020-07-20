using System.Collections.Generic;

namespace KinoCMS.Shared.Models
{
    public class FilmModelView
    {
        public List<FilmItem> List { get; set; } = new List<FilmItem>();
    }

    public class FilmItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Src { get; set; }
    }
}
