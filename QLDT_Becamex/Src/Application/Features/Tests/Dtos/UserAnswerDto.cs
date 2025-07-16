
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class UserAnswerDto
    {
        [Required]
        public int QuestionId { get; set; }
        public List<string> SelectedOptions { get; set; } = new List<string>();
    }
}
