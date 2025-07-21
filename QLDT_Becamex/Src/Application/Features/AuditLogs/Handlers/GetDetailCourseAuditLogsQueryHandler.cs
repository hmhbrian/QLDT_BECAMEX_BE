using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.DataProvider;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.Handlers
{
    public class GetDetailCourseAuditLogsQueryHandler : IRequestHandler<GetDetailCourseAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogMapper _auditLogMapper;

        public GetDetailCourseAuditLogsQueryHandler(IUnitOfWork unitOfWork, IAuditLogMapper auditLogMapper)
        {
            _unitOfWork = unitOfWork;
            _auditLogMapper = auditLogMapper;
        }

        public async Task<List<AuditLogDto>> Handle(GetDetailCourseAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var courseId = request.courseId;

            // Lấy các Lesson, Test liên quan đến course
            var lessonIds = (await _unitOfWork.LessonRepository.FindAndSelectAsync(
                l => l.CourseId == courseId,
                l => l.Id.ToString()))?.ToList() ?? new List<string>();

            var testIds = (await _unitOfWork.TestRepository.FindAndSelectAsync(
                t => t.CourseId == courseId,
                t => t.Id.ToString()))?.ToList() ?? new List<string>();

            // Lấy audit logs
            var auditLogs = (await _unitOfWork.AuditLogRepository.GetFlexibleAsync(
                predicate: a =>
                    (a.EntityName == "Courses" && a.EntityId == courseId) ||
                    (a.EntityName == "Lessons" && lessonIds.Contains(a.EntityId)) ||
                    (a.EntityName == "Tests" && testIds.Contains(a.EntityId)),
                orderBy: q => q.OrderByDescending(l => l.Timestamp),
                includes: q => q.Include(a => a.User).AsNoTracking()
            )).ToList();


            //Lấy dữ liệu tham chiếu
            //User
            var userIds = auditLogs.Where(al => al.UserId != null).Select(al => al.UserId!).Distinct().ToList();
            var users = await _unitOfWork.UserRepository.GetFlexibleAsync(
                predicate: u => userIds.Contains(u.Id),
                asNoTracking: true
            );
            var userDict = users.ToDictionary(u => u.Id, u => u);

            // Đăng ký các provider xử lý dữ liệu tham chiếu
            var referenceDataProviders = new Dictionary<string, IEntityReferenceDataProvider>
            {
                { "Courses", new CourseReferenceDataProvider(_unitOfWork, auditLogs) },
                { "Lessons", new LessonReferenceDataProvider(_unitOfWork, auditLogs) },
                { "Tests", new TestReferenceDataProvider(_unitOfWork, auditLogs) }
            };

            // Ánh xạ sang DTO
            var auditLogDtos = auditLogs.Select(al =>
                _auditLogMapper.MapToDto(al, userDict, referenceDataProviders)
            ).ToList();
            return auditLogDtos;
        }
    }
}