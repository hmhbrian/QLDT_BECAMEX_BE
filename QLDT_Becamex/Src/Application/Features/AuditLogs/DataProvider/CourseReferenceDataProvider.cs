using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Data.Entity;
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

            // Lấy dữ liệu hiện tại và bao gồm cả ID từ bản ghi Deleted
            var (departmentDict, positionDict, userDict, previousDepartmentIds, previousPositionIds) = await GetCurrentReferenceDictionariesAsync(courseId, auditLog);
            var (statusName, categoryName, lecturerName, hasStatusChange) = await GetCurrentCourseDetailsAsync(courseId, auditLog);

            if (auditLog.Action == "Added")
            {
                await AddFieldsForAddedActionAsync(referenceData, courseId, departmentDict, positionDict, userDict, statusName, categoryName, lecturerName);
            }
            else if (auditLog.Action == "Modified")
            {
                await AddFieldsForModifiedActionAsync(referenceData, auditLog, courseId, departmentDict, positionDict, userDict, statusName, categoryName, lecturerName, hasStatusChange, previousDepartmentIds, previousPositionIds);
            }

            return referenceData;
        }

        private async Task<(Dictionary<string, string> departmentDict, Dictionary<string, string> positionDict, Dictionary<string, string> userDict, List<string> previousDepartmentIds, List<string> previousPositionIds)> GetCurrentReferenceDictionariesAsync(string courseId, AuditLog auditLog)
        {
            // Lấy CourseDepartment, CoursePosition và UserCourse
            var courseDepartments = await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == courseId);
            var departmentIds = courseDepartments.Select(cd => cd.DepartmentId.ToString()).ToList();

            var coursePositions = await _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == courseId);
            var positionIds = coursePositions.Select(cp => cp.PositionId.ToString()).ToList();

            var courseUsers = await _unitOfWork.UserCourseRepository.FindAsync(cp => cp.CourseId == courseId);
            var userIds = courseUsers.Select(cp => cp.UserId).ToList();

            var timeWindow = TimeSpan.FromSeconds(0.5);
            // Lấy ID từ bản ghi Deleted để đảm bảo ánh xạ đầy đủ
            var deletedDepartmentLogs = _allAuditLogs
                .Where(p => p.EntityName == "CourseDepartment" && p.Action == "Deleted" && p.Changes.Contains($"\"CourseId\":\"{courseId}\"") && p.Timestamp <= auditLog.Timestamp.Add(timeWindow))
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var previousDepartmentIds = ExtractIdsFromDeletedLogs(deletedDepartmentLogs, "DepartmentId");

            var deletedPositionLogs = _allAuditLogs
                .Where(p => p.EntityName == "CoursePosition" && p.Action == "Deleted" && p.Changes.Contains($"\"CourseId\":\"{courseId}\"") && p.Timestamp <= auditLog.Timestamp.Add(timeWindow))
                .OrderByDescending(p => p.Timestamp)
                .ToList();
            var previousPositionIds = ExtractIdsFromDeletedLogs(deletedPositionLogs, "PositionId");

            var deletedUserLogs = _allAuditLogs
                .Where(p => p.EntityName == "UserCourse" && p.Action == "Deleted" && p.Changes.Contains($"\"CourseId\":\"{courseId}\"") && p.Timestamp <= auditLog.Timestamp.Add(timeWindow))
                .ToList();
            var previousUserIds = ExtractIdsFromDeletedLogs(deletedUserLogs, "UserId");

            departmentIds.AddRange(previousDepartmentIds);
            positionIds.AddRange(previousPositionIds);
            userIds.AddRange(previousUserIds);

            // Lấy tên Department, Position, User
            var departments = await _unitOfWork.DepartmentRepository.FindAsync(d => departmentIds.Contains(d.DepartmentId.ToString()));
            var departmentDict = departments.ToDictionary(d => d.DepartmentId.ToString(), d => d.DepartmentName);

            var positions = await _unitOfWork.PositionRepository.FindAsync(p => positionIds.Contains(p.PositionId.ToString()));
            var positionDict = positions.ToDictionary(p => p.PositionId.ToString(), p => p.PositionName);

            var users = await _unitOfWork.UserRepository.FindAsync(p => userIds.Contains(p.Id));
            var userDict = users.ToDictionary(p => p.Id, p => p.FullName);

            // Ghi log để debug
            Console.WriteLine($"CourseId: {courseId}, DepartmentIds: {string.Join(", ", departmentIds)}");
            Console.WriteLine($"CourseId: {courseId}, PositionIds: {string.Join(", ", positionIds)}");
            Console.WriteLine($"CourseId: {courseId}, UserIds: {string.Join(", ", userIds)}");

            return (departmentDict, positionDict, userDict, previousDepartmentIds, previousPositionIds);
        }

        private async Task<(string statusName, string categoryName, string lecturerName,bool hasStatusChange)> GetCurrentCourseDetailsAsync(string courseId, AuditLog auditLog)
        {
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                    predicate: c => c.Id.ToString() == courseId,
                    includes: p => p
                        .Include(a => a.Category)
                        .Include(a => a.Status)
                        .Include(a => a.Lecturer)
               );
            string statusName = "Unknown";
            string categoryName = "Unknown";
            string lecturerName = "Unknown";

            if (course != null)
            {
                var status = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(s => s.Id == course.StatusId);
                statusName = status?.StatusName ?? "Unknown";

                if (course.CategoryId > 0)
                {
                    var category = await _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.Id == course.CategoryId);
                    categoryName = category?.CategoryName ?? "Unknown";
                }

                if (course.LecturerId > 0)
                {
                    var lecturer = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(l => l.Id == course.LecturerId);
                    lecturerName = lecturer?.FullName ?? "Unknown";
                }
            }

            // Kiểm tra xem Status có thay đổi trong Changes không
            var changes = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                auditLog.Changes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            bool hasStatusChange = changes?.OldValues?.ContainsKey("StatusId") == true || changes?.NewValues?.ContainsKey("StatusId") == true;

            return (statusName, categoryName, lecturerName, hasStatusChange);
        }

        private async Task AddFieldsForAddedActionAsync(
            ReferenceData referenceData,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> positionDict,
            Dictionary<string, string> userDict,
            string statusName,
            string categoryName,
            string lecturerName)
        {
            // Thêm Department vào AddedFields
            var departmentIds = (await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == courseId))
                .Select(cd => cd.DepartmentId.ToString()).ToList();
            if (departmentIds.Any())
            {
                var departmentNames = departmentIds
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

            // Thêm Position vào AddedFields
            var positionIds = (await _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == courseId))
                .Select(cp => cp.PositionId.ToString()).ToList();
            if (positionIds.Any())
            {
                var positionNames = positionIds
                    .Where(id => positionDict.ContainsKey(id))
                    .Select(id => positionDict[id])
                    .OrderBy(name => name)
                    .ToList();
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "EmploymentPosition",
                    Value = string.Join(", ", positionNames)
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

            // Thêm StatusName, CategoryName, LecturerName vào AddedFields
            if (statusName != "Unknown")
            {
                referenceData.AddedFields.Add(new AddedField { FieldName = "StatusName", Value = statusName });
            }
            if (categoryName != "Unknown")
            {
                referenceData.AddedFields.Add(new AddedField { FieldName = "CategoryName", Value = categoryName });
            }
            if (lecturerName != "Unknown")
            {
                referenceData.AddedFields.Add(new AddedField { FieldName = "LecturerName", Value = lecturerName });
            }
        }

        private async Task AddFieldsForModifiedActionAsync(
            ReferenceData referenceData,
            AuditLog auditLog,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> positionDict,
            Dictionary<string, string> userDict,
            string statusName,
            string categoryName,
            string lecturerName, bool hasStatusChange,
            List<string> previousDepartmentIds,
            List<string> previousPositionIds)
        {
            // Lấy các bản ghi Deleted trong khoảng thời gian gần với auditLog.Timestamp
            var timeWindow = TimeSpan.FromSeconds(0.5); // Khoảng thời gian 1 giây

            var deletedUserLogs = _allAuditLogs
                .Where(p => p.EntityName == "UserCourse" &&
                           p.Action == "Deleted" &&
                           p.Timestamp <= auditLog.Timestamp.Add(timeWindow) &&
                           p.Changes.Contains($"\"CourseId\":\"{courseId}\""))
                .OrderByDescending(p => p.Timestamp)
                .ToList();

            var previousUserIds = ExtractIdsFromDeletedLogs(deletedUserLogs, "UserId");

            // Lấy thông tin trạng thái trước đó
            var (previousStatusName, previousCategoryName, previousLecturerName) = await GetPreviousCourseDetailsAsync(auditLog, courseId);

            // So sánh và thêm vào ChangedFields
            await AddChangedFieldsAsync(referenceData, courseId, departmentDict, positionDict, userDict,
                previousDepartmentIds, previousPositionIds, previousUserIds,
                statusName, previousStatusName, categoryName, previousCategoryName, lecturerName, previousLecturerName, hasStatusChange);
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
                            ids.Add(changes.OldValues[idFieldName].ToString());
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

        private async Task<(string statusName, string categoryName, string lecturerName)> GetPreviousCourseDetailsAsync(AuditLog auditLog, string courseId)
        {
            string previousStatusName = "Unknown";
            string previousCategoryName = "Unknown";
            string previousLecturerName = "Unknown";

            // Truy vấn bảng Courses để lấy trạng thái trước đó
            var previousCourse = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id.ToString() == courseId && c.UpdatedAt < auditLog.Timestamp,
                includes: p => p
                    .Include(a => a.Category)
                    .Include(a => a.Status)
                    .Include(a => a.Lecturer));

            if (previousCourse != null)
            {
                previousStatusName = previousCourse.Status?.StatusName ?? "Unknown";
                previousCategoryName = previousCourse.Category?.CategoryName ?? "Unknown";
                previousLecturerName = previousCourse.Lecturer?.FullName ?? "Unknown";
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

                    if (previousChanges?.NewValues?.ContainsKey("LecturerId") == true)
                    {
                        var previousLecturerId = previousChanges.NewValues["LecturerId"]?.ToString();
                        var previousLecturer = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(l => l.Id.ToString() == previousLecturerId);
                        previousLecturerName = previousLecturer?.FullName ?? "Unknown";
                    }
                }
            }

            return (previousStatusName, previousCategoryName, previousLecturerName);
        }

        private async Task AddChangedFieldsAsync(
            ReferenceData referenceData,
            string courseId,
            Dictionary<string, string> departmentDict,
            Dictionary<string, string> positionDict,
            Dictionary<string, string> userDict,
            List<string> previousDepartmentIds,
            List<string> previousPositionIds,
            List<string> previousUserIds,
            string statusName,
            string previousStatusName,
            string categoryName,
            string previousCategoryName,
            string lecturerName,
            string previousLecturerName,
            bool hasStatusChange)
        {
            // Lấy danh sách tên hiện tại
            var currentDepartmentNames = (await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == courseId))
                .Select(cd => cd.DepartmentId.ToString())
                .Where(id => departmentDict.ContainsKey(id))
                .Select(id => departmentDict[id])
                .OrderBy(name => name)
                .ToList();

            var currentPositionNames = (await _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == courseId))
                .Select(cp => cp.PositionId.ToString())
                .Where(id => positionDict.ContainsKey(id))
                .Select(id => positionDict[id])
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

            var previousPositionNames = previousPositionIds
                .Where(id => positionDict.ContainsKey(id))
                .Select(id => positionDict[id])
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

            if (previousPositionNames.Any())
            {
                var positionOldValue = previousPositionNames.Any() ? string.Join(", ", previousPositionNames) : "None";
                var positionNewValue = currentPositionNames.Any() ? string.Join(", ", currentPositionNames) : "None";
                if (positionOldValue != positionNewValue)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "EmploymentPosition",
                        OldValue = positionOldValue,
                        NewValue = positionNewValue
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

            if (lecturerName != previousLecturerName)
            {
                referenceData.ChangedFields.Add(new ChangedField
                {
                    FieldName = "LecturerName",
                    OldValue = previousLecturerName,
                    NewValue = lecturerName
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