using QLDT_Becamex.Src.Application.Common;
using QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Text.Json;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.DataProvider
{
    public class AttachFileReferenceDataProvider : IEntityReferenceDataProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly List<AuditLog> _allAuditLogs;

        public AttachFileReferenceDataProvider(IUnitOfWork unitOfWork, List<AuditLog> allAuditLogs)
        {
            _unitOfWork = unitOfWork;
            _allAuditLogs = allAuditLogs;
        }

        public async Task<ReferenceData> GetReferenceData(AuditLog auditLog)
        {
            var referenceData = new ReferenceData();
            var attachFileId = auditLog.EntityId;

            // Lấy thông tin Lesson
            var attachFile = await _unitOfWork.CourseAttachedFileRepository.GetFirstOrDefaultAsync(predicate: q => q.Id.ToString() == attachFileId);
            string documentTypeName = "Unknown";

            if (attachFile != null)
            {
                var documentType = await _unitOfWork.TypeDocumentRepository.GetFirstOrDefaultAsync(dt => dt.Id == attachFile.TypeDocId);
                documentTypeName = documentType?.NameType ?? "Unknown";
            }

            if (auditLog.Action == "Added")
            {
                // ThêmDocumentTypeName vào AddedFields
                referenceData.AddedFields.Add(new AddedField
                {
                    FieldName = "NameType",
                    Value = documentTypeName
                });
            }
            else if (auditLog.Action == "Modified")
            {
                // Lấy audit log trước đó để so sánh
                var previousAuditLog = _allAuditLogs
                    .Where(p => p.EntityName == "CourseAttachedFile" && p.EntityId == auditLog.EntityId && p.Timestamp < auditLog.Timestamp)
                    .OrderByDescending(p => p.Timestamp)
                    .FirstOrDefault();

                string previousDocumentTypeName = "Unknown";

                if (previousAuditLog != null && !string.IsNullOrEmpty(previousAuditLog.Changes))
                {
                    var previousChanges = JsonSerializer.Deserialize<AuditLogMapper.AuditLogChanges>(
                        previousAuditLog.Changes,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (previousChanges?.NewValues?.ContainsKey("TypeDocId") == true)
                    {
                        var previousTypeDocId = previousChanges.NewValues["TypeDocId"]?.ToString();
                        var previousDocumentType = _unitOfWork.TypeDocumentRepository.GetFirstOrDefaultAsync(dt => dt.Id.ToString() == previousTypeDocId).Result;
                        previousDocumentTypeName = previousDocumentType?.NameType ?? "Unknown";
                    }
                }

                // So sánh DocumentTypeName
                if (documentTypeName != previousDocumentTypeName)
                {
                    referenceData.ChangedFields.Add(new ChangedField
                    {
                        FieldName = "NameType",
                        OldValue = previousDocumentTypeName,
                        NewValue = documentTypeName
                    });
                }
            }
            return referenceData;
        }
    }
}