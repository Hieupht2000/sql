using CarManagetment.DTOs;

namespace CarManagetment.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();
        Task<ServiceDTO> GetServiceByIdAsync(int id);
        Task<ServiceDTO> CreateServiceAsync(ServiceDTO serviceDto);
        Task<bool> UpdateServiceAsync(int id, ServiceDTO serviceDto);
        Task<bool> DeleteServiceAsync(int id);
    }
}
