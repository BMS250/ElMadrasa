using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.S3Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _connectionString;

        public S3Service(IConfiguration configuration)
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

            _s3Client = string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey)
                ? new AmazonS3Client(region)
                : new AmazonS3Client(accessKey, secretKey, region);

            _bucketName = configuration["AWS:BucketName"];
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Retrieve File from S3
        public async Task<byte[]> GetStudentImageAsync(string studentName, int classNumber)
        {
            string[] possibleExtensions = { ".jpg", ".jpeg", ".png" };

            foreach (var ext in possibleExtensions)
            {
                string objectKey = $"class_{classNumber}/{studentName}{ext}";

                try
                {
                    var request = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = objectKey
                    };

                    using (var response = await _s3Client.GetObjectAsync(request))
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        await response.ResponseStream.CopyToAsync(memoryStream);
                        return memoryStream.ToArray(); // Return first found file
                    }
                }
                catch (AmazonS3Exception e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        continue; // Try the next extension
                    else
                        throw; // Other errors (e.g., permission issues)
                }
            }

            return null; // No valid image found
        }

        // Get List of Student Names in a Specific Class
        public async Task<List<string>> ListNamesInClassAsync(int classNumber)
        {
            var fileList = new List<string>();

            try
            {
                string classPrefix = $"class_{classNumber}/";

                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = classPrefix,  // Folder path (e.g., "class_10/")
                };

                var response = await _s3Client.ListObjectsV2Async(request);

                // Add Files
                foreach (var obj in response.S3Objects)
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(obj.Key);
                    if (!fileList.Contains(fileName))
                    {
                        fileList.Add(fileName);
                    }
                }

                return fileList;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error listing files: {e.Message}");
                return null;
            }
        }

        // Retrieve Image URLs and Store in SQL Server
        public async Task StoreImageUrlsInDatabaseAsync(int classNumber)
        {
            var imageUrls = await GetImageUrlsAsync(classNumber);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var url in imageUrls)
                {
                    var studentName = ExtractStudentNameFromUrl(url); // Extract name from URL

                    // Update ProfileImage for Students table
                    var updateStudentQuery = "UPDATE Students SET ProfileImage = @url WHERE Name = @studentName";
                    using (var studentCommand = new SqlCommand(updateStudentQuery, connection))
                    {
                        studentCommand.Parameters.AddWithValue("@url", url);
                        studentCommand.Parameters.AddWithValue("@studentName", studentName);
                        await studentCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public string ExtractStudentNameFromUrl(string url)
        {
            var match = Regex.Match(url, @"class_\d+/(.+)\.(jpg|jpeg|png)$");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        // Get Image URLs for a Specific Class
        public async Task<List<string>> GetImageUrlsAsync(int classNumber)
        {
            var imageUrls = new List<string>();
            string classPrefix = $"class_{classNumber}/";

            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = classPrefix
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            foreach (var s3Object in response.S3Objects)
            {
                if (s3Object.Key.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    s3Object.Key.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    s3Object.Key.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    string url = $"https://{_bucketName}.s3.amazonaws.com/{s3Object.Key}";
                    imageUrls.Add(url);
                }
            }

            return imageUrls;
        }


        public async Task<List<string>> GetAllImageUrlsInClassAsync(int classNumber)
        {
            var imageUrls = new List<string>();
            string classPrefix = $"class_{classNumber}/";

            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = classPrefix
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            foreach (var s3Object in response.S3Objects)
            {
                if (s3Object.Key.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    s3Object.Key.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    s3Object.Key.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    string url = $"https://{_bucketName}.s3.amazonaws.com/{s3Object.Key}";
                    imageUrls.Add(url);
                }
            }

            return imageUrls;
        }
    }
}
