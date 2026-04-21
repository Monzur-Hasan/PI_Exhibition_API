using Domain.Models.Head_Model.DomainModel;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using Infrastructure.Features.Repository.Interface;
using Infrastructure.Features.Service.Interface;
using Microsoft.Extensions.Logging;

namespace Application.Features.Service.Implementation
{
    public class HeadService : IHeadService
    {
        private readonly IHeadRepository _repository;
        private readonly ILogger<HeadService> _logger;

        public HeadService(IHeadRepository repository, ILogger<HeadService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResponse<Head>> GetHeadAsync(Pagination_Filter filter)
        {
            try
            {
                return await _repository.GetHeadAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHeadAsync");
                throw;
            }
        }

        public async Task<Head?> GetHeadByIdAsync(int id, string userId)
        {
            try
            {
                return await _repository.GetHeadByIdAsync(id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHeadByIdAsync");
                throw;
            }
        }

        public async Task<int> SaveHeadAsync(Head entity)
        {
            try
            {
                return await _repository.SaveHeadAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveHeadAsync");
                throw;
            }
        }

        public async Task<int> UpdateHeadAsync(Head entity)
        {
            try
            {
                return await _repository.UpdateHeadAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHeadAsync");
                throw;
            }
        }

        public async Task<int> DeleteHeadAsync(int id, string userId)
        {
            try
            {
                return await _repository.DeleteHeadAsync(id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHeadAsync");
                throw;
            }
        }
    }
}
