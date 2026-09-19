using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Migrations;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;
using MyProject.S3Services;
using MyProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly S3Service _s3Service;
        private readonly StudentCardPdfService _pdfService;

        public StudentsController(IUnitOfWork unitOfWork, S3Service s3Service, StudentCardPdfService pdfService)
        {
            _unitOfWork = unitOfWork;
            _s3Service = s3Service;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            var students = await _unitOfWork.Students.GetAllWithAbsencesAsync();
            return Ok(students);
        }

        [HttpGet("Id/{id}")]
        public async Task<ActionResult<Student>> GetStudent(string id)
        {
            var student = await _unitOfWork.Students.GetStudentWithAbsencesByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpGet("Name/{name}")]
        public async Task<ActionResult<Student>> GetStudentByName(string name)
        {
            var student = await _unitOfWork.Students.GetStudentByNameAsync(name, true);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpGet("Class/{c}/{servantId}")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetClass(int c, string servantId)
        {
            var studentsAbsences = await _unitOfWork.Students.GetClassStudentsAsync(c, servantId);
            return Ok(studentsAbsences);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromBody] Student student)
        {
            var c = await _unitOfWork.Classes.GetById(student.ClassId);
            if (c == null) return BadRequest("Invalid ClassId");
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(string id, [FromBody] Student student)
        {
            if (id != student.Id) return BadRequest();

            _unitOfWork.Students.Update(student);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return NotFound();

            _unitOfWork.Students.Remove(student);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("student-image/{studentName}")]
        public async Task<IActionResult> GetStudentImage(string studentName)
        {
            var student = await _unitOfWork.Students
                .GetStudentByNameAsync(studentName, includeClass: true);

            if (student == null)
                return NotFound("Student not found in database.");

            var fileBytes = await _s3Service.GetStudentImageAsync(student.Name, student.Class.Number);

            if (fileBytes == null)
                return NotFound("Image not found in S3.");

            return File(fileBytes, "image/jpeg");
        }

        [HttpGet("class-names/{classNumber}")]
        public async Task<IActionResult> GetImageNamesByClass(int classNumber)
        {
            var names = await _s3Service.ListNamesInClassAsync(classNumber);
            if (names == null || names.Count == 0)
                return NotFound("No files found in S3.");

            return Ok(names);
        }

        [HttpGet("class-images/{classNumber}")]
        public async Task<IActionResult> GetClassImageUrls(int classNumber)
        {
            var imageUrls = await _s3Service.GetAllImageUrlsInClassAsync(classNumber);
            if (imageUrls == null || imageUrls.Count == 0)
                return NotFound("No images found in this class.");

            return Ok(imageUrls);
        }

        [HttpGet("students-cards")]
        public async Task<IActionResult> GetStudentsCards()
        {
            var students = await _unitOfWork.Students.GetStudentCardsAsync();
            var studentCards = students.Select(s => new
            {
                s.Id,
                s.Name,
                s.ClassNumber
            });
            return Ok(studentCards);
        }

        [HttpGet("download-student-cards")]
        public async Task<IActionResult> DownloadStudentCards()
        {
            try
            {
                // Step 1: Get students from database
                var students = await _unitOfWork.Students.GetStudentCardsAsync();

                // Step 2: Generate PDF
                var pdfBytes = _pdfService.GenerateStudentCardsPdf(students);

                // Step 3: Return as file download
                return File(
                    pdfBytes,
                    "application/pdf",
                    $"StudentCards_{DateTime.Now:yyyy-MM-dd}.pdf"
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Failed to generate PDF: {ex.Message}" });
            }
        }

        [HttpPut("store-class-images/{classNumber}")]
        public async Task<IActionResult> StoreClassImageUrls(int classNumber)
        {
            try
            {
                var imageUrls = await _s3Service.GetImageUrlsAsync(classNumber);

                foreach (var url in imageUrls)
                {
                    string name = _s3Service.ExtractStudentNameFromUrl(url);
                    var student = await _unitOfWork.Students.GetStudentByNameAsync(name);

                    if (student != null)
                    {
                        student.ProfileImage = url;
                        _unitOfWork.Students.Update(student);
                    }
                }

                await _unitOfWork.CompleteAsync();

                return Ok($"Image URLs for class {classNumber} have been stored in the database.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }

}
