using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic; // Cần thiết cho List

using DomainEntities = QLDT_Becamex.Src.Domain.Entities;
using Azure.Core;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Handlers
{
    // !!! QUAN TRỌNG: Thay đổi kiểu trả về từ CourseAttachedFileDto sang List<CourseAttachedFileDto>
    public class CreateCourseAttachedFileCommandHandler : IRequestHandler<CreateCourseAttachedFileCommand, List<CourseAttachedFileDto>>
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public CreateCourseAttachedFileCommandHandler(ICloudinaryService cloudinaryService, IUnitOfWork unitOfWork, IUserService userService)
        {
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<List<CourseAttachedFileDto>> Handle(CreateCourseAttachedFileCommand command, CancellationToken cancellationToken)
        {
            var courseId = command.CourseId; // Lấy CourseId từ command
            var requests = command.Request;  // Lấy danh sách DTO từ command
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Không tìm thấy thông tin người dùng được xác thực.", 401);
            }
            if (requests.Count == 0 || requests == null )
            {
                throw new AppException("Danh sách request không được để trống.", 400);
            }

            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);

            if (existingCourse == null)
            {
                throw new AppException($"Course with ID: {courseId} not found.", 404);
            }

            var createdFilesDto = new List<CourseAttachedFileDto>(); // Danh sách để lưu các DTO kết quả

            foreach (var request in requests)
            {
                string? fileOrLinkUrl = null;
                int fileType = 1;
                string? filePublicId = null; // Biến để lưu PublicId

                // Kiểm tra xem có Title không
                if (string.IsNullOrEmpty(request.Title))
                {
                    throw new AppException("Tiêu đề của file đính kèm không được để trống.", 400);
                }

                // 1. Kiểm tra ưu tiên: nếu có Link thì lưu Link
                if (request.Link != null && request.File == null)
                {
                    fileOrLinkUrl = request.Link;
                    fileType = 2; //link
                }
                // 2. Nếu không có Link, kiểm tra File
                else if (request.File != null)
                {
                    // CHỈ CHẤP NHẬN PDF: Cập nhật allowedMimeTypes
                    var allowedMimeTypes = new[] { "application/pdf" };
                    // CHỈ CHẤP NHẬN PDF: Cập nhật allowedExtensions
                    var allowedExtensions = new[] { ".pdf" };

                    // Kiểm tra kiểu MIME của tệp
                    if (!allowedMimeTypes.Contains(request.File.ContentType))
                    {
                        throw new AppException($"Chỉ được phép tải lên các file PDF. File '{request.File.FileName}' có định dạng không hợp lệ.", 403);
                    }

                    var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                    // Kiểm tra phần mở rộng của tệp
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new AppException($"Phần mở rộng của file không hợp lệ. Chỉ được phép .pdf. File '{request.File.FileName}' có phần mở rộng '{fileExtension}'.", 403);
                    }

                    // Đơn giản hóa logic fileType, chỉ gán là "PDF"
                    if (fileExtension == ".pdf")
                    {
                        fileType = 1; //pdf
                    }
                    // Không cần else if cho PPT/PPTX nữa, vì chúng ta đã loại bỏ chúng ở trên

                    try
                    {
                        // Gọi hàm UploadPdfAsync từ CloudinaryService của bạn
                        var uploadResult = await _cloudinaryService.UploadPdfAsync(request.File, "Course_pdfs");
                        fileOrLinkUrl = uploadResult?.url; // Lấy URL từ kết quả upload
                        filePublicId = uploadResult?.publicId; // Lấy PublicId từ kết quả upload
                        if (fileOrLinkUrl == null)
                        {
                            throw new AppException($"Không thể tải file '{request.File.FileName}' lên dịch vụ lưu trữ.", 500);
                        }
                        if (string.IsNullOrEmpty(filePublicId))
                        {
                            throw new AppException("Không thể lấy PublicId từ file tải lên.", 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[HANDLER ERROR] Lỗi khi tải file '{request.File.FileName}' lên Cloudinary: {ex.Message}");
                        throw new AppException($"Có lỗi xảy ra khi tải file '{request.File.FileName}' lên.", 500);
                    }
                }
                // 3. Nếu không có cả Link và File cho một mục, ném lỗi
                else
                {
                    throw new AppException("Phải cung cấp một file hoặc một liên kết đính kèm cho mỗi mục.", 400);
                }

                // --- Lưu thông tin vào Database ---
                var newAttachedFile = new DomainEntities.CourseAttachedFile();
                newAttachedFile.Create(courseId, userId, request.Title, fileOrLinkUrl, filePublicId, fileType);

                await _unitOfWork.CourseAttachedFileRepository.AddAsync(newAttachedFile);

                Console.WriteLine($"[HANDLER LOG] Đã chuẩn bị lưu file đính kèm: CourseId: {courseId}, Title: {request.Title}, Type: {fileType}, Link: {fileOrLinkUrl}");

                // Thêm DTO của file đã tạo vào danh sách kết quả
                createdFilesDto.Add(new CourseAttachedFileDto
                {
                    CourseId = newAttachedFile.CourseId,
                    Title = newAttachedFile.Title,
                    Type = newAttachedFile.TypeDoc?.NameType ?? "Unknown",
                    Link = newAttachedFile.Link,
                    PublicIdUrlPdf = newAttachedFile.PublicIdUrlPdf,
                    CreatedAt = newAttachedFile.CreatedAt
                });
            }

            // Gọi CompleteAsync một lần duy nhất sau khi đã thêm tất cả các file
            await _unitOfWork.CompleteAsync();
            //if (result <= 0)
            //{
            //    Console.WriteLine("[HANDLER ERROR] Không có bản ghi nào được lưu vào database.");
            //    throw new AppException("Không thể lưu dữ liệu vào database.", 500);
            //}

            return createdFilesDto; // Trả về danh sách DTO của các file đã tạo
        }
    }
}