using MyProject.Repositories.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class Class
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public int Shift { get; set; }
        public int Level { get; set; }
        // Navigation property for many-to-many relationship
        public ICollection<ServantClass> ServantClasses { get; set; }
    }
}