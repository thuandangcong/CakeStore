using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [DisplayName("Brand Name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 10000)]
        public int DisplayOrder { get; set; }
    }
}
