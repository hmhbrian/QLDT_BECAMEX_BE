using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.DataProvider;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Queries;
using QLDT_Becamex.Src.Domain.Entities;
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

            // Lấy tất cả các bản ghi audit log liên quan trước, bao gồm cả Deleted
            var allRelatedAuditLogs = (await _unitOfWork.AuditLogRepository.GetFlexibleAsync(
                predicate: a =>
                    (a.EntityName == "Courses" && a.EntityId == courseId) ||
                    new[] { "CourseDepartment", "CourseELevel", "Lessons", "Tests", "CourseAttachedFile" }.Contains(a.EntityName),
                orderBy: q => q.OrderByDescending(l => l.Timestamp),
                includes: q => q.Include(a => a.User).AsNoTracking()
            )).ToList();

            // Lọc các bản ghi Deleted để lấy EntityId dựa trên CourseId trong Changes
            var deletedCourseDepartmentIds = allRelatedAuditLogs
                .Where(a => a.EntityName == "CourseDepartment" && a.Action == "Deleted" && a.Changes?.Contains($"\"CourseId\":\"{courseId}\"") == true)
                .Select(a => a.EntityId)
                .Where(id => id != null)
                .ToList();
            var deletedCourseELevelIds = allRelatedAuditLogs
                .Where(a => a.EntityName == "CourseELevel" && a.Action == "Deleted" && a.Changes?.Contains($"\"CourseId\":\"{courseId}\"") == true)
                .Select(a => a.EntityId)
                .Where(id => id != null)
                .ToList();
            var deletedLessonIds = allRelatedAuditLogs
                .Where(a => a.EntityName == "Lessons" && a.Action == "Deleted" && a.Changes?.Contains($"\"CourseId\":\"{courseId}\"") == true)
                .Select(a => a.EntityId)
                .Where(id => id != null)
                .ToList();
            var deletedTestIds = allRelatedAuditLogs
                .Where(a => a.EntityName == "Tests" && a.Action == "Deleted" && a.Changes?.Contains($"\"CourseId\":\"{courseId}\"") == true)
                .Select(a => a.EntityId)
                .Where(id => id != null)
                .ToList();
            var deletedAttachFileIds = allRelatedAuditLogs
                .Where(a => a.EntityName == "CourseAttachedFile" && a.Action == "Deleted" && a.Changes?.Contains($"\"CourseId\":\"{courseId}\"") == true)
                .Select(a => a.EntityId)
                .Where(id => id != null)
                .ToList();

            // Lấy các Lesson, Test liên quan đến course
            var lessonIds = (await _unitOfWork.LessonRepository
                .FindAndSelectAsync(
                    l => l.CourseId == courseId,
                    l => l.Id.ToString()))?
                .Where(id => id != null).ToList() ?? new List<string>();
            lessonIds.AddRange(deletedLessonIds!);

            var testIds = (await _unitOfWork.TestRepository
                .FindAndSelectAsync(
                    t => t.CourseId == courseId,
                    t => t.Id.ToString()))?
                .Where(id => id != null).ToList() ?? new List<string>();
            testIds.AddRange(deletedTestIds!);

            var attachFileIds = (await _unitOfWork.CourseAttachedFileRepository
                .FindAndSelectAsync(
                    a => a.CourseId == courseId,
                    a => a.Id.ToString()))?
                .Where(id => id != null).ToList() ?? new List<string>();
            attachFileIds.AddRange(deletedAttachFileIds!);

            var courseDepartment = (await _unitOfWork.CourseDepartmentRepository
                .FindAndSelectAsync(
                    a => a.CourseId == courseId,
                    a => a.Id.ToString()))?
                .Where(id => id != null)
                .ToList() ?? new List<string>();
            courseDepartment.AddRange(deletedCourseDepartmentIds!);

            var courseELevel = (await _unitOfWork.CourseELevelRepository
                .FindAndSelectAsync(
                    a => a.CourseId == courseId,
                    a => a.Id.ToString()))?
                .Where(id => id != null)
                .ToList() ?? new List<string>();
            courseELevel.AddRange(deletedCourseELevelIds!);

            var predicate = PredicateBuilder.New<AuditLog>(false);
            predicate = predicate.Or(a => a.EntityName == "Courses" && a.EntityId == courseId);
            predicate = predicate.Or(a => a.EntityName == "Lessons" && a.EntityId != null && lessonIds.Contains(a.EntityId));
            predicate = predicate.Or(a => a.EntityName == "CourseDepartment" && a.EntityId != null && courseDepartment.Contains(a.EntityId));
            predicate = predicate.Or(a => a.EntityName == "CourseELevel" && a.EntityId != null && courseELevel.Contains(a.EntityId));
            predicate = predicate.Or(a => a.EntityName == "Tests" && a.EntityId != null && testIds.Contains(a.EntityId));
            predicate = predicate.Or(a => a.EntityName == "CourseAttachedFile" && a.EntityId != null && attachFileIds.Contains(a.EntityId));

            // Lấy audit logs
            var auditLogs = (await _unitOfWork.AuditLogRepository.GetFlexibleAsync(
                predicate:predicate,
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
                { "Tests", new TestReferenceDataProvider(_unitOfWork, auditLogs) },
                { "CourseAttachedFile", new TestReferenceDataProvider(_unitOfWork, auditLogs) }
            };

            // Ánh xạ sang DTO
            //var auditLogDtos = auditLogs.Select(al =>
            //    _auditLogMapper.MapToDto(al, userDict, referenceDataProviders)
            //).ToList();
            var auditLogDtos = new List<AuditLogDto>();
            foreach (var al in auditLogs)
            {
                // Loại bỏ CourseDepartment và CourseELevel có action là Deleted
                if (!(new[] { "CourseDepartment", "CourseELevel" }.Contains(al.EntityName)))
                {
                    var referenceData = al.EntityName != null && referenceDataProviders.ContainsKey(al.EntityName)
                        ? await referenceDataProviders[al.EntityName].GetReferenceData(al) 
                        : new ReferenceData();
                    auditLogDtos.Add(_auditLogMapper.MapToDto(al, userDict, referenceDataProviders).Result);
                }
            }
            return auditLogDtos;
        }
    }
}