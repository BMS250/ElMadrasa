using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class OtpVerification
    {
        public string Id { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(10)]
        public string OtpCode { get; set; }
    }
}
