using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;
using System.Text.Json;

[Route("[controller]")]
[ApiController]
public class AbsencesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AbsencesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<Absence>>> GetAbsences()
    //{
    //    var absences = await _unitOfWork.Absences.GetAbsencesWithStudentsAsync();
    //    return Ok(absences);
    //}

    [HttpGet("{servantId}")]
    public async Task<ActionResult<IEnumerable<Absence>>> GetAbsences(string servantId)
    {
        var absences = await _unitOfWork.Absences.GetAbsencesWithStudentsAndServantIdAsync(servantId);
        return Ok(absences);
    }

    [HttpGet("Absence/{id}")]
    public async Task<ActionResult<Absence>> GetAbsence(string id)
    {
        var absence = await _unitOfWork.Absences.GetAbsenceWithStudentAsync(id);
        if (absence == null) return NotFound();
        return Ok(absence);
    }

    [HttpGet("Student/Id/{sId}")]
    public async Task<ActionResult<IEnumerable<Absence>>> GetStudentAbsenceById(string sId)
    {
        var absences = await _unitOfWork.Absences.GetStudentAbsencesByIdAsync(sId);
        return Ok(absences);
    }

    [HttpGet("Student/Name/{sName}")]
    public async Task<ActionResult<IEnumerable<Absence>>> GetStudentAbsenceByName(string sName)
    {
        var absences = await _unitOfWork.Absences.GetStudentAbsencesByNameAsync(sName);
        return Ok(absences);
    }

    [HttpGet("Class/{classNumber}")]
    public async Task<ActionResult<IEnumerable<Student>>> GetClassAbsence(int classNumber)
    {
        var studentsWithAbsences = await _unitOfWork.Absences.GetClassWithAbsenceStatusAsync(classNumber);
        return Ok(studentsWithAbsences);
    }

    [HttpPost]
    public async Task<ActionResult<Absence>> PostAbsence([FromBody] Absence absence)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(absence.StudentId);
        if (student == null)
            return NotFound($"Student with ID {absence.StudentId} not found.");

        absence.AlhanAttendant = absence.CopticAttendant = absence.TacsAttendant = student.State == State.عادى;
        absence.Student = student;
        await _unitOfWork.Absences.AddAsync(absence);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            var obj = new { message = $"Error saving absence: {ex.Message}" };
            string json = JsonSerializer.Serialize(obj);
            return StatusCode(StatusCodes.Status500InternalServerError, json);
        }

        return CreatedAtAction(nameof(GetAbsence), new { id = absence.Id }, absence);
    }

    [HttpPost("add-student-attendance/{studentId}")]
    public async Task<ActionResult> AddStudentAttendance(string studentId)
    {
        var IsStudentExist = await _unitOfWork.Students.ExistsAsync(studentId);
        if (!IsStudentExist)
        {
            return NotFound($"Student with ID {studentId} not found.");
        }

        try
        {
            await _unitOfWork.KorasAbsences.InsertKorasAbsence(studentId);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding student attendance.");
        }
        return Ok(new { message = "Student attendance added successfully." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAbsence(string id, AbsenceDTO absenceDTO)
    {
        if (id != absenceDTO.Id) return BadRequest();

        Absence absence = await _unitOfWork.Absences.GetByIdAsync(id);
        if (absence == null) return NotFound();
        var classId = absenceDTO.Student?.ClassId ?? await _unitOfWork.Students.GetClassIdByStudentIdAsync(absenceDTO.StudentId);
        if (classId == null) return BadRequest("The student is not assigned to any class.");
        var subject = await _unitOfWork.ServantClasses.GetSubjectAsync(absenceDTO.ServantId, classId);
        if (subject == -1) return BadRequest("The servant is not assigned to this class.");
        absence.AbsenceDate = absenceDTO.AbsenceDate;
        absence.AbsenceReason = absenceDTO.AbsenceReason;
        absence.StudentId = absenceDTO.StudentId;
        if (subject == 0)
        {
            absence.AlhanAttendant = absenceDTO.Attendant;
        }
        else if (subject == 1)
        {
            absence.CopticAttendant = absenceDTO.Attendant;
        }
        else if (subject == 2)
        {
            absence.TacsAttendant = absenceDTO.Attendant;
        }
        else
        {
            return BadRequest("Invalid subject.");
        }
        _unitOfWork.Absences.Update(absence);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

        //return NoContent();
        return Ok(new { message = "the data is updated successfully"});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAbsence(string id)
    {
        var absence = await _unitOfWork.Absences.GetByIdAsync(id);
        if (absence == null) return NotFound();

        _unitOfWork.Absences.Remove(absence);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
