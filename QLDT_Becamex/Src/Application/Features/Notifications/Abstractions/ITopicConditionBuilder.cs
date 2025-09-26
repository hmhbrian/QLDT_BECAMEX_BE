namespace QLDT_Becamex.Src.Application.Features.Notifications.Abstractions
{
    public interface ITopicConditionBuilder
    {
        /// Trả về danh sách FCM conditions:
        /// - dept & level:  "'dept_42' in topics && 'level_L3' in topics"
        /// - chỉ dept:      "'dept_42' in topics"
        /// - chỉ level:     "'level_L3' in topics"
        IEnumerable<string> BuildConditions(
            IEnumerable<string>? departmentIds,
            IEnumerable<string>? levels);
    }
}
