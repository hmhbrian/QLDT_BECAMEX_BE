using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Text.Json;

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

        public ReferenceData GetReferenceData(AuditLog auditLog)
        {
            var referenceData = new ReferenceData();
            var courseId = auditLog.EntityId;

            // Lấy CourseDepartment và CoursePosition
            var courseDepartments = _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == courseId).Result;
            var departmentIds = courseDepartments.Select(cd => cd.DepartmentId).ToList();

            var coursePositions = _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == courseId).Result;
            var positionIds = coursePositions.Select(cp => cp.PositionId).ToList();

            // Lấy tên Department và Position
            var departments = _unitOfWork.DepartmentRepository.FindAsync(d => departmentIds.Contains(d.DepartmentId)).Result;
            var departmentDict = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var positions = _unitOfWork.PositionRepository.FindAsync(p => positionIds.Contains(p.PositionId)).Result;
            var positionDict = positions.ToDictionary(p => p.PositionId, p => p.PositionName);

            // Lấy thông tin Status, Category, Lecturer
            var course = _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(c => c.Id.ToString() == courseId).Result;
            string statusName = "Unknown";
            string categoryName = "Unknown";
            string lecturerName = "Unknown";

            if (course != null)
            {
                var status = _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(s => s.Id == course.StatusId).Result;
                statusName = status?.StatusName ?? "Unknown";

                if(course.CategoryId > 0)
                {
                    var category = _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.Id == course.CategoryId).Result;
                    categoryName = category?.CategoryName ?? "Unknown";
                }

                if (course.LecturerId > 0)
                {
                    var lecturer = _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(l => l.Id == course.LecturerId).Result;
                    lecturerName = lecturer?.FullName ?? "Unknown";
                }
                
            }

            if (auditLog.Action == "Added")
            {
                // Thêm Department vào AddedFields
                if (departmentIds.Any())
                {
                    var departmentNames = departmentIds
                        .Where(id => departmentDict.ContainsKey(id))
                        .Select(id => departmentDict[id])
                        .OrderBy(name => name)
                        .ToList();
                    if (departmentNames.Any())
                    {
                        referenceData.AddedFields.Add(new AddedField
                        {
                            FieldName = "Department",
                            Value = string.Join(", ", departmentNames)
                        });
                    }
                }

                // Thêm Position vào AddedFields
                if (positionIds.Any())
                {
                    var positionNames = positionIds
                        .Where(id => positionDict.ContainsKey(id))
                        .Select(id => positionDict[id])
                        .OrderBy(name => name)
                        .ToList();
                    if (positionNames.Any())
                    {
                        referenceData.AddedFields.Add(new AddedField
                        {
                            FieldName = "EmploymentPosition",
                            Value = string.Join(", ", positionNames)
                        });
                    }
                }

                // Thêm StatusName, CategoryName, LecturerName vào AddedFields
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "StatusName",
                    Value = statusName
                });
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "CategoryName",
                    Value = categoryName
                });
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "LecturerName",
                    Value = lecturerName
                });
            }
            else if (auditLog.Action == "Modified")
            {
                // Lấy audit log trước đó để so sánh
                var previousAuditLog = _allAuditLogs
                    .Where(p => p.EntityName == "Courses" && p.EntityId == auditLog.EntityId && p.Timestamp < auditLog.Timestamp)
                    .OrderByDescending(p => p.Timestamp)
                    .FirstOrDefault();

                // Lấy danh sách Department và Position từ audit log trước đó (nếu có)
                var previousDepartmentIds = new List<string>();
                var previousPositionIds = new List<string>();
                string previousStatusName = "Unknown";
                string previousCategoryName = "Unknown";
                string previousLecturerName = "Unknown";

                if (previousAuditLog != null && !string.IsNullOrEmpty(previousAuditLog.Changes))
                {
                    var previousChanges = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                        previousAuditLog.Changes,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (previousChanges?.NewValues?.ContainsKey("Department") == true)
                    {
                        previousDepartmentIds = (previousChanges.NewValues["Department"]?.ToString() ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(d => d.Trim())
                            .ToList();
                    }

                    if (previousChanges?.NewValues?.ContainsKey("Position") == true)
                    {
                        previousPositionIds = (previousChanges.NewValues["Position"]?.ToString() ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToList();
                    }

                    if (previousChanges?.NewValues?.ContainsKey("StatusId") == true)
                    {
                        var previousStatusId = previousChanges.NewValues["StatusId"]?.ToString();
                        var previousStatus = _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(s => s.Id.ToString() == previousStatusId).Result;
                        previousStatusName = previousStatus?.StatusName ?? "Unknown";
                    }

                    if (previousChanges?.NewValues?.ContainsKey("CategoryId") == true)
                    {
                        var previousCategoryId = previousChanges.NewValues["CategoryId"]?.ToString();
                        var previousCategory = _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.Id.ToString() == previousCategoryId).Result;
                        previousCategoryName = previousCategory?.CategoryName ?? "Unknown";
                    }

                    if (previousChanges?.NewValues?.ContainsKey("LecturerId") == true)
                    {
                        var previousLecturerId = previousChanges.NewValues["LecturerId"]?.ToString();
                        var previousLecturer = _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(l => l.Id.ToString() == previousLecturerId).Result;
                        previousLecturerName = previousLecturer?.FullName ?? "Unknown";
                    }
                }

                // So sánh danh sách Department
                var currentDepartmentNames = departmentIds
                    .Where(id => departmentDict.ContainsKey(id))
                    .Select(id => departmentDict[id])
                    .OrderBy(name => name)
                    .ToList();
                if (currentDepartmentNames.Any() && !currentDepartmentNames.SequenceEqual(previousDepartmentIds))
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "Department",
                        OldValue = string.Join(", ", previousDepartmentIds),
                        NewValue = string.Join(", ", currentDepartmentNames)
                    });
                }

                // So sánh danh sách Position
                var currentPositionNames = positionIds
                    .Where(id => positionDict.ContainsKey(id))
                    .Select(id => positionDict[id])
                    .OrderBy(name => name)
                    .ToList();
                if (currentPositionNames.Any() && !currentPositionNames.SequenceEqual(previousPositionIds))
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "EmploymentPosition",
                        OldValue = string.Join(", ", previousPositionIds),
                        NewValue = string.Join(", ", currentPositionNames)
                    });
                }

                // So sánh StatusName, CategoryName, LecturerName
                if (statusName != previousStatusName)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "StatusName",
                        OldValue = previousStatusName,
                        NewValue = statusName
                    });
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
            }

            return referenceData;
        }
    }
}
