using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QRCoder;
using MyProject.Models.DTOs;

namespace MyProject.Services
{
    

    /// <summary>
    /// ✅ FINAL WORKING VERSION - Two Column Layout
    /// 
    /// Generates student ID cards PDF with QR codes.
    /// Layout: Two columns per page
    /// Each card has: QR code (left) | Name + Class Number (right, right-aligned)
    /// 11 cards per column, auto-paginated
    /// 
    /// Installation:
    ///   dotnet add package QuestPDF
    ///   dotnet add package QRCoder
    /// </summary>

    public class StudentCardPdfService
    {
        private const float CardWidth = 35f;      // mm (about 1.4 inches)
        private const float CardHeight = 21f;     // mm (about 0.8 inches)
        private const float QrSize = 56f;         // mm QR code size
        private const int CardsPerPage = 11;      // Cards per column

        public byte[] GenerateStudentCardsPdf(IEnumerable<StudentCard> students)
        {
            var studentList = students.ToList();

            // Step 1: Pre-generate all QR code images (one-time, reusable)
            var qrCodeCache = new Dictionary<string, byte[]>();

            foreach (var student in studentList)
            {
                try
                {
                    qrCodeCache[student.Id] = GenerateQrCodeImage(student.Id);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to generate QR for {student.Id}: {ex.Message}");
                    // Continue with other students even if one fails
                }
            }

            // Step 2: Create PDF document
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(0.5f, Unit.Centimetre);

                    page.Content()
                        .Column(mainColumn =>
                        {
                            // Calculate how many complete 2-column sets we need
                            int cardsPerColumn = (int)Math.Ceiling((double)studentList.Count / 2);
                            int pageCount = (int)Math.Ceiling((double)cardsPerColumn / CardsPerPage);

                            int studentIndex = 0;

                            // Process each page
                            for (int pageNum = 0; pageNum < pageCount; pageNum++)
                            {
                                if (pageNum > 0)
                                {
                                    mainColumn.Item().PageBreak();
                                }

                                // Create two-column layout for this page
                                mainColumn.Item()
                                    .Row(pageRow =>
                                    {
                                        // Left Column
                                        pageRow.RelativeColumn()
                                            .PaddingRight(2, Unit.Millimetre)
                                            .Column(leftColumn =>
                                            {
                                                int cardsInLeftColumn = 0;
                                                while (studentIndex < studentList.Count && cardsInLeftColumn < CardsPerPage)
                                                {
                                                    var student = studentList[studentIndex];

                                                    leftColumn.Item()
                                                        .Padding(2.5f, Unit.Millimetre)
                                                        .Border(1f)
                                                        .BorderColor(Colors.Grey.Lighten3)
                                                        .Height(CardHeight, Unit.Millimetre)
                                                        .ExtendHorizontal()
                                                        .Row(row =>
                                                        {
                                                            // QR Code (left side)
                                                            if (qrCodeCache.ContainsKey(student.Id))
                                                            {
                                                                row.ConstantColumn(QrSize)
                                                                    .AlignCenter()
                                                                    .AlignMiddle()
                                                                    .Padding(1, Unit.Millimetre)
                                                                    .Image(qrCodeCache[student.Id]);
                                                            }
                                                            else
                                                            {
                                                                row.ConstantColumn(QrSize)
                                                                    .AlignCenter()
                                                                    .AlignMiddle()
                                                                    .Text("ERR")
                                                                    .FontSize(8);
                                                            }

                                                            // Student Info (right side)
                                                            row.RelativeColumn()
                                                                .ExtendHorizontal()
                                                                .AlignMiddle()
                                                                .AlignRight()
                                                                .PaddingRight(1.5f, Unit.Millimetre)
                                                                .Column(info =>
                                                                {
                                                                    info.Item()
                                                                        .AlignRight()
                                                                        .Text(student.Name)
                                                                        .FontSize(8)
                                                                        .SemiBold()
                                                                        .LineHeight(1);

                                                                    info.Item()
                                                                        .AlignRight()
                                                                        .PaddingTop(1, Unit.Millimetre)
                                                                        .Text($"الفصل: {student.ClassNumber}")
                                                                        .FontSize(8)
                                                                        .FontColor(Colors.Grey.Darken2)
                                                                        .LineHeight(1);
                                                                });
                                                        });

                                                    studentIndex++;
                                                    cardsInLeftColumn++;
                                                }
                                            });

                                        // Right Column
                                        pageRow.RelativeColumn()
                                            .PaddingLeft(2, Unit.Millimetre)
                                            .Column(rightColumn =>
                                            {
                                                int cardsInRightColumn = 0;
                                                while (studentIndex < studentList.Count && cardsInRightColumn < CardsPerPage)
                                                {
                                                    var student = studentList[studentIndex];

                                                    rightColumn.Item()
                                                        .Padding(2.5f, Unit.Millimetre)
                                                        .Border(1f)
                                                        .BorderColor(Colors.Grey.Lighten3)
                                                        .Height(CardHeight, Unit.Millimetre)
                                                        .ExtendHorizontal()
                                                        .Row(row =>
                                                        {
                                                            // QR Code (left side)
                                                            if (qrCodeCache.ContainsKey(student.Id))
                                                            {
                                                                row.ConstantColumn(QrSize)
                                                                    .AlignCenter()
                                                                    .AlignMiddle()
                                                                    .Padding(1, Unit.Millimetre)
                                                                    .Image(qrCodeCache[student.Id]);
                                                            }
                                                            else
                                                            {
                                                                row.ConstantColumn(QrSize)
                                                                    .AlignCenter()
                                                                    .AlignMiddle()
                                                                    .Text("ERR")
                                                                    .FontSize(8);
                                                            }

                                                            // Student Info (right side)
                                                            row.RelativeColumn()
                                                                .ExtendHorizontal()
                                                                .AlignMiddle()
                                                                .PaddingRight(0.5f, Unit.Millimetre)   // or even 0
                                                                .Column(info =>
                                                                {
                                                                    info.Item()
                                                                        .AlignRight()
                                                                        .Text(student.Name)
                                                                        .FontSize(8)
                                                                        .SemiBold()
                                                                        .LineHeight(1);

                                                                    info.Item()
                                                                        .AlignRight()
                                                                        .PaddingTop(1, Unit.Millimetre)
                                                                        .Text($"الفصل: {student.ClassNumber}")
                                                                        .FontSize(8)
                                                                        .FontColor(Colors.Grey.Darken2)
                                                                        .LineHeight(1);
                                                                });
                                                        });

                                                    studentIndex++;
                                                    cardsInRightColumn++;
                                                }
                                            });
                                    });
                            }
                        });
                });
            });

            // Step 3: Generate and return PDF bytes
            return document.GeneratePdf();
        }

        /// <summary>
        /// Generates QR code image from student ID.
        /// 
        /// WHAT HAPPENS UNDER THE HOOD:
        /// 1. QRCodeGenerator.CreateQrCode() encodes the data
        ///    - Converts string to binary
        ///    - Adds error correction (30% recovery rate)
        ///    - Creates QR matrix pattern
        /// 
        /// 2. QRCode.GetGraphic(10) renders to bitmap
        ///    - 10 = pixels per module/square in QR code
        ///    - Creates visual pattern that scanners read
        /// 
        /// 3. Save as PNG
        ///    - PNG = compressed image format
        ///    - Smaller file size than BMP
        ///    - Better quality than JPEG for text/patterns
        /// 
        /// 4. Return as byte array
        ///    - Bytes can be embedded in PDF
        ///    - Can be reused multiple times
        /// </summary>
        private byte[] GenerateQrCodeImage(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                // Q = 30% error correction (high)
                // Allows scanning even if code is partially damaged/dirty
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                using (var qrCode = new QRCode(qrCodeData))
                {
                    // 10 = modules per pixel (larger = clearer but bigger file)
                    var qrCodeImage = qrCode.GetGraphic(10);

                    using (var memoryStream = new MemoryStream())
                    {
                        // Save as PNG (compressed)
                        qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                        // Return bytes
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}
