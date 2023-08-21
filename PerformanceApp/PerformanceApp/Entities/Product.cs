using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerformanceApp.Entities
{
    public class Product
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("productName")]
        public string ProductName { get; set; }
        [Column("productPrice")]
        public float ProductPrice { get; set; }
        [Column("productDescription")]
        public string? ProductDescription { get; set; }
    }
}
