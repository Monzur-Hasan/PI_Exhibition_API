using Application.Features.Repository.Implementation;
using Domain.Models.Administrator.DTO;
using Infrastructure.Features.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Setup
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IConfiguration _configuration;
        public ProjectController(IProjectRepository projectRepository, IConfiguration configuration)
        {
            _projectRepository = projectRepository;
            _configuration = configuration;
        }

        [HttpGet, Route("GetProjects")]
        public async Task<IActionResult> GetProjectsAsync()
            => Ok(await _projectRepository.GetProjectsAsync());

        [HttpGet, Route("GetProjectById{id}")]
        public async Task<IActionResult> GetProjectByIdAsync(long id)
            => Ok(await _projectRepository.GetProjectByIdAsync(id));

        [HttpPut, Route("UpdateProject{id}")]
        public async Task<IActionResult> UpdateProjectAsync(long id, [FromForm] UpdateProjectRequestDto request)
        {
            await _projectRepository.UpdateProjectAsync(id, request);
            return Ok("Updated Successfully");
        }

        [HttpDelete, Route("DeleteProject{id}")]
        public async Task<IActionResult> DeleteProjectAsync(long id)
        {
            await _projectRepository.DeleteProjectAsync(id);
            return Ok("Deleted Successfully");
        }

        [HttpGet("file")]
        public IActionResult GetFile(string fileName)
        {
            var basePath = _configuration["FileUploadSettings:BasePath"];

            var path = Path.Combine(basePath, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetContentType(path);

            var bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, contentType);
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

    }
}
