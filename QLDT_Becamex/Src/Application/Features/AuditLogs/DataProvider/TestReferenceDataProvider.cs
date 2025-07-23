using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.DataProvider
{
    public class TestReferenceDataProvider : IEntityReferenceDataProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<AuditLog> _allAuditLogs;

        public TestReferenceDataProvider(IUnitOfWork unitOfWork, List<AuditLog> allAuditLogs)
        {
            _unitOfWork = unitOfWork;
            _allAuditLogs = allAuditLogs;
        }

        public async Task<ReferenceData> GetReferenceData(AuditLog auditLog)
        {
            var referenceData = new ReferenceData();
            var testId = auditLog.EntityId;

            if (auditLog.Action == "Added")
            {
                var test = await _unitOfWork.TestRepository.GetFirstOrDefaultAsync(predicate: q => q.Id.ToString() == testId);
                if (test != null)
                {
                    var course = _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(predicate: c => c.Id.ToString() == test.CourseId).Result;
                    if (course != null)
                    {
                        referenceData.AddedFields.Add(new AddedField
                        {
                            FieldName = "CourseName",
                            Value = course.Name ?? "Unknown Course"
                        });
                    }
                }
            }

            // Thêm logic cho Modified
            return referenceData;
        }
    }
}
