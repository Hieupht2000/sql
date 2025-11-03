using CarManagetment.Data;
using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.EntityFrameworkCore;

namespace CarManagetment.Services
{
    public class ServiceService : IServiceService
    {
        private readonly CarDBContext _context;
        public ServiceService(CarDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            return await _context.Servicess
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    EstimatedDuration = s.EstimatedDuration
                })
                .ToListAsync();
        }
        public async Task<ServiceDTO> GetServiceByIdAsync(int id)
        {
            var service = await _context.Servicess.FindAsync(id);
            if (service == null) return null;
            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                EstimatedDuration = service.EstimatedDuration
            };
        }

        public async Task<ServiceDTO> CreateServiceAsync(ServiceDTO serviceDto)
        {
            var service = new Servicess
            {
                Name = serviceDto.Name,
                Description = serviceDto.Description,
                Price = serviceDto.Price,
                EstimatedDuration = serviceDto.EstimatedDuration
            };
            _context.Servicess.Add(service);
            await _context.SaveChangesAsync();
            
            serviceDto.ServiceId = service.ServiceId;
            return serviceDto;
        }

        public async Task<bool> UpdateServiceAsync(int id, ServiceDTO serviceDto)
        {
            var service = await _context.Servicess.FindAsync(id);
            if (service == null) return false;
            service.Name = serviceDto.Name;
            service.Description = serviceDto.Description;
            service.Price = serviceDto.Price;
            service.EstimatedDuration = serviceDto.EstimatedDuration;
            _context.Servicess.Update(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Servicess.FindAsync(id);
            if (service == null) return false;
            _context.Servicess.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
