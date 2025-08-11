using System.Globalization;
using System.Text;

namespace QLDT_Becamex.Src.Shared.Helpers
{


    public static class StringHelper
    {
        public static string RemoveDiacritics(string text)
        {
            text = text.Replace("đ", "d").Replace("Đ", "D");

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
