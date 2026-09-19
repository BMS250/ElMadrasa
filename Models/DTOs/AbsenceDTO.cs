using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models.DTOs
{
    public class AbsenceDTO
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        [ValidateNever, ForeignKey("StudentId")]
        public Student Student { get; set; }
        public int ClassNumber { get; set; }
        public string ServantId { get; set; }
        public DateOnly AbsenceDate { get; set; }
        public string? AbsenceReason { get; set; }
        public bool Attendant { get; set; }
    }
}
