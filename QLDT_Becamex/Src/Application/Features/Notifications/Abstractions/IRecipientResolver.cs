namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface IRecipientResolver
    {
        /// Trả về (DeviceId, Token) cho học viên thỏa điều kiện Dept/Level của khóa học.
        Task<List<(int DeviceId, string Token)>> ResolveByDeptLevelAsync(
            IReadOnlyCollection<string> departmentIds,
            IReadOnlyCollection<string> levels,
            CancellationToken ct);

        // Token theo danh sách userId (mandatory)
        Task<List<(int DeviceId, string Token)>> ResolveByUserIdsAsync(
            IReadOnlyCollection<string> userIds,
            CancellationToken ct);
    }
}
