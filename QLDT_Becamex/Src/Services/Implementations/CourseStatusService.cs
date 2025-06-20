using AutoMapper;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.UnitOfWork;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class CourseStatusService : ICourseStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CourseSatusDto>>> GetAllAsync()
        {
            try
            {
                var list = await _unitOfWork.CourseStatusRepository.GetAllAsync();
                var mapped = _mapper.Map<IEnumerable<CourseSatusDto>>(list);
                return Result<IEnumerable<CourseSatusDto>>.Success(data: mapped);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CourseSatusDto>>.Failure(
                    error: ex.Message,
                    message: "An error occurred while retrieving course statuses.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> CreateAsync(CourseStatusDtoRq rq)
        {
            try
            {
                var existing = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(
                    predicate: us => us.Name.ToLower() == rq.Name.ToLower()
                );

                if (existing != null)
                {
                    return Result.Failure(
                        error: "CourseStatus with this name already exists.",
                        code: "CONFLICT",
                        statusCode: 409
                    );
                }

                var courseStatus = new CourseSatus { Name = rq.Name };

                await _unitOfWork.CourseStatusRepository.AddAsync(courseStatus);
                await _unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: ex.Message,
                    message: "An error occurred while creating the course status.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> UpdateAsync(int id, CourseStatusDtoRq rq)
        {
            try
            {
                var entity = await _unitOfWork.CourseStatusRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return Result.Failure(
                        error: "CourseStatus not found.",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }

                entity.Name = rq.Name;
                _unitOfWork.CourseStatusRepository.Update(entity);
                await _unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: ex.Message,
                    message: "An error occurred while updating the course status.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> DeleteAsync(List<int> ids)
        {
            try
            {
                // Lấy tất cả các entity cần xóa
                var entities = await _unitOfWork.CourseStatusRepository.FindAsync(cs => ids.Contains(cs.Id));

                if (entities == null || !entities.Any())
                {
                    return Result.Failure(
                        error: "No CourseStatus found with the provided IDs.",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }

                _unitOfWork.CourseStatusRepository.RemoveRange(entities);
                await _unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: ex.Message,
                    message: "An error occurred while deleting course statuses.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

    }
}
