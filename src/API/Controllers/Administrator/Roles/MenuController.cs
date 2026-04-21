using System.Net;
using API.Controllers.Base;
using Domain.Models.Access.DTO;
using Infrastructure.Features.Service.Administrator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Administrator.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : BaseApiController
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        //Main Menu controllers

        [HttpPost("AddMainMenu")]
        public async Task<IActionResult> AddMainMenu([FromBody] MainMenuInputDto menu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.SaveMainMenuAsync(menu);
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Msg, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet, Route("GetMainMenus")]
        public async Task<IActionResult> GetMainMenuAsync([FromQuery] MainMenuFilter filter)
        {
            try
            {
                var result = await _menuService.GetMainMenus(filter);
                return CustomResult("Data loaded successfully.", result, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred while fetching agency info.");
                return CustomResult("Data loading failed!", HttpStatusCode.BadRequest);
            }
        }
        [HttpPost("UpdateMainMenu")]
        public async Task<IActionResult> UpdateMainMenu([FromBody] UpdateMainMenuDto menu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.UpdateMainMenuAsync(menu);
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Msg, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost("DeleteMainMenu")]
        public async Task<IActionResult> DeleteMainMenu(string MMId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.DeleteMenuAsync(MMId, "tblMainMenus");
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }
                if (result.Code == "404")
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.NotFound);
                }

                return CustomResult(result.Msg, result, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        //Sub Menu controllers

        [HttpPost("AddSubMenu")]
        public async Task<IActionResult> AddSubMenu([FromBody] SubmenuDto submenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.SaveSubMenuAsync(submenu);
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }

                return CustomResult(result.Msg, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }


        }

        [HttpGet, Route("GetSubMenus")]
        public async Task<IActionResult> GetSubMenuAsync([FromQuery] SubMenuFilter filter)
        {
            try
            {
                var result = await _menuService.GetSubMenus(filter);
                return CustomResult("Data loaded successfully.", result, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred while fetching agency info.");
                return CustomResult("Data loading failed!", HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("UpdateSubMenu")]
        public async Task<IActionResult> UpdateSubMenu([FromBody] UpdateSubMenu submenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.UpdateSubMenuAsync(submenu);
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }
                if (result.Code == "404")
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.NotFound);
                }

                return CustomResult(result.Msg, result, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost("DeleteSubMenu")]
        public async Task<IActionResult> DeleteSubMenu(string SubmenuId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CustomResult("Invalid input", HttpStatusCode.UnprocessableEntity);
                }

                var result = await _menuService.DeleteMenuAsync(SubmenuId, "tblSubMenus");
                if (result.Status)
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.OK);
                }
                if (result.Code == "404")
                {
                    return CustomResult(result.Msg, result, HttpStatusCode.NotFound);
                }

                return CustomResult(result.Msg, result, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in SaveAgencyInfo");
                return CustomResult("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        //Shared Menu controllers
        [HttpGet, Route("GetMenuHierarchy")]
        public async Task<IActionResult> GetMenuHierarchy()
        {
            try
            {
                var result = _menuService.GetMenuHierarchy();
                return CustomResult("Data loaded successfully.", result, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred while fetching agency info.");
                return CustomResult("Data loading failed!", HttpStatusCode.BadRequest);
            }
        }
    }
}
