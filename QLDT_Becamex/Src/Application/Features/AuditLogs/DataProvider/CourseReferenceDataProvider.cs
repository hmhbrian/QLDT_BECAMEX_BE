using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.DataProvider
{
    public class CourseReferenceDataProvider : IEntityReferenceDataProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<AuditLog> _allAuditLogs;

        public CourseReferenceDataProvider(IUnitOfWork unitOfWork, List<AuditLog> auditLogs)
        {
            _unitOfWork = unitOfWork;
            _allAuditLogs = auditLogs;
        }

        public async Task<ReferenceData> GetReferenceData(AuditLog auditLog)
        {
            var referenceData = new ReferenceData();
            var courseId = auditLog.EntityId;

            if (courseId == null)
                throw new AppException($"Id khóa học không hợp lệ", 400);

            // Lấy dữ liệu hiện tại và bao gồm cả ID từ bản ghi Deleted
            var (departmentDict, ELevelDict, userDict, previousDepartmentIds, currentDepartmentIds, previousELevelIds, currentELevelIds) = await GetCurrentReferenceDictionariesAsync(courseId, auditLog); 
            var (statusName, categoryName, hasStatusChange) = await GetCurrentCourseDetailsAsync(courseId, auditLog);


            if (auditLog.Action == "Added")
            {
                await AddFieldsForAddedActionAsync(referenceData, courseId, departmentDict, ELevelDict, userDict, statusName, categoryName, currentDepartmentIds, currentELevelIds);
            }
            else if (auditLog.Action == "Modified")
            {
                await AddFieldsForModifiedActionAsync(referenceData, auditLog, courseId, departmentDict, ELevelDict, userDict, statusName, categoryName, hasStatusChange, previousDepartmentIds, currentDepartmentIds, previousELevelIds, currentELevelIds);
            }

            return referenceData;
        }

        private async Task<(Dictionary<string, string> departmentDict, Dictionary<string, string> ELevelDict, Dictionary<string, string> userDict, List<string> previousDepartmentIds, List<string> currentDepartmentIds, List<string> previousELevelIds, List<string> currentELevelIds)> GetCurrentReferenceDictionariesAsync(string courseId, AuditLog auditLog)
        {
            // Loại bỏ mili giây
            var auditLogTimeWithoutMs = auditLog.Timestamp.AddTicks(-(auditLog.Timestamp.Ticks % TimeSpan.TicksPerSecond));
            var timeWindow = TimeSpan.FromSeconds(1);

            // Lấy CourseDepartment, CourseELevel và UserCourse
            //var courseDepartments = await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == courseId);
            //var departmentIds = courseDepartments.Select(cd => cd.DepartmentId.ToString()).ToList();

            //var courseELevels = await _unitOfWork.CourseELevelRepository.FindAsync(cp => cp.CourseId == courseId);
            //var ELevelIds = courseELevels.Select(cp => cp.ELevelId.ToString()).ToList();

            var courseUsers = await _unitOfWork.UserCourseRepository.FindAsync(cp => cp.CourseId == courseId);
            var userIds = courseUsers.Select(cp => cp.UserId).ToList();

            // Lấy ID từ bản ghi Add để đảm bảo ánh xạ đầy đủ
            var addedDepartmentLogs = _allAuditLogs
                .Where(p => p.EntityName == "CourseDepartment" && p.Action == "Added" &&
                    p.Changes!.Contains($"\"CourseId\":\"{courseId}\"") &&
                    p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs)
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var currentDepartmentIds = ExtractIdsFromAddedLogs(addedDepartmentLogs, "DepartmentId");

            var addedELevelLogs = _allAuditLogs 
                .Where(p => p.EntityName == "CourseELevel" && p.Action == "Added" &&
                    p.Changes!.Contains($"\"CourseId\":\"{courseId}\"") &&
                    p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs)
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var currentELevelIds = ExtractIdsFromAddedLogs(addedELevelLogs, "ELevelId");

            // Lấy ID từ bản ghi Deleted để đảm bảo ánh xạ đầy đủ
            var deletedDepartmentLogs = _allAuditLogs
                .Where(p => p.EntityName == "CourseDepartment" && p.Action == "Deleted" && p.Changes!.Contains($"\"CourseId\":\"{courseId}\"") 
                        && p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs)
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var previousDepartmentIds = ExtractIdsFromDeletedLogs(deletedDepartmentLogs, "DepartmentId");

            var deletedELevelLogs = _allAuditLogs
                .Where(p => p.EntityName == "CourseELevel" && p.Action == "Deleted" && p.Changes!.Contains($"\"CourseId\":\"{courseId}\"") 
                        && p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs)
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var previousELevelIds = ExtractIdsFromDeletedLogs(deletedELevelLogs, "ELevelId");

            var deletedUserLogs = _allAuditLogs
                .Where(p => p.EntityName == "UserCourse" && p.Action == "Deleted" && p.Changes!.Contains($"\"CourseId\":\"{courseId}\"") 
                        && p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs)
                .ToList();
            var previousUserIds = ExtractIdsFromDeletedLogs(deletedUserLogs, "UserId");

            // Lấy tên Department, ELevel, User
            var departments = await _unitOfWork.DepartmentRepository.FindAsync(d => currentDepartmentIds.Concat(previousDepartmentIds).Contains(d.DepartmentId.ToString()));
            var departmentDict = departments.ToDictionary(d => d.DepartmentId.ToString(), d => d.DepartmentName ?? string.Empty);

            var ELevels = await _unitOfWork.EmployeeLevelRepository.FindAsync(p => currentELevelIds.Concat(previousELevelIds).Contains(p.ELevelId.ToString()));
            var ELevelDict = ELevels.ToDictionary(p => p.ELevelId.ToString(), p => p.ELevelName ?? string.Empty);

            var users = await _unitOfWork.UserRepository.FindAsync(p => userIds.Contains(p.Id));
            var userDict = users.ToDictionary(p => p.Id, p => p.FullName ?? string.Empty);

            // Ghi log để debug
            Console.WriteLine($"CourseId: {courseId}, AuditLog Timestamp (without ms): {auditLogTimeWithoutMs}, Original: {auditLog.Timestamp}");
            Console.WriteLine($"Previous Department IDs: {string.Join(", ", previousDepartmentIds)}");
            Console.WriteLine($"Current Department IDs: {string.Join(", ", currentDepartmentIds)}");
            Console.WriteLine($"Previous ELevel IDs: {string.Join(", ", previousELevelIds)}");
            Console.WriteLine($"Current ELevel IDs: {string.Join(", ", currentELevelIds)}");

            return (departmentDict, ELevelDict, userDict, previousDepartmentIds, currentDepartmentIds, previousELevelIds, currentELevelIds);
        }

        private async Task<(string statusName, string categoryName,bool hasStatusChange)> GetCurrentCourseDetailsAsync(string courseId, AuditLog auditLog)
        {
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                    predicate: c => c.Id.ToString() == courseId,
                    includes: p => p
                        .Include(a => a.Category)
                        .Include(a => a.Status)
               );
            string statusName = "Unknown";
            string categoryName = "Unknown";

            if (course != null)
            {
                var status = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(s => s.Id == course.StatusId);
                statusName = status?.StatusName ?? "Unknown";

                if (course.CategoryId > 0)
                {
                    var category = await _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.Id == course.CategoryId);
                    categoryName = category?.CategoryName ?? "Unknown";
                }
            }

            // Kiểm tra xem Status có thay đổi trong Changes không
            var changes = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                auditLog.Changes!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            bool hasStatusChange = changes?.OldValues?.ContainsKey("StatusId") == true || changes?.NewValues?.ContainsKey("StatusId") == true;

            return (statusName, categoryName, hasStatusChange);
        }

        private async Task AddFieldsForAddedActionAsync(
            ReferenceData referenceData,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> ELevelDict,
            Dictionary<string, string> userDict,
            string statusName,
            string categoryName,
            List<string> currentDepartmentIds,
            List<string> currentELevelIds)
        {
            // Thêm Department vào AddedFields
            if (currentDepartmentIds.Any())
            {
                var departmentNames = currentDepartmentIds
                    .Where(id => departmentDict.ContainsKey(id))
                    .Select(id => departmentDict[id])
                    .OrderBy(name => name)
                    .ToList();
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "Department",
                    Value = string.Join(", ", departmentNames)
                });
            }

            // Thêm ELevel vào AddedFields
            if (currentELevelIds.Any())
            {
                var ELevelNames = currentELevelIds
                    .Where(id => ELevelDict.ContainsKey(id))
                    .Select(id => ELevelDict[id])
                    .OrderBy(name => name)
                    .ToList();
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "EmploymentLevel",
                    Value = string.Join(", ", ELevelNames)
                });
            }

            // Thêm User vào AddedFields
            var userIds = (await _unitOfWork.UserCourseRepository.FindAsync(cp => cp.CourseId == courseId))
                .Select(cp => cp.UserId).ToList();
            if (userIds.Any())
            {
                var userNames = userIds
                    .Where(id => userDict.ContainsKey(id))
                    .Select(id => userDict[id])
                    .OrderBy(name => name)
                    .ToList();
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "UserName",
                    Value = string.Join(", ", userNames)
                });
            }

            // Thêm StatusName, CategoryName vào AddedFields
            if (statusName != "Unknown")
            {
                referenceData.AddedFields.Add(new AddedField { FieldName = "StatusName", Value = statusName });
            }
            if (categoryName != "Unknown")
            {
                referenceData.AddedFields.Add(new AddedField { FieldName = "CategoryName", Value = categoryName });
            }
        }

        private async Task AddFieldsForModifiedActionAsync(
            ReferenceData referenceData,
            AuditLog auditLog,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> ELevelDict,
            Dictionary<string, string> userDict,
            string statusName,
            string categoryName,
            bool hasStatusChange,
            List<string> previousDepartmentIds,
            List<string> currentDepartmentIds,
            List<string> previousELevelIds,
            List<string> currentELevelIds)
        {
            // Lấy các bản ghi Deleted trong khoảng thời gian gần với auditLog.Timestamp
            var auditLogTimeWithoutMs = auditLog.Timestamp.AddTicks(-(auditLog.Timestamp.Ticks % TimeSpan.TicksPerSecond));
            var timeWindow = TimeSpan.FromSeconds(1);

            var deletedUserLogs = _allAuditLogs
                .Where(p => p.EntityName == "UserCourse" &&
                           p.Action == "Deleted" &&
                           p.Timestamp.AddTicks(-(p.Timestamp.Ticks % TimeSpan.TicksPerSecond)) == auditLogTimeWithoutMs &&
                           p.Changes!.Contains($"\"CourseId\":\"{courseId}\""))
                .OrderByDescending(p => p.Timestamp)
                .ToList();

            var previousUserIds = ExtractIdsFromDeletedLogs(deletedUserLogs, "UserId");

            // Lấy thông tin trạng thái trước đó
            var (previousStatusName, previousCategoryName) = await GetPreviousCourseDetailsAsync(auditLog, courseId);

            // So sánh và thêm vào ChangedFields
            await AddChangedFieldsAsync(referenceData, courseId, departmentDict, ELevelDict, userDict,
                previousDepartmentIds, currentDepartmentIds, previousELevelIds, currentELevelIds, previousUserIds,
                statusName, previousStatusName, categoryName, previousCategoryName,  hasStatusChange);
        }

        private List<string> ExtractIdsFromAddedLogs(List<AuditLog> logs, string idFieldName)
        {
            var ids = new List<string>();
            foreach (var log in logs)
            {
                if (!string.IsNullOrEmpty(log.Changes))
                {
                    try
                    {
                        var changes = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                            log.Changes,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (changes?.NewValues?.ContainsKey(idFieldName) == true)
                        {
                            var idValue = changes.NewValues[idFieldName].ToString();
                            if (idValue != null)
                            {
                                ids.Add(idValue);
                            }
                            else
                            {
                                Console.WriteLine($"Null value found for {idFieldName} in AuditLog ID {log.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing Changes for AuditLog ID {log.Id}: {ex.Message}");
                    }
                }
            }
            return ids;
        }

        private List<string> ExtractIdsFromDeletedLogs(List<AuditLog> logs, string idFieldName)
        {
            var ids = new List<string>();
            foreach (var log in logs)
            {
                if (!string.IsNullOrEmpty(log.Changes))
                {
                    try
                    {
                        var changes = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                            log.Changes,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (changes?.OldValues?.ContainsKey(idFieldName) == true)
                        {
                            var idValue = changes.OldValues[idFieldName].ToString();
                            if (idValue != null)
                            {
                                ids.Add(idValue);
                            }
                            else
                            {
                                Console.WriteLine($"Null value found for {idFieldName} in AuditLog ID {log.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing Changes for AuditLog ID {log.Id}: {ex.Message}");
                    }
                }
            }
            return ids;
        }

        private async Task<(string statusName, string categoryName)> GetPreviousCourseDetailsAsync(AuditLog auditLog, string courseId)
        {
            string previousStatusName = "Unknown";
            string previousCategoryName = "Unknown";

            // Truy vấn bảng Courses để lấy trạng thái trước đó
            var previousCourse = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id.ToString() == courseId && c.UpdatedAt < auditLog.Timestamp,
                includes: p => p
                    .Include(a => a.Category)
                    .Include(a => a.Status));

            if (previousCourse != null)
            {
                previousStatusName = previousCourse.Status?.StatusName ?? "Unknown";
                previousCategoryName = previousCourse.Category?.CategoryName ?? "Unknown";
            }
            else
            {
                // Nếu không tìm thấy bản ghi trước đó, kiểm tra previousAuditLog
                var previousAuditLog = _allAuditLogs
                    .Where(p => p.EntityName == "Courses" && p.EntityId == auditLog.EntityId && p.Timestamp < auditLog.Timestamp)
                    .OrderByDescending(p => p.Timestamp)
                    .FirstOrDefault();

                if (previousAuditLog != null && !string.IsNullOrEmpty(previousAuditLog.Changes))
                {
                    var previousChanges = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                        previousAuditLog.Changes,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (previousChanges?.NewValues?.ContainsKey("StatusId") == true)
                    {
                        var previousStatusId = previousChanges.NewValues["StatusId"]?.ToString();
                        var previousStatus = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(s => s.Id.ToString() == previousStatusId);
                        previousStatusName = previousStatus?.StatusName ?? "Unknown";
                    }

                    if (previousChanges?.NewValues?.ContainsKey("CategoryId") == true)
                    {
                        var previousCategoryId = previousChanges.NewValues["CategoryId"]?.ToString();
                        var previousCategory = await _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.Id.ToString() == previousCategoryId);
                        previousCategoryName = previousCategory?.CategoryName ?? "Unknown";
                    }
                }
            }

            return (previousStatusName, previousCategoryName);
        }

        private async Task AddChangedFieldsAsync(
            ReferenceData referenceData,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> ELevelDict,
            Dictionary<string, string> userDict,
            List<string> previousDepartmentIds,
            List<string> currentDepartmentIds,
            List<string> previousELevelIds,
            List<string> currentELevelIds,
            List<string> previousUserIds,
            string statusName,
            string previousStatusName,
            string categoryName,
            string previousCategoryName,
            bool hasStatusChange)
        {
            // Lấy danh sách tên hiện tại
            var currentDepartmentNames = currentDepartmentIds
                .Where(id => departmentDict.ContainsKey(id))
                .Select(id => departmentDict[id])
                .OrderBy(name => name)
                .ToList();

            var currentELevelNames = currentELevelIds
                .Where(id => ELevelDict.ContainsKey(id))
                .Select(id => ELevelDict[id])
                .OrderBy(name => name)
                .ToList();

            var currentUserNames = (await _unitOfWork.UserCourseRepository.FindAsync(cp => cp.CourseId == courseId))
                .Select(cp => cp.UserId)
                .Where(id => userDict.ContainsKey(id))
                .Select(id => userDict[id])
                .OrderBy(name => name)
                .ToList();

            // Ánh xạ previous IDs sang tên
            var previousDepartmentNames = previousDepartmentIds
                .Where(id => departmentDict.ContainsKey(id))
                .Select(id => departmentDict[id])
                .OrderBy(name => name)
                .ToList();

            var previousELevelNames = previousELevelIds
                .Where(id => ELevelDict.ContainsKey(id))
                .Select(id => ELevelDict[id])
                .OrderBy(name => name)
                .ToList();

            var previousUserNames = previousUserIds
                .Where(id => userDict.ContainsKey(id))
                .Select(id => userDict[id])
                .OrderBy(name => name)
                .ToList();

            if (previousDepartmentNames.Any())
            {
                var departmentOldValue = previousDepartmentNames.Any() ? string.Join(", ", previousDepartmentNames) : "None";
                var departmentNewValue = currentDepartmentNames.Any() ? string.Join(", ", currentDepartmentNames) : "None";
                if (departmentOldValue != departmentNewValue)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "Department",
                        OldValue = departmentOldValue,
                        NewValue = departmentNewValue
                    });
                }
            }

            if (previousELevelNames.Any())
            {
                var ELevelOldValue = previousELevelNames.Any() ? string.Join(", ", previousELevelNames) : "None";
                var ELevelNewValue = currentELevelNames.Any() ? string.Join(", ", currentELevelNames) : "None";
                if (ELevelOldValue != ELevelNewValue)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "EmploymentLevel",
                        OldValue = ELevelOldValue,
                        NewValue = ELevelNewValue
                    });
                }
            }

            if (hasStatusChange)
            {
                if (statusName != previousStatusName)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "StatusName",
                        OldValue = previousStatusName,
                        NewValue = statusName
                    });
                }
            }

            if (categoryName != previousCategoryName)
            {
                referenceData.ChangedFields.Add(new ChangedField
                {
                    FieldName = "CategoryName",
                    OldValue = previousCategoryName,
                    NewValue = categoryName
                });
            }

            var userOldValue = previousUserNames.Any() ? string.Join(", ", previousUserNames) : "None";
            var userNewValue = currentUserNames.Any() ? string.Join(", ", currentUserNames) : "None";
            if (userOldValue != userNewValue)
            {
                referenceData.ChangedFields.Add(new ChangedField
                {
                    FieldName = "UserName",
                    OldValue = userOldValue,
                    NewValue = userNewValue
                });
            }
        }
    }
}