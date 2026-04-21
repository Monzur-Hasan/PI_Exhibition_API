//using API.Controllers.Base;
//using Domain.Models.Head_Model.DomainModel;
//using Domain.OtherModels.Pagination;
//using Infrastructure.Features.Service.Interface;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.Design;
//using System.Net;

//namespace API.Controllers.Setup
//{
//    [Authorize]
//    [ApiController, Route("api/[controller]")]    
//    public class HeadController : BaseApiController
//    {
//        private readonly IHeadService _service;

//        public HeadController(IHeadService service)
//        {
//            _service = service;
//        }

//        [HttpGet, Route("GetHead")]
//        public async Task<IActionResult> GetHeadAsync([FromQuery] Pagination_Filter filter)
//        {
//            try
//            {
//                var result = await _service.GetHeadAsync(filter);
//                return CustomResult("Head list retrieved successfully", result, HttpStatusCode.OK);
//            }
//            catch (Exception ex)
//            {
//                return CustomResult("Error retrieving heads", null, HttpStatusCode.InternalServerError);
//            }
//        }

//        [HttpGet("GetHeadById")]
//        public async Task<IActionResult> GetHeadByIdAsync(int id, string userId)
//        {
//            try
//            {
//                var result = await _service.GetHeadByIdAsync(id, userId);
//                if (result == null)
//                    return CustomResult("Head not found", HttpStatusCode.NotFound);

//                return CustomResult("Head retrieved successfully", result, HttpStatusCode.OK);
//            }
//            catch (Exception ex)
//            {
//                return CustomResult("Error retrieving head", null, HttpStatusCode.InternalServerError);
//            }
//        }

//        [HttpPost("SaveHead")]
//        public async Task<IActionResult> SaveHeadAsync([FromBody] Head model)
//        {
//            try
//            {
//                var id = await _service.SaveHeadAsync(model);
//                if (id == -1)
//                    return CustomResult("Head name already exists", HttpStatusCode.Conflict);

//                return CustomResult("Head saved successfully", new { Id = id }, HttpStatusCode.Created);
//            }
//            catch (Exception ex)
//            {
//                return CustomResult("Error saving head", null, HttpStatusCode.InternalServerError);
//            }
//        }

//        [HttpPut("UpdateHead")]
//        public async Task<IActionResult> UpdateHeadAsync(int id, string userId, [FromBody] Head model)
//        {
//            try
//            {
//                model.ID = id;
//                model.UserID = userId;
//                var result = await _service.UpdateHeadAsync(model);

//                if (result == -1)
//                    return CustomResult("Head name already exists", HttpStatusCode.Conflict);
//                if (result == 0)
//                    return CustomResult("Head not found", HttpStatusCode.NotFound);

//                return CustomResult("Head updated successfully", new { Id = result }, HttpStatusCode.OK);
//            }
//            catch (Exception ex)
//            {
//                return CustomResult("Error updating head", null, HttpStatusCode.InternalServerError);
//            }
//        }

//        [HttpDelete("DeleteHead")]
//        public async Task<IActionResult> DeleteHeadAsync(int id, string userId)
//        {
//            try
//            {
//                var result = await _service.DeleteHeadAsync(id, userId);
//                if (result == 0)
//                    return CustomResult("Head not found", HttpStatusCode.NotFound);

//                return CustomResult("Head deleted successfully", null, HttpStatusCode.OK);
//            }
//            catch (Exception ex)
//            {
//                return CustomResult("Error deleting head", null, HttpStatusCode.InternalServerError);
//            }
//        }
//    }
//}
