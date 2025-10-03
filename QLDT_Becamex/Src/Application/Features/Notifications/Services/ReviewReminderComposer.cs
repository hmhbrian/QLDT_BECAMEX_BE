using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Services
{
    public class ReviewReminderComposer : IReviewReminderComposer
    {
        public (string Title, string Body, Dictionary<string, string> Data) Build(string fullName, string courseTitle, string courseId)
        {
            var safeName = string.IsNullOrWhiteSpace(fullName) ? "Bạn" : fullName;
            var safeTitle = string.IsNullOrWhiteSpace(courseTitle) ? "khóa học" : courseTitle;

            var title = "Đánh giá khóa học";
            var body = $"{safeName} ơi, bạn đã hoàn thành \"{safeTitle}\" 🎉\n"
                      + "Dành 1 phút chia sẻ cảm nhận để ứng dụng được cải thiện tốt hơn nhé!";

            var data = new Dictionary<string, string>
            {
                ["type"] = "CourseDetail",
                ["courseId"] = courseId
            };

            return (title, body, data);
        }
    }
}
