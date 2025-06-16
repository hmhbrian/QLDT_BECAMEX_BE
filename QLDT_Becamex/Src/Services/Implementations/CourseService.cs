using System.Collections.Generic;
using System.Threading.Tasks;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Config;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CourseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses
                .Include(c => c.Materials)
                .Include(c => c.Lessons)
                .Include(c => c.Tests)
                    .ThenInclude(t => t.Questions)
                .Include(c => c.SyllabusItems)
                .Include(c => c.Slides)
                .Include(c => c.CourseDepartments)
                .Include(c => c.CourseLevels)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Materials)
                .Include(c => c.Lessons)
                .Include(c => c.Tests)
                    .ThenInclude(t => t.Questions)
                .Include(c => c.SyllabusItems)
                .Include(c => c.Slides)
                .Include(c => c.CourseDepartments)
                .Include(c => c.CourseLevels)
                .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<CourseDto>(course);
        }        public async Task<CourseDto> CreateCourseAsync(CourseDto dto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required");
            if (string.IsNullOrWhiteSpace(dto.CourseCode))
                throw new ArgumentException("CourseCode is required");
            if (string.IsNullOrWhiteSpace(dto.Instructor))
                throw new ArgumentException("Instructor is required");
            if (dto.DepartmentId == 0)
                throw new ArgumentException("DepartmentId is required");

            var course = _mapper.Map<Course>(dto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDto>(course);
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);            if (course == null) throw new KeyNotFoundException("Course not found");
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required");
            if (string.IsNullOrWhiteSpace(dto.CourseCode))
                throw new ArgumentException("CourseCode is required");
            if (string.IsNullOrWhiteSpace(dto.Instructor))
                throw new ArgumentException("Instructor is required");
            if (dto.DepartmentId == 0)
                throw new ArgumentException("DepartmentId is required");
            _mapper.Map(dto, course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
