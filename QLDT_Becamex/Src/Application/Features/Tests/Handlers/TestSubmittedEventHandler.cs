using MediatR;
using Microsoft.Extensions.Primitives;
using QLDT_Becamex.Src.Application.Features.Tests.Events;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using SelectPdf;


public class TestSubmittedEventHandler : INotificationHandler<TestSubmittedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly ICloudinaryService _cloudinaryService;


    public TestSubmittedEventHandler(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment env,
        ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _env = env;
        _cloudinaryService = cloudinaryService;
    }

    public async Task Handle(TestSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(notification.UserId);
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(notification.CourseId);

        if (user == null || course == null)
        {
            Console.WriteLine("Không tìm thấy user hoặc course.");
            return;
        }

        var certificateUrl = await GenerateCertificateAsync(user.FullName ?? "No Name", course.Name ?? "Unknown Course");

        var cert = new Certificates
        {
            UserId = user.Id,
            CourseId = course.Id,
            CreatedAt = DateTime.UtcNow,
            CertificateUrl = certificateUrl
        };

        await _unitOfWork.CertificatesRepository.AddAsync(cert);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine($"Đã cấp chứng chỉ cho {user.FullName} - {course.Name}");
    }

    // Hàm sinh file chứng chỉ PDF từ template HTML
    private async Task<string> GenerateCertificateAsync(string name, string courseName)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "templates", "certificate.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Không tìm thấy file template HTML chứng chỉ.", templatePath);

        var html = await File.ReadAllTextAsync(templatePath);
        html = html.Replace("{{Name}}", name)
                   .Replace("{{Course}}", courseName)
                   .Replace("{{Date}}", DateTime.Now.ToString("dd/MM/yyyy"));

        // Tạo PDF bằng MemoryStream (tối ưu I/O)
        var fileName = $"{Guid.NewGuid()}.pdf";
        using var pdfStream = new MemoryStream();

        HtmlToPdf converter = new HtmlToPdf();
        converter.Options.MarginTop = 20;
        converter.Options.MarginBottom = 20;
        converter.Options.MarginLeft = 20;
        converter.Options.MarginRight = 20;
        converter.Options.PdfPageSize = PdfPageSize.A4;
        converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;

        PdfDocument doc = converter.ConvertHtmlString(html);
        doc.Save(pdfStream);
        doc.Close();

        pdfStream.Position = 0; // Reset về đầu stream trước khi upload

        // Convert MemoryStream thành IFormFile để dùng cho Cloudinary
        var formFile = new FormFile(pdfStream, 0, pdfStream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary
            {
                { "Content-Disposition", new StringValues($"form-data; name=\"file\"; filename=\"{fileName}\"") }
            },
            ContentType = "application/pdf"
        };

        var uploadResult = await _cloudinaryService.UploadPdfAsync(formFile, "certificates");

        return uploadResult?.url ?? throw new Exception("Không upload được chứng chỉ lên Cloudinary.");
    }

}
