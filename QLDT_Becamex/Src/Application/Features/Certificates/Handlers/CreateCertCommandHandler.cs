using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using QLDT_Becamex.Src.Shared.Helpers;
using SelectPdf;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Handlers
{
    public class CreateCertCommandHandler : IRequestHandler<CreateCertCommand, string>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;
        private readonly ICloudinaryService _cloudinaryService;

        public CreateCertCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, IMediator mediator, IWebHostEnvironment env,
        ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _mediator = mediator;
            _env = env;
            _cloudinaryService = cloudinaryService;

        }

        public async Task<string> Handle(CreateCertCommand request, CancellationToken cancellationToken)
        {
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (currentUserId == null)
            {
                throw new AppException("Không xác định được người dùng hiện tại.", 401);
            }
            var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại.", 404);
            }

            bool isFinish = await MeetTheStandards(currentUserId, request.CourseId);
            // Kiểm tra nếu user đã hoàn thành khóa học (lessons + passed all tests)
            if (isFinish)
            {
                var isExistCert = await _unitOfWork.CertificatesRepository.AnyAsync(c => c.CourseId == course.Id && c.UserId == currentUserId);

                if (isExistCert)
                {
                    throw new AppException($"Chứng chỉ {course.Name} đã tồn tại.", 409);
                }
                var certificateUrl = await GenerateCertificateAsync(currentUser.FullName ?? "No Name", course.Name ?? "Unknown Course");

                var cert = new Domain.Entities.Certificates
                {
                    UserId = currentUser.Id,
                    CourseId = course.Id,
                    CreatedAt = DateTimeHelper.GetVietnamTimeNow(),
                    CertificateUrl = certificateUrl
                };

                await _unitOfWork.CertificatesRepository.AddAsync(cert);
                await _unitOfWork.CompleteAsync();

                return certificateUrl;
            }
            else
            {
                throw new AppException($"Bạn không đủ điều kiện cấp chứng chỉ cho khóa học này {course.Name}");
            }
        }


        private async Task<bool> MeetTheStandards(string userId, string courseId)
        {
            var lessons = await _unitOfWork.LessonRepository.GetQueryable()
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            var lessonIds = lessons.Select(l => l.Id).ToList();
            var lessonProgresses = await _unitOfWork.LessonProgressRepository.GetQueryable()
                .Where(lp => lp.UserId == userId && lessonIds.Contains(lp.LessonId))
                .ToListAsync();

            var tests = await _unitOfWork.TestRepository.GetQueryable()
                .Where(t => t.CourseId == courseId)
                .ToListAsync();

            var testIds = tests.Select(t => t.Id).ToList();
            var testResults = await _unitOfWork.TestResultRepository.GetQueryable()
                .Where(tr => tr.UserId == userId && testIds.Contains(tr.TestId))
                .ToListAsync();

            bool lessonsExist = lessonIds.Any();
            bool testsExist = testIds.Any();

            // Nếu khóa học không có nội dung (không lesson và không test) => không đạt
            if (!lessonsExist && !testsExist)
                return false;

            bool allLessonsCompleted = !lessonsExist || lessons.All(lesson =>
                lessonProgresses.Any(lp => lp.LessonId == lesson.Id && lp.IsCompleted));

            bool allTestsPassed = !testsExist || tests.All(test =>
                testResults.Any(tr => tr.TestId == test.Id && tr.IsPassed));

            return allLessonsCompleted && allTestsPassed;
        }


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
}