using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyProject.Data;

namespace MyProject.Models
{
    public partial class Student
    {
        private MadrasaDbContext? _context;

        // Parameterless constructor for model binding
        public Student() { }

        // Constructor to initialize with DbContext
        public Student(MadrasaDbContext context)
        {
            _context = context;
        }

        public string Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }
        public string ClassId { get; set; }

        [ValidateNever, ForeignKey("ClassId")]
        public Class Class { get; set; }

        public Gender Gender { get; set; }

        public DateOnly? BirthDate { get; set; }

        [ValidateNever]
        public int? Age
        {
            get
            {
                if (BirthDate.HasValue)
                {
                    if (DateTime.Now.Month > BirthDate.Value.Month ||
                        (DateTime.Now.Month == BirthDate.Value.Month && DateTime.Now.Day >= BirthDate.Value.Day))
                    {
                        return DateTime.Now.Year - BirthDate.Value.Year;
                    }
                    else
                    {
                        return DateTime.Now.Year - BirthDate.Value.Year - 1;
                    }
                }
                return null;
            }
        }

        [MaxLength(15)]
        public string? MamPhone { get; set; }

        [MaxLength(15)]
        public string? DadPhone { get; set; }

        [MaxLength(15)]
        public string? StudPhone { get; set; }

        [ValidateNever]
        public int NumberOfAbsences
        {
            get
            {
                if (_context == null)
                {
                    return 0;
                }
                return _context.Absences.Count(a => a.StudentId == Id && !(a.AlhanAttendant && a.CopticAttendant && a.TacsAttendant));
            }
        }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public State State { get; set; } = 0;

        [MaxLength(500)]
        public string? ProfileImage { get; set; }

        public virtual List<Absence> Absences { get; set; } = new List<Absence>();
    }
}
