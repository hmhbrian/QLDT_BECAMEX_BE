namespace QLDT_Becamex.Src.Models
{
    public class CoursePosition
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public int PositionId { get; set; }
        public Course Course { get; set; } = null!;
        public Position Position { get; set; } = null!;



    }
}
