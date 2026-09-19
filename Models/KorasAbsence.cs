using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models;

public partial class KorasAbsence
{
    public string Id { get; set; }
    public string StudentId { get; set; }
    public DateOnly AbsenceDate { get; set; }
    public bool Attendant { get; set; }
}
