using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 10000)]
        public int DisplayOrder { get; set; }
    }
}
