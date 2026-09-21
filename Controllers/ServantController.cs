using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MyProject.Models;
using MyProject.Repositories.IRepositories;
using MailKit.Net.Smtp;
using System.Text.Json;
using MyProject.Services.IServices;
using MyProject.Models.DTOs;

namespace MyProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServantService _servantService;
        public ServantController(IUnitOfWork unitOfWork, IServantService servantService)
        {
            _unitOfWork = unitOfWork;
            _servantService = servantService;
        }

        [HttpPost("add-phoneNumber")]
        public async Task<IActionResult> AddPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || !_servantService.IsPhoneNumberValid(phoneNumber))
            {
                return BadRequest(new { message = "Invalid phone number." });
            }
            var servantDTO = await _unitOfWork.Servants.GetServantByPhoneNumberAsync(phoneNumber);
            if (servantDTO is null)
            {
                return BadRequest(new { message = "Invalid phone number." });
            }
            if (servantDTO is not null && string.IsNullOrWhiteSpace(servantDTO.UserName))
            {
                return BadRequest(new { message = "This phone number is already used, Please login." });
            }
            IdentityUser fullServant;

            // Create new servant
            fullServant = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = phoneNumber
            };
            await _unitOfWork.Servants.AddAsync(fullServant);

            await _unitOfWork.CompleteAsync();

            return Created();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
        {
            if (registerVM.Password != registerVM.ConfirmPassword)
            {

                return BadRequest(new { message = "Passwords do not match." });
            }

            var servantDTO = await _unitOfWork.Servants.GetServantByPhoneNumberAsync(registerVM.PhoneNumber);
            if (servantDTO is null)
            {
                return BadRequest(new { message = "Invalid phone number." });
            }
            if (!string.IsNullOrWhiteSpace(servantDTO.UserName))
            {
                return BadRequest(new { message = "You have registered before, Please login." });
            }
            IdentityUser fullServant;

            fullServant = await _unitOfWork.Servants.GetByIdAsync(servantDTO.Id);

            if (fullServant == null)
            {
                return BadRequest(new { message = "Servant not found." });
            }

            // Common properties for both new and existing servants
            var hashPassword = new PasswordHasher<IdentityUser>();
            fullServant.PasswordHash = hashPassword.HashPassword(fullServant, registerVM.Password);
            fullServant.UserName = registerVM.Name.Trim();
            fullServant.Email = registerVM.Email.Trim();

            await _unitOfWork.Servants.AddClassSubjectsOfServantAsync(fullServant.Id, registerVM.ServantClasses);

            await _unitOfWork.CompleteAsync();

            return Created($"/servant", registerVM);
        }

        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var servant = await _unitOfWork.Servants.GetServantByPhoneNumberAsync(loginVM.PhoneNumber);
            if (servant is null)
            {
                return BadRequest(new { message = "Invalid phone number." });
            }
            var isValidUser = await _unitOfWork.Servants.CheckServantPasswordAsync(servant.Password, loginVM.Password);
            if (!isValidUser)
            {
                return BadRequest(new { message = "Invalid Phone number or password." });
            }
            return Ok(servant);
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(string email)
        {
            try
            {
                var servantName = await _unitOfWork.Servants.GetServantNameByEmailAsync(email);
                if (servantName is null)
                {
                    return BadRequest(new { message = "Invalid email." });
                }
                var newEmail = new MimeMessage();
                newEmail.From.Add(new MailboxAddress("Bola Milad", "bola.milad25@gmail.com"));
                newEmail.To.Add(new MailboxAddress(servantName, email));
                newEmail.Subject = "Change your password on Manassa Elsamaeien";

                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();

                var otpEntity = new OtpVerification
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    OtpCode = otp
                };

                await _unitOfWork.Otps.AddAsync(otpEntity);
                await _unitOfWork.CompleteAsync();


                newEmail.Body = new TextPart("plain")
                {
                    Text = $"Hello, The next number {otp} is the code that you should enter in the manassa, please enter it to change your password."
                };

                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("bola.milad25@gmail.com", "xfnvifdfrzqrbowp");
                await smtp.SendAsync(newEmail);
                await smtp.DisconnectAsync(true);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error sending email: {ex.Message}" });
            }
        }

        [HttpPost("check-otp")]
        public async Task<IActionResult> CheckOtp(string email, string otp)
        {
            var isValidOtp = await _unitOfWork.Otps.CheckAndUseOtpAsync(email, otp);
            if (!isValidOtp)
            {
                return BadRequest(new { message = "Invalid OTP." });
            }
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return BadRequest(new { message = "Passwords do not match." });
            }
            var result = await _unitOfWork.Servants.ChangePassword(email, password);
            if (!result)
            {
                return BadRequest(new { message = "Error changing password." });
            }
            await _unitOfWork.CompleteAsync();
            return Ok();
        }

        [HttpGet("get-classes")]
        public async Task<IActionResult> GetServantClasses(string servantId)
        {
            var classes = await _servantService.GetServantClasses(servantId, User.IsInRole("Admin") ? "Admin" : "Servant");
            if (classes == null)
            {
                return BadRequest(new { message = "ServantId is empty." });
            }
            if (classes.Count == 0)
            {
                var obj = new { message = "Can not reach the classes of the servant." };
                string json = JsonSerializer.Serialize(obj);
                return NotFound(json);
            }
            return Ok(classes);
        }

        [HttpGet("get-subject")]
        public async Task<IActionResult> GetSubjectAsync(string servantId, string classId)
        {
            if (string.IsNullOrEmpty(servantId) || string.IsNullOrEmpty(classId))
            {
                return BadRequest(new { message = "ServantId and ClassId should not be empty." });
            }
            int subject = await _unitOfWork.ServantClasses.GetSubjectAsync(servantId, classId);
            if (subject == -1)
            {
                return BadRequest(new { message = "Can not reach the subject of the servantClass." });
            }
            return Ok(subject);
        }

        [HttpGet("check-eftikad-of-classes")]
        public async Task<IActionResult> CheckEftikadOfClasses(string servantId)
        {
            if (string.IsNullOrEmpty(servantId))
            {
                return BadRequest(new { message = "ServantId should not be empty." });
            }
            List<int> servantClasses = await _unitOfWork.ServantClasses.GetClassesByServantIdAsync(servantId);
            var hasEftikad = await _unitOfWork.ServantClasses.CheckEftikadOfClassesAsync(servantClasses);
            return Ok(hasEftikad);
        }
    }
}
