using Domain.Models.Head_Model.DomainModel;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Features.Repository.Interface
{
    public interface IHeadRepository
    {
        Task<PagedResponse<Head>> GetHeadAsync(Pagination_Filter filter);
        Task<Head?> GetHeadByIdAsync(int id, string userId);
        Task<int> SaveHeadAsync(Head entity);
        Task<int> UpdateHeadAsync(Head entity);
        Task<int> DeleteHeadAsync(int id, string userId);
    }
}
