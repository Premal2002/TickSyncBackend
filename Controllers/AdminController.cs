using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickSyncAPI.HelperClasses;
using TickSyncAPI.Interfaces;
using TickSyncAPI.Services;

namespace TickSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("getAllDataCounts")]
        public async Task<IActionResult> GetAllDataCounts()
        {
            var result = await _adminService.GetAllDataCounts();
            return Ok(result);
        }
        
        [HttpGet("getEntityData/{entity}")]
        public async Task<IActionResult> GetEntityData(string entity)
        {
            var result = await _adminService.GetEntityData(entity);
            return Ok(result);
        }
    }
}
