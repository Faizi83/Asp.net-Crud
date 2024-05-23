using System.ComponentModel.DataAnnotations;

namespace crud.Models
{
    public class prodinfo
    {
        [Key]
        public int Id { get; set; }
        public string ProdName { get; set; }
        public string ProdPrice { get; set; }
        public string ProdImageUrl { get; set; } // Update property name to match column name
    }
}
