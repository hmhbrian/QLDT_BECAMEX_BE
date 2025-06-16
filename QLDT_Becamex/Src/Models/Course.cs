using System;
using System.Collections.Generic;

namespace QLDT_Becamex.Src.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sessions { get; set; }
        public double HoursPerSession { get; set; }
        public string LearningType { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string EnrollmentType { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int? MaxParticipants { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;

        public ICollection<CourseDepartment> CourseDepartments { get; set; }
        public ICollection<CourseLevel> CourseLevels { get; set; }
        public ICollection<CoursePrerequisite> Prerequisites { get; set; }
        public ICollection<CourseTrainee> CourseTrainees { get; set; }
        public ICollection<Material> Materials { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Test> Tests { get; set; }
        public ICollection<SyllabusItem> SyllabusItems { get; set; }
        public ICollection<Slide> Slides { get; set; }
        
        public Course()
        {
            CourseDepartments = new List<CourseDepartment>();
            CourseLevels = new List<CourseLevel>();
            Prerequisites = new List<CoursePrerequisite>();
            CourseTrainees = new List<CourseTrainee>();
            Materials = new List<Material>();
            Lessons = new List<Lesson>();
            Tests = new List<Test>();
            SyllabusItems = new List<SyllabusItem>();
            Slides = new List<Slide>();
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }
    }

    public class CourseLevel
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public string Level { get; set; } = string.Empty;
    }

    public class Material
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }

    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PassingScorePercentage { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public ICollection<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
    }

    public class SyllabusItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int OrderIndex { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }    
    
    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }

    public class CoursePrerequisite
    {
        public int CourseId { get; set; }
        public int PrerequisiteId { get; set; }
        public Course Course { get; set; } = null!;
        public Course PrerequisiteCourse { get; set; } = null!;
    }

    public class CourseTrainee
    {
        public int CourseId { get; set; }
        public string TraineeId { get; set; } = string.Empty;
        public Course Course { get; set; } = null!;
        public ApplicationUser Trainee { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? Score { get; set; }
        public string? CompletionCertificate { get; set; }
    }
}
