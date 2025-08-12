using CloudinaryDotNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Departments.Handlers
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var id = command.Id;

                // Lấy phòng ban theo ID với các liên kết cần thiết
                var departments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: false, // Cần theo dõi để cập nhật/xóa
                    includes: q => q
                        .Include(d => d.Children)
                        .Include(d => d.Parent)
                        .Include(d => d.Manager)
                );

                if (!departments.Any())
                {
                    throw new AppException("Phòng ban không tồn tại", 404);
                }

                foreach (var Department in departments)
                {
                    var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.DepartmentId == Department.DepartmentId);
                    if (user != null)
                    {
                        Console.WriteLine("Không thể xóa vì còn tồn tại user trong phòng ban");
                        throw new AppException("Không thể xóa vì còn tồn tại user trong phòng ban", 409);
                    }
                    
                }

                var department = departments.First();

                // Lấy tất cả phòng ban để tra cứu
                var allDepartments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                var departmentDict = allDepartments.ToDictionary(d => d.DepartmentId, d => d);

                // Cập nhật DepartmentId của các user liên kết thành null
                var usersInDepartment = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: u => u.DepartmentId == id,
                    asNoTracking: false // Cần theo dõi để cập nhật
                );

                foreach (var user in usersInDepartment)
                {
                    var updateUser = new ApplicationUser
                    {
                        Id = user.Id,
                        DepartmentId = null,
                    };
                    _unitOfWork.UserRepository.Update(user, updateUser);
                }

                // Xử lý phòng ban con
                if (department.Children?.Any() == true)
                {
                    int newLevel;
                    int? newParentId;

                    if (department.Level == 1)
                    {
                        // Phòng ban cấp 1: con trở thành cấp 1, ParentId = null
                        newLevel = 1;
                        newParentId = null;
                    }
                    else
                    {
                        // Phòng ban không phải cấp 1: con kế thừa ParentId của phòng ban hiện tại
                        newLevel = department.ParentId.HasValue ? department.Level : 1;
                        newParentId = department.ParentId;
                    }

                    // Cập nhật các phòng ban con
                    await UpdateChildrenAfterDeleteAsync(department, newParentId, newLevel, departmentDict);
                }

                // Xóa phòng ban
                _unitOfWork.DepartmentRepository.Remove(department);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception)
            {
                throw new AppException("Vui lòng thử lại sau", 500);
            }
        }

        private async Task UpdateChildrenAfterDeleteAsync(
            Department parent,
            int? newParentId,
            int newLevel,
            Dictionary<int, Department> departmentDict)
        {
            if (parent.Children == null || !parent.Children.Any())
            {
                return;
            }

            foreach (var child in parent.Children)
            {
                child.ParentId = newParentId;
                child.Level = newLevel;
                child.StatusId = 2; //inactive
                child.UpdatedAt = DateTime.Now;

                departmentDict[child.DepartmentId] = child;

                // Đệ quy cập nhật các phòng ban con
                await UpdateChildrenAfterDeleteAsync(child, newParentId, newLevel + 1, departmentDict);
            }
        }
    }
}
