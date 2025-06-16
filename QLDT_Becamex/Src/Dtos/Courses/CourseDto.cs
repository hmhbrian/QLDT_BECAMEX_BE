using System;
using System.Collections.Generic;

namespace QLDT_Becamex.Src.Dtos.Courses
{
    public class CourseDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public string? Category { get; set; }
        public string? Instructor { get; set; }
        public int? DepartmentId { get; set; }
        public DurationDto? Duration { get; set; }
        public string? LearningType { get; set; }
        public string? Image { get; set; }
        public string? Status { get; set; }
        public List<string>? Department { get; set; }
        public List<string>? Level { get; set; }
        public string? Location { get; set; }
        public List<MaterialDto>? Materials { get; set; }
        public List<LessonDto>? Lessons { get; set; }
        public List<TestDto>? Tests { get; set; }
        public List<SyllabusItemDto>? Syllabus { get; set; }
        public List<string>? EnrolledTrainees { get; set; }
        public List<string>? Prerequisites { get; set; }
        public List<SlideDto>? Slides { get; set; }
        public string? CreatedAt { get; set; }
        public string? ModifiedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? EnrollmentType { get; set; }
        public string? RegistrationDeadline { get; set; }
        public bool IsPublic { get; set; }
        public int? MaxParticipants { get; set; }
    }

    public class DurationDto
    {
        public int? Sessions { get; set; }
        public double HoursPerSession { get; set; }
    }

    public class MaterialDto
    {
        public int? Id { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
    }

    public class LessonDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? ContentType { get; set; }
        public string? Content { get; set; }
        public string? Duration { get; set; }
    }

    public class QuestionDto
    {
        public int? Id { get; set; }
        public string? Text { get; set; }
        public List<string>? Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string? Explanation { get; set; }
    }

    public class TestDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public List<QuestionDto>? Questions { get; set; }
        public int PassingScorePercentage { get; set; }
    }

    public class SyllabusItemDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Duration { get; set; }
    }

    public class SlideDto
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
    }
}
