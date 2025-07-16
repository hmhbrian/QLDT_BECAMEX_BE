using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class UpdatePositionTestCommandHandler : IRequestHandler<UpdatePositionTestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public UpdatePositionTestCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task Handle(UpdatePositionTestCommand request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }
            // Kiểm tra test có tồn tại không
            var existingTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existingTest == null)
            {
                throw new AppException($"Test with ID: {request.TestId} not found.", 404);
            }

            //Kiểm tra test cần di chuyển có tồn tại không
            var TestToMove = await _unitOfWork.TestRepository.GetFirstOrDefaultAsync(
                predicate: l => l.Id == request.TestId && l.CourseId == request.CourseId
            );
            if (TestToMove == null)
            {
                throw new AppException($"Test with ID: {request.TestId} not found.", 404);
            }

            // Transaction giúp đảm bảo tính toàn vẹn dữ liệu khi cập nhật position của test
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                int newPosition;
                if (request?.PreviousTestId == 0)
                {
                    //Nếu không có PreviousTestId thì đặt vị trí là 1
                    newPosition = 1;
                }
                else
                {
                    //Kiểm tra test trước đó có tồn tại không
                    var previousTest = await _unitOfWork.TestRepository.GetFirstOrDefaultAsync(
                        predicate: l => l.Id == request!.PreviousTestId && l.CourseId == request.CourseId
                    );
                    if (previousTest == null)
                    {
                        throw new AppException($"Previous test with ID: {request!.PreviousTestId} not found.", 404);
                    }
                    //Đặt vị trí mới là vị trí của test trước đó + 1
                    newPosition = previousTest.Position + 1;
                }

                //Lấy Position hiện tại của Test để so sánh
                int oldPosition = TestToMove.Position;

                //Cập nhật position của các test cùng khóa học
                if (newPosition != oldPosition)
                {
                    if (newPosition < oldPosition)
                    {
                        //Nếu vị trí mới nhỏ hơn vị trí cũ, tăng position của các Test từ newPosition đến oldPosition - 1
                        await _unitOfWork.TestRepository.UpdatePositionAsync(request?.CourseId!, newPosition, oldPosition - 1, 1);
                    }
                    else
                    {
                        //Nếu vị trí mới lớn hơn vị trí cũ, giảm position của các Test từ oldPosition + 1 đến newPosition
                        newPosition = newPosition - 1; // Giảm vị trí mới để tránh xung đột với TestToMove
                        await _unitOfWork.TestRepository.UpdatePositionAsync(request?.CourseId!, oldPosition + 1, newPosition, -1);
                    }
                    TestToMove.Position = newPosition; // Cập nhật vị trí mới cho test
                    TestToMove.UpdatedAt = DateTime.UtcNow;
                    TestToMove.UpdatedById = userId;
                    //_unitOfWork.TestRepository.Update(TestToMove); // Cập nhật test

                    await _unitOfWork.CompleteAsync(); // Lưu thay đổi
                }

                await transaction.CommitAsync(cancellationToken); // Commit transaction
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException($"An error occurred while updating the test position: {ex.Message}", 500);
            }
        }
    }
}
