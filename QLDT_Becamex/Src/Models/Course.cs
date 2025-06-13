using System;
using System.Collections.Generic;

namespace QLDT_Becamex.Src.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public string Description { get; set; }
        public string Objectives { get; set; }
        public string Category { get; set; }
        public string Instructor { get; set; }
        public int Sessions { get; set; }
        public double HoursPerSession { get; set; }
        public string LearningType { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string Department { get; set; } // Có thể là List<string> nếu cần
        public string Level { get; set; } // Có thể là List<string> nếu cần
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string EnrollmentType { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public bool IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        // Navigation properties
        public ICollection<Material> Materials { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Test> Tests { get; set; }
        public ICollection<SyllabusItem> Syllabus { get; set; }
        public ICollection<string> EnrolledTrainees { get; set; }
        public int? MaxParticipants { get; set; }
        public ICollection<string> Prerequisites { get; set; }
        public ICollection<Slide> Slides { get; set; }
    }

    public class Material
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string Duration { get; set; }
    }

    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Question> Questions { get; set; }
        public int PassingScorePercentage { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }

    public class SyllabusItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Duration { get; set; }
    }

    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }
}
