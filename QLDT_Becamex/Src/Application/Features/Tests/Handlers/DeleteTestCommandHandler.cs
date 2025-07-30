using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class DeleteTestCommandHandler : IRequestHandler<DeleteTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteTestCommand command, CancellationToken cancellationToken)
        {
            // Kiểm tra Test existence
            var test = await _unitOfWork.TestRepository.GetByIdAsync(command.Id);
            if (test == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }

            if (string.IsNullOrEmpty(test.CourseId))
            {
                throw new AppException("CourseId không hợp lệ", 400);
            }
            string courseId = test.CourseId;

            //Kiểm tra khóa học được phép xóa hay không
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: a => a.Id == courseId,
                includes: p => p.Include(a => a.Status));

            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            if (course.Status != null && course.Status.Key > 1)
            {
                throw new AppException("Không thể xóa bài kiểm tra vì đã bắt đầu", 403);
            }

            //Kiểm tra test đã có ai làm chưa
            var TestExist = await _unitOfWork.TestResultRepository
                   .GetFirstOrDefaultAsync(tr => tr.TestId == command.Id);
            if(TestExist != null) 
            {
                throw new AppException("Không thể xóa bài kiểm tra vì đã có người tham gia", 403);
            }
            // Remove Test from repository
            _unitOfWork.TestRepository.Remove(test);
            await _unitOfWork.CompleteAsync();

            var allTestOfCourse = (await _unitOfWork.TestRepository.GetFlexibleAsync(predicate: t => t.CourseId == courseId)).ToList();

            int position = 1;
            foreach (var t in allTestOfCourse.OrderBy(t => t.Position))
            {
                t.Position = position++;
                t.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.TestRepository.UpdateEntity(t);
            }

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}