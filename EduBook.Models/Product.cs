using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduBook.Models
{
    // tạo class Product
    public class Product
    {
        // khoá chính Id
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        [Range(1,10000)]
        public double ListPrice { get; set; }
        [Required]
        [Range(1,10000)]
        public double Price { get; set; }
        [Required]
        [Range(1,10000)]
        public double Price50 { get; set; }
        [Required]
        [Range(1,10000)]
        public double Price100 { get; set; }
        public string? ImageUrl { get; set; }
        // khoá ngoại CategoryId -> Category
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        // khoá ngoại CoverTypeId -> CoverType
        [Required]
        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }
        [ForeignKey("CoverTypeId")]
        public CoverType? CoverType { get; set; }
        public int Status { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}