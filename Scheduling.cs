using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyProject
{
    public class Scheduling : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly string _connectionString;

        public Scheduling(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("The ConnectionStrings:DefaultConnection configuration value is required.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Query at: {TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")).Date.AddDays((5 - (int)TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")).DayOfWeek + 7) % 7).AddHours(15)}");
            // Define Egypt's time zone
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            // Get the current time in Egypt time
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, egyptTimeZone);

            // Calculate the next Friday at 3 PM
            var nextFriday = now.Date.AddDays((5 - (int)now.DayOfWeek + 7) % 7).AddHours(15);
            if (nextFriday <= now) // Ensure it's in the future
            {
                nextFriday = nextFriday.AddDays(7);
            }

            // Calculate the delay until the next Friday at 3 PM
            var delay = nextFriday - now;

            // Define the interval as 1 week
            var interval = TimeSpan.FromDays(7);

            // Initialize the timer with the calculated delay and interval
            _timer = new Timer(ExecuteTaskAsync, null, delay, interval);

            return Task.CompletedTask;
        }


        private async void ExecuteTaskAsync(object state)
        {
            try
            {
                string updateQuery = "Insert into [dbo].[Absences] Select ID, GETDATE(), '', 1 from[dbo].[Students]; UPDATE Absences SET Attendant = 0 FROM Absences INNER JOIN Students ON Absences.StudentId = Students.Id WHERE Students.Notes IN(N'اونلاين', N'مؤجل', N'لاغى') and AbsenceDate = GETDATE();";

                await ExecuteSqlNonQueryAsync(updateQuery);


                // Log the result (you can also process it as needed)
                Console.WriteLine($"Query executed at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error executing SQL query: {ex.Message}");
            }
        }

        private async Task ExecuteSqlNonQueryAsync(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer when the service is stopped
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of the timer when the service is disposed
            _timer?.Dispose();
        }
    }
}
