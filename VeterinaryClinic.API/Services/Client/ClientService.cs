using VeterinaryClinic.API.Dtos.Client;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Services
{
    public class ClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ClientResponseDto>> GetAllAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return clients.Select(c => new ClientResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                IsApproved = c.User?.IsApproved ?? false,
                Address = c.Address,
                Name = c.User?.Name ?? string.Empty,
                Email = c.User?.Email ?? string.Empty,
                Phone = c.User?.Phone ?? string.Empty
            });
        }

        public async Task<ClientResponseDto?> GetByIdAsync(int id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client is null) return null;

            return new ClientResponseDto
            {
                Id = client.Id,
                Address = client.Address,
                Name = client.User?.Name ?? string.Empty,
                Email = client.User?.Email ?? string.Empty,
                Phone = client.User?.Phone ?? string.Empty
            };
        }

        public async Task<ClientResponseDto?> GetByUserIdAsync(int userId)
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            var client = clients.FirstOrDefault(c => c.UserId == userId);
            if (client is null) return null;

            return new ClientResponseDto
            {
                Id = client.Id,
                Address = client.Address,
                Name = client.User?.Name ?? string.Empty,
                Email = client.User?.Email ?? string.Empty,
                Phone = client.User?.Phone ?? string.Empty
            };
        }

        public async Task<ClientResponseDto?> UpdateAsync(int id, ClientUpdateDto dto)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client is null) return null;

            client.Address = dto.Address;

            if (client.User is not null)
            {
                client.User.Name = dto.Name;
                client.User.Phone = dto.Phone;
            }

            await _unitOfWork.Clients.UpdateAsync(client);

            return new ClientResponseDto
            {
                Id = client.Id,
                Address = client.Address,
                Name = client.User?.Name ?? string.Empty,
                Email = client.User?.Email ?? string.Empty,
                Phone = client.User?.Phone ?? string.Empty
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client is null) return false;

            await _unitOfWork.Clients.DeleteAsync(id);
            return true;
        }

    }
}