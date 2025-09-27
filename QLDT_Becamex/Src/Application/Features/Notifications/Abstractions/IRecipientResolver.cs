namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface IRecipientResolver
    {
        /// Trả về (DeviceId, Token) cho học viên thỏa điều kiện Dept/Level của khóa học.
        Task<List<(int DeviceId, string Token)>> ResolveStudentDeviceTokensAsync(
            IReadOnlyCollection<string> departmentIds,
            IReadOnlyCollection<string> levels,
            CancellationToken ct);
    }
}
