using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;

namespace QLDT_Becamex.Src.Application.Features.Notifications.Services
{
    public sealed class TopicConditionBuilder : ITopicConditionBuilder
    {
        private readonly string _deptPrefix;
        private readonly string _levelPrefix;

        public TopicConditionBuilder(IConfiguration cfg)
        {
            _deptPrefix = cfg["Notification:TopicPrefixes:Department"] ?? "dept_";
            _levelPrefix= cfg
                ["Notification:TopicPrefixes:Level"] ?? "level_";
        }
        public IEnumerable<string> BuildConditions(IEnumerable<string> departmentIds, IEnumerable<string> levels)
        {
            var depts = (departmentIds ?? Enumerable.Empty<string>()).Distinct().ToList();
            var lvls = (levels ?? Enumerable.Empty<string>()).Distinct().ToList();

            var hasDept = depts.Count > 0;
            var hasLevel = lvls.Count > 0;

            if (!hasDept && !hasLevel)
            {
                // Có thể gửi qua broadcast theo 1 topic chung.
                yield break; 
            }

            if (hasDept && hasLevel)
            {
                // Tổ hợp (dept && level)
                foreach (var d in depts)
                    foreach (var l in lvls)
                    {
                        var deptTopic = $"{_deptPrefix}{d}";
                        var lvlTopic = $"{_levelPrefix}{l}";
                        yield return $"'{deptTopic}' in topics && '{lvlTopic}' in topics";
                    }
                yield break;
            }

            if (hasDept) // chỉ phòng ban
            {
                foreach (var d in depts)
                {
                    var deptTopic = $"{_deptPrefix}{d}";
                    yield return $"'{deptTopic}' in topics";
                }
                yield break;
            }

            // chỉ cấp độ
            foreach (var l in lvls)
            {
                var lvlTopic = $"{_levelPrefix}{l}";
                yield return $"'{lvlTopic}' in topics";
            }
        }
    }
}
