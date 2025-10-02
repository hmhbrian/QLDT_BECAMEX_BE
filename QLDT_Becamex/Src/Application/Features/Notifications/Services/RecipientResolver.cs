using Microsoft.CodeAnalysis.CSharp;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Linq;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Services
{
    public sealed class RecipientResolver : IRecipientResolver
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecipientResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<(int DeviceId, string Token)>> ResolveByDeptLevelAsync(IReadOnlyCollection<string> departmentIds, IReadOnlyCollection<string> levels, CancellationToken ct)
        {
            var hasDept = departmentIds != null && departmentIds.Count > 0;
            var hasLevel = levels != null && levels.Count > 0;

            var users = (await _unitOfWork.UserRepository.GetFlexibleAsync
                (
                    predicate: u => departmentIds.Contains(u.DepartmentId.ToString())
                                    && levels.Contains(u.ELevelId.ToString())
                )).ToList();

            var devices = (await _unitOfWork.DevicesRepository.GetFlexibleAsync
                (
                    predicate: d => users.Contains(d.User)
                )).ToList();
            return devices.Select(x => (x.Id, x.DeviceToken!)).ToList();
        }

        public async Task<List<(int DeviceId, string Token)>> ResolveByUserIdsAsync(
        IReadOnlyCollection<string> userIds, CancellationToken ct)
        {
            if (userIds == null || userIds.Count == 0) return new();
            var devices = (await _unitOfWork.DevicesRepository.GetFlexibleAsync
                (
                    predicate: d => userIds.Contains(d.User.Id)
                )).ToList();
            return devices.Select(x => (x.Id, x.DeviceToken!)).ToList();
        }
    }
}
