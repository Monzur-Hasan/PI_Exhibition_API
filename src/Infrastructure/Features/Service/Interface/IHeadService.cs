using Domain.Models.Head_Model.DomainModel;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Features.Service.Interface
{
    public interface IHeadService
    {
        Task<PagedResponse<Head>> GetHeadAsync(Pagination_Filter filter);
        Task<Head?> GetHeadByIdAsync(int id, string userId);
        Task<int> SaveHeadAsync(Head entity);
        Task<int> UpdateHeadAsync(Head entity);
        Task<int> DeleteHeadAsync(int id, string userId);
    }
}
