using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models.DTOs
{
    public class AccountVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        public bool RememberMe { get; set; } = false;

        // Navigation property for many-to-many relationship
        public ICollection<ServantClass> ServantClasses { get; set; }

    }
}
