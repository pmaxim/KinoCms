using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Film
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Src { get; set; }
    }
}
