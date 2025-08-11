using Microsoft.AspNet.Identity;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using System.Globalization;
using System.Text.Json;

namespace QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs
{
    public class AuditLogMapper : IAuditLogMapper
    {
        // Danh sách các trường cần loại bỏ (sử dụng HashSet để so sánh không phân biệt chữ hoa/thường)
        private readonly HashSet<string> _fieldsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Id", "CourseId","CreatedAt", "UpdatedAt", "CreatedById", "CreateById", "ModifiedAt", "UpdatedById", "UpdateById","StatusId","CategoryId","TypeDocId", "ModifiedTime", "UserId" };

        // Từ điển ánh xạ tên trường 
        private static readonly Dictionary<string, string> FieldNameMappings = new Dictionary<string, string>
        {
            { "Title", "Tiêu đề" },
            { "Department", "Phòng ban" },
            { "EmploymentLevel", "Cấp bậc" },
            { "FileUrl", "Đường dẫn tệp" },
            { "PublicIdUrlPdf", "ID công khai PDF" },
            { "EndDate", "Ngày kết thúc" },
            { "RegistrationClosingDate", "Ngày đóng đăng ký" },
            { "RegistrationStartDate", "Ngày mở đăng ký" },
            { "StartDate", "Ngày bắt đầu" },
            { "CategoryName", "Danh mục" },
            { "Code", "Mã khóa học" },
            { "Description", "Mô tả" },
            { "Format", "Hình thức" },
            { "HoursPerSessions", "Số giờ mỗi buổi" },
            { "IsDeleted", "Trạng thái xóa" },
            { "IsPrivate", "Riêng tư" },
            { "FullName", "Giảng viên" },
            { "Location", "Địa điểm/Link" },
            { "MaxParticipant", "Số lượng tối đa" },
            { "Name", "Tên khóa học" },
            { "Objectives", "Mục tiêu" },
            { "Optional", "Tùy chọn" },
            { "Sessions", "Số buổi" },
            { "StatusName", "Trạng thái" },
            { "ThumbUrl", "Ảnh đại diện" },
            { "Position", "Thứ tự" },
            { "TotalDurationSeconds", "Thời lượng (giây)" },
            { "TotalPages", "Tổng số trang" },
            { "NameType", "Loại tài liệu" },
            { "CourseName", "Tên khóa học" },
            { "TimeTest", "Thời gian làm bài" },
            { "PassThreshold", "Ngưỡng cần đạt" },
            { "UserName", "Người tham gia" }
        };

        public class AuditLogChanges
        {
            public Dictionary<string, object>? OldValues { get; set; }
            public Dictionary<string, object>? NewValues { get; set; }
        }

        public async Task<AuditLogDto> MapToDto(AuditLog auditLog, Dictionary<string, ApplicationUser> userDict, Dictionary<string, IEntityReferenceDataProvider> referenceDataProviders)
        {
            // Lấy múi giờ Việt Nam (UTC+7)
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Chuyển đổi thời gian UTC sang múi giờ Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(auditLog.Timestamp, vietnamTimeZone);

            var dto = new AuditLogDto
            {
                Id = auditLog.Id,
                Action = auditLog.Action ?? "Modified",
                EntityName = auditLog.EntityName ?? "Unknown",
                EntityId = auditLog.EntityId,
                UserName = auditLog.UserId != null && userDict.ContainsKey(auditLog.UserId) ? userDict[auditLog.UserId].FullName ?? "Unknown" : "Unknown",
                Timestamp = vietnamTime.ToString("dddd, dd MMMM, yyyy, HH:mm", new CultureInfo("vi-VN")),
                ChangedFields = new List<ChangedField>(),
                AddedFields = new List<AddedField>(),
                DeletedFields = new List<DeletedField>()
            };

            // Xử lý dữ liệu tham chiếu (Department, Position, v.v.)
            if (auditLog.EntityName != null && referenceDataProviders.ContainsKey(auditLog.EntityName))
            {
                var provider = referenceDataProviders[auditLog.EntityName];
                //var referenceData = provider.GetReferenceData(auditLog);
                var referenceData = await referenceDataProviders[auditLog.EntityName].GetReferenceData(auditLog);

                // Ánh xạ tên trường từ ReferenceData
                dto.AddedFields.AddRange(referenceData.AddedFields.Select(f => new AddedField
                {
                    FieldName = f.FieldName != null && FieldNameMappings.ContainsKey(f.FieldName) ? FieldNameMappings[f.FieldName] : f.FieldName,
                    Value = f.Value
                }));

                dto.ChangedFields.AddRange(referenceData.ChangedFields.Select(f => new ChangedField
                {
                    FieldName = f.FieldName != null && FieldNameMappings.ContainsKey(f.FieldName) ? FieldNameMappings[f.FieldName] : f.FieldName,
                    OldValue = f.OldValue,
                    NewValue = f.NewValue
                }));

                dto.DeletedFields.AddRange(referenceData.DeletedFields.Select(f => new DeletedField
                {
                    FieldName = f.FieldName != null && FieldNameMappings.ContainsKey(f.FieldName) ? FieldNameMappings[f.FieldName] : f.FieldName,
                    Value = f.Value
                }));
            }

            //Xử lý trường Changes
            if (!string.IsNullOrEmpty(auditLog.Changes))
            {
                try
                {
                    var changes = JsonSerializer.Deserialize<AuditLogChanges>(auditLog.Changes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                    if(changes?.OldValues !=  null && changes?.NewValues != null)
                    {
                        // Xử lý các trường trong NewValues (thêm hoặc thay đổi)
                        foreach (var field in changes.NewValues)
                        {
                            var fieldName = field.Key;
                            if (_fieldsToExclude.Contains(fieldName))
                                continue;

                            var displayFieldName = FieldNameMappings.ContainsKey(fieldName) ? FieldNameMappings[fieldName] : fieldName;

                            var newValue = ConvertJsonElement(field.Value);
                            var oldValue = changes.OldValues.ContainsKey(fieldName) ? ConvertJsonElement(changes.OldValues[fieldName]) : null;

                            if (oldValue == null && newValue != null)
                            {
                                dto.AddedFields.Add(new AddedField { FieldName = displayFieldName, Value = newValue });
                            }
                            else if (oldValue != null && newValue != null && !AreValuesEqual(oldValue, newValue))
                            {
                                dto.ChangedFields.Add(new ChangedField { FieldName = displayFieldName, OldValue = oldValue, NewValue = newValue });
                            }
                        }
                        // Xử lý các trường bị xóa
                        foreach (var field in changes.OldValues)
                        {
                            var fieldName = field.Key;
                            if (_fieldsToExclude.Contains(fieldName))
                                continue;

                            if (!changes.NewValues.ContainsKey(fieldName))
                            {
                                var displayFieldName = FieldNameMappings.ContainsKey(fieldName) ? FieldNameMappings[fieldName] : fieldName;

                                dto.DeletedFields.Add(new DeletedField { FieldName = displayFieldName, Value = ConvertJsonElement(field.Value) });
                            }
                        }
                    }    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing Changes for ID {auditLog.Id}: {ex.Message}");
                }
            }
            return dto;
        }

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
