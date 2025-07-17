using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.Handlers
{
    public class GetDetailCourseAuditLogsQueryHandler : IRequestHandler<GetDetailCourseAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDetailCourseAuditLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public class AuditLogChanges
        {
            public Dictionary<string, object>? OldValues { get; set; }
            public Dictionary<string, object>? NewValues { get; set; }
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

            var auditLogs = await _unitOfWork.AuditLogRepository.GetFlexibleAsync(
                predicate: a =>
                    (a.EntityName == "Courses" && a.EntityId == courseId) ||
                    (a.EntityName == "Lessons" && lessonIds.Contains(a.EntityId)) ||
                    (a.EntityName == "Tests" && testIds.Contains(a.EntityId)),
                orderBy: q => q.OrderByDescending(l => l.Timestamp),
                includes: q => q.Include(a => a.User).AsNoTracking()
            );

            var userIds = auditLogs.Where(al => al.UserId != null).Select(al => al.UserId!).Distinct().ToList();
            var users = await _unitOfWork.UserRepository.GetFlexibleAsync(
                predicate: u => userIds.Contains(u.Id),
                asNoTracking: true
            );
            var userDict = users.ToDictionary(u => u.Id, u => u);

            var auditLogDtos = auditLogs.Select(al =>
            {
                var dto = new AuditLogDto
                {
                    Id = al.Id,
                    Action = al.Action ?? "Modified",
                    EntityName = al.EntityName ?? "Unknown",
                    EntityId = al.EntityId,
                    UserName = al.UserId != null && userDict.ContainsKey(al.UserId) ? userDict[al.UserId].FullName ?? "Unknown" : "Unknown",
                    Timestamp = al.Timestamp.ToString("dddd, dd MMMM, yyyy, HH:mm", new CultureInfo("vi-VN")),
                    ChangedFields = new List<ChangedField>(),
                    AddedFields = new List<AddedField>(),
                    DeletedFields = new List<DeletedField>()
                };

                if (!string.IsNullOrEmpty(al.Changes))
                {
                    try
                    {
                        // Deserialize chuỗi Changes thành AuditLogChanges
                        var changes = JsonSerializer.Deserialize<AuditLogChanges>(
                            al.Changes,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (changes?.OldValues != null && changes?.NewValues != null)
                        {
                            // Xử lý các trường trong NewValues (thêm hoặc thay đổi)
                            foreach (var field in changes.NewValues)
                            {
                                var fieldName = field.Key;
                                var newValue = ConvertJsonElement(field.Value);
                                var oldValue = changes.OldValues.ContainsKey(fieldName) ? ConvertJsonElement(changes.OldValues[fieldName]) : null;

                                if (oldValue == null && newValue != null)
                                {
                                    dto.AddedFields.Add(new AddedField { FieldName = fieldName, Value = newValue });
                                }
                                else if (oldValue != null && newValue != null && !AreValuesEqual(oldValue, newValue))
                                {
                                    dto.ChangedFields.Add(new ChangedField { FieldName = fieldName, OldValue = oldValue, NewValue = newValue });
                                }
                            }

                            // Xử lý các trường bị xóa (có trong OldValues nhưng không trong NewValues)
                            foreach (var field in changes.OldValues)
                            {
                                var fieldName = field.Key;
                                if (!changes.NewValues.ContainsKey(fieldName))
                                {
                                    dto.DeletedFields.Add(new DeletedField { FieldName = fieldName, Value = ConvertJsonElement(field.Value) });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing Changes for ID {al.Id}: {ex.Message}");
                    }
                }

                return dto;
            }).ToList();

            return auditLogDtos;
        }

        // Hàm chuyển đổi JsonElement thành kiểu dữ liệu phù hợp
        private object? ConvertJsonElement(object? value)
        {
            if (value is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.String:
                        return jsonElement.GetString();
                    case JsonValueKind.Number:
                        return jsonElement.GetDouble();
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.False:
                        return false;
                    case JsonValueKind.Null:
                        return null;
                    default:
                        return jsonElement.ToString();
                }
            }
            return value;
        }

        // Hàm so sánh giá trị an toàn
        private bool AreValuesEqual(object? oldValue, object? newValue)
        {
            if (oldValue == null && newValue == null)
                return true;
            if (oldValue == null || newValue == null)
                return false;
            return oldValue.ToString() == newValue.ToString();
        }
    }
}