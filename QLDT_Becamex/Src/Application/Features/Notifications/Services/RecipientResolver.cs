using Microsoft.CodeAnalysis.CSharp;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Services
{
    public sealed class RecipientResolver : IRecipientResolver
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecipientResolver (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<(int DeviceId, string Token)>> ResolveStudentDeviceTokensAsync(IReadOnlyCollection<string> departmentIds, IReadOnlyCollection<string> levels, CancellationToken ct)
        {
            var hasDept = departmentIds != null && departmentIds.Count > 0;
            var hasLevel = levels != null && levels.Count > 0;

            var userIds = (await _unitOfWork.UserRepository.GetFlexibleAsync
                (
                    predicate: u => departmentIds.Contains(u.DepartmentId.ToString()) 
                                    && levels.Contains(u.ELevelId.ToString())
                )).ToList();

            var devices = (await _unitOfWork.DevicesRepository.GetFlexibleAsync
                (
                    predicate: d => userIds.Contains(d.User)
                )).ToList();
            return devices.Select(x => (x.Id, x.DeviceToken!)).ToList();
        }
    }
}
