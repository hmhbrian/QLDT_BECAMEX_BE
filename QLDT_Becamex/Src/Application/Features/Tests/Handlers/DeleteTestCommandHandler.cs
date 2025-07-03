using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

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

            // Remove Test from repository
            _unitOfWork.TestRepository.Remove(test);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}