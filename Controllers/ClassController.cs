using Microsoft.AspNetCore.Mvc;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;
using MyProject.Services.IServices;

namespace MyProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServantService _servantService;
        public ClassController(IUnitOfWork unitOfWork, IServantService servantService)
        {
            _unitOfWork = unitOfWork;
            _servantService = servantService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            if (classes == null)
            {
                return NotFound();
            }
            return Ok(classes);
        }

        [HttpGet("get-all-numbers")]
        public async Task<IActionResult> GetAllNumbers()
        {
            var classesNumbers = await _unitOfWork.Classes.GetAllNumbersAsync();
            if (classesNumbers == null)
            {
                return NotFound();
            }
            return Ok(classesNumbers);
        }

        [HttpGet("get-capacity")]
        public async Task<IActionResult> GetCapacity(string servantId)
        {
            var classes = await _servantService.GetServantClasses(servantId, User.IsInRole("Admin") ? "Admin" : "Servant");
            if (classes == null || classes.Count == 0)
            {
                return NotFound();
            }
            List<ClassCapacityDTO> capacities = new List<ClassCapacityDTO>();

            foreach (var classNumber in classes)
            {
                var capacity = await _unitOfWork.Classes.GetCapacityByNumberAsync(classNumber);
                if (capacity is not null)
                {
                    capacities.Add(capacity);
                }
            }
            
            return Ok(capacities);
        }
    }
}
