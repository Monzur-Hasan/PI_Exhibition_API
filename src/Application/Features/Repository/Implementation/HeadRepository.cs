using Dapper;
using Domain.Models.Head_Model.DomainModel;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;
using Domain.Shared.Helpers;
using Infrastructure.Features.Repository.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Application.Features.Repository.Implementation
{
    public class HeadRepository : IHeadRepository
    {
        private readonly IDbConnection _db;
        private readonly ILogger<HeadRepository> _logger;

        public HeadRepository(IDbConnection db, ILogger<HeadRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResponse<Head>> GetHeadAsync(Pagination_Filter filter)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Flag", Data.Read);
                param.Add("@UserID", filter.UserID);
                param.Add("@PageNumber", filter.PageNumber);
                param.Add("@PageSize", filter.PageSize);
                using var multi = await _db.QueryMultipleAsync(
                   "sp_Head",
                   param,
                   commandType: CommandType.StoredProcedure
               );

                var totalCount = await multi.ReadSingleAsync<int>();
                var data = (await multi.ReadAsync<Head>()).ToList();

                return new PagedResponse<Head>
                {
                    ListOfObject = data,
                    TotalRows = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };               
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
                var param = new DynamicParameters();
                param.Add("@Flag", Data.ReadById);
                param.Add("@ID", id);
                param.Add("@UserID", userId);

                return await _db.QueryFirstOrDefaultAsync<Head>("sp_Head", param, commandType: CommandType.StoredProcedure);
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
                var param = new DynamicParameters();
                param.Add("@Flag", Data.Insert);
                param.Add("@HeadTypeID", entity.HeadTypeID);
                param.Add("@HeadName", entity.HeadName);
                param.Add("@Description", entity.Description);
                param.Add("@IsActive", entity.IsActive);   
                param.Add("@CreatedBy", entity.UserID);
                param.Add("@CreatedDate", entity.CreatedDate);
                param.Add("@UserID", entity.UserID);

                return await _db.ExecuteScalarAsync<int>("sp_Head", param, commandType: CommandType.StoredProcedure);
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
                var param = new DynamicParameters();
                param.Add("@Flag", Data.Update);
                param.Add("@ID", entity.ID);
                param.Add("@HeadTypeID", entity.HeadTypeID);
                param.Add("@HeadName", entity.HeadName);
                param.Add("@Description", entity.Description);
                param.Add("@IsActive", entity.IsActive);
                param.Add("@UserID", entity.UserID);
                param.Add("@UpdatedBy", entity.UserID);
                param.Add("@UpdatedDate", entity.UpdatedDate);

                return await _db.ExecuteScalarAsync<int>("sp_Head", param, commandType: CommandType.StoredProcedure);
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
                var param = new DynamicParameters();
                param.Add("@Flag", Data.Delete);
                param.Add("@ID", id);               
                param.Add("@UserID", userId);    

                return await _db.ExecuteScalarAsync<int>("sp_Head", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteHeadAsync");
                throw;
            }
        }
    }
}
