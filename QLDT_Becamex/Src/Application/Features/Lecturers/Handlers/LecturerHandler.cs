using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lecturer.Commands;
using QLDT_Becamex.Src.Application.Features.Lecturer.Dtos;
using QLDT_Becamex.Src.Application.Features.Lecturer.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Lecturer.Handlers
{
    public class LecturerHandler : IRequestHandler<GetAlLecturerQuery, IEnumerable<LecturerDto>>,
        IRequestHandler<CreateLecturerCommand, Unit>,
        IRequestHandler<UpdateLecturerCommand, Unit>,
        IRequestHandler<DeleteLecturerCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LecturerHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LecturerDto>> Handle(GetAlLecturerQuery request, CancellationToken cancellationToken)
        {
            var lecturers = await _unitOfWork.LecturerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LecturerDto>>(lecturers);
        }

        public async Task<Unit> Handle(CreateLecturerCommand request, CancellationToken cancellationToken)
        {
            var NameExisting = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.FullName.ToLower() == request.Request.FullName.ToLower());
            if (NameExisting != null)
                throw new AppException("Giảng viên đã tồn tại", 409);

            if(!string.IsNullOrEmpty(request.Request.Email?.Trim()))
            {
                var EmailExisting = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.Email!.ToLower() == request.Request.Email.ToLower());
                if (EmailExisting != null)
                    throw new AppException("Email giảng viên đã tồn tại", 409);
            }

            if (!string.IsNullOrEmpty(request.Request.PhoneNumber?.Trim()))
            {
                var PhoneExisting = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.PhoneNumber!.ToLower() == request.Request.PhoneNumber.ToLower());
                if (PhoneExisting != null)
                    throw new AppException("Số điện thoại giảng viên đã tồn tại", 409);
            }


            var lecturer = _mapper.Map<Domain.Entities.Lecturer>(request.Request);
            lecturer.FullName = request.Request.FullName.Trim();
            lecturer.Email = request.Request.Email?.Trim();
            lecturer.PhoneNumber = request.Request.PhoneNumber?.Trim();
            lecturer.ProfileImageUrl = request.Request.ProfileImageUrl?.Trim();

            await _unitOfWork.LecturerRepository.AddAsync(lecturer);
            await _unitOfWork.CompleteAsync();
            _mapper.Map<LecturerDto>(lecturer);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateLecturerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.LecturerRepository.GetByIdAsync(request.id);
            if (entity == null)
                throw new AppException("Không tìm thấy giảng viên", 404);

            var conflict = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.FullName.ToLower() == request.Request.FullName.ToLower() && l.Id != request.id);
            if (conflict != null)
                throw new AppException("Tên giảng viên đã tồn tại", 409);

            if (!string.IsNullOrEmpty(request.Request.Email?.Trim()))
            {
                var EmailExisting = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.Email!.ToLower() == request.Request.Email.ToLower());
                if (EmailExisting != null)
                    throw new AppException("Email giảng viên đã tồn tại", 409);
            }

            if (!string.IsNullOrEmpty(request.Request.PhoneNumber?.Trim()))
            {
                var PhoneExisting = await _unitOfWork.LecturerRepository.GetFirstOrDefaultAsync(
                l => l.PhoneNumber!.ToLower() == request.Request.PhoneNumber.ToLower());
                if (PhoneExisting != null)
                    throw new AppException("Số điện thoại giảng viên đã tồn tại", 409);
            }

            entity.FullName = request.Request.FullName;
            entity.Email = request.Request.Email;
            entity.PhoneNumber = request.Request.PhoneNumber;
            entity.ProfileImageUrl = request.Request.ProfileImageUrl;
            _unitOfWork.LecturerRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteLecturerCommand request, CancellationToken cancellationToken)
        {
            var lecturers = await _unitOfWork.LecturerRepository.FindAsync(l => request.Ids.Contains(l.Id));

            if (lecturers == null || !lecturers.Any())
                throw new AppException("Không tìm thấy giảng viên nào với ID đã cho", 404);

            _unitOfWork.LecturerRepository.RemoveRange(lecturers);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
