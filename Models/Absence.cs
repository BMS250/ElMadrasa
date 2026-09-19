using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models;

public partial class Absence
{
    public string Id { get; set; }
    public string StudentId { get; set; }
    [ValidateNever, ForeignKey("StudentId")]
    public virtual Student Student { get; set; }
    public DateOnly AbsenceDate { get; set; }
    [MaxLength(500)]
    public string? AbsenceReason { get; set; }
    public bool AlhanAttendant { get; set; } = true;
    public bool CopticAttendant { get; set; } = true;
    public bool TacsAttendant { get; set; } = true;
    public bool Attendant => AlhanAttendant && CopticAttendant && TacsAttendant;
}
