using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using MyProject.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models.DTOs
{
    public class StudentDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClassId { get; set; }
        public int Class { get; set; }
        public Gender Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public int? Age { get; set; }
        public string? MamPhone { get; set; }
        public string? DadPhone { get; set; }
        public string? StudPhone { get; set; }
        public int NumberOfAbsences { get; set; }
        public string? Notes { get; set; }
        public State State { get; set; } = 0;
        public string? ProfileImage { get; set; }
        public List<Absence> Absences { get; set; } = new List<Absence>();
        public bool? LastAttendance { get; set; }

    }
}
