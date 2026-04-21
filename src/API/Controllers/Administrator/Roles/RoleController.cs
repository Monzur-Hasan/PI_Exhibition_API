using API.Controllers.Base;
using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Filter;
using Infrastructure.Features.Service.Administrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers.Administrator.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : BaseApiController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet, Route("GetAllRoles")]
        [ProducesResponseType(type: typeof(IEnumerable<GetApplicationRoleDto>), statusCode: (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllRoles([FromQuery] ApplicationRole_Filter filter)
        {
            try
            {
                var data_list = await _roleService.GetAllRolesAsync(filter);

                if (data_list == null || data_list.ListOfObject == null)
                {
                    return CustomResult("No data found.", HttpStatusCode.NotFound);
                }

                return CustomResult("Data loaded successfully.", new
                {
                    List = data_list.ListOfObject,
                    Pagination = new
                    {
                        data_list.Pageparam.PageNumber,
                        data_list.Pageparam.PageSize,
                        data_list.Pageparam.TotalRows,
                        data_list.Pageparam.TotalPages
                    }
                }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred while fetching agency info.");
                return CustomResult("Data loading failed!", HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequestDto createRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _roleService.CreateRoleAsync(createRoleDto);
                if (result.Success)
                {
                    return CustomResult(result.Message, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("GetActivePermissions")]
        public async Task<IActionResult> GetActivePermissionsAsync([FromQuery] ActivePermissions_Filter filter)
        {
            try
            {
                var data = await _roleService.GetActivePermissionsAsync(filter);

                return CustomResult("Data loaded successfully.", data, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return CustomResult($"Error: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleRequestDto updateRoleDto)
        {
            try
            {
                var result = await _roleService.UpdateRoleAsync(updateRoleDto);
                if (result.Success)
                {
                    return CustomResult(result.Message, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }

        }

        [HttpPost("DeleteRole")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody] DeleteRoleRequestDto deleteRoleRequestDto)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(deleteRoleRequestDto);
                if (result.Success)
                {
                    return CustomResult(result.Message, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("RolePermissions")]
        public async Task<IActionResult> RolePermissions([FromQuery] string[] roleId)
        {
            try
            {
                var response = await _roleService.GetRolePermissionsAsync(roleId);
                return CustomResult(response.Message, response, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return CustomResult(ex.Message, HttpStatusCode.BadRequest);
            }
        }

    }
}
