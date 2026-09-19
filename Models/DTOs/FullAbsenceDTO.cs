using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models.DTOs
{
    public class FullAbsenceDTO
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
        public DateOnly AbsenceDate { get; set; }
        public string? AbsenceReason { get; set; }
        public bool AlhanAttendant { get; set; } = true;
        public bool CopticAttendant { get; set; } = true;
        public bool TacsAttendant { get; set; } = true;
        public bool Attendant => AlhanAttendant && CopticAttendant && TacsAttendant;
        public bool? LastAttendance { get; set; }
    }
}
