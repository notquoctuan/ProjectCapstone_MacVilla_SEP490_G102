using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        // =====================================================
        // GET: api/admin/employees
        // =====================================================
        [HttpGet]
        public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetAll(
            [FromQuery] string? name,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool getAll = false)
        {
            var request = new EmployeeSearchRequest
            {
                Name = name,
                Status = status,
                PageNumber = pageNumber > 0 ? pageNumber : 1,
                PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10,
                GetAll = getAll
            };

            var result = await _service.SearchEmployeesAsync(request);
            return Ok(result);
        }
        //[HttpGet]
        //public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetAll(
        //    [FromQuery] string? name,
        //    [FromQuery] string? status,
        //    [FromQuery] int pageNumber = 1,
        //    [FromQuery] int pageSize = 10)
        //{
        //    var request = new EmployeeSearchRequest
        //    {
        //        Name = name,
        //        Status = status,
        //        PageNumber = pageNumber > 0 ? pageNumber : 1,
        //        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10
        //    };

        //    var result = await _service.SearchEmployeesAsync(request);
        //    return Ok(result);
        //}

        // =====================================================
        // GET: api/admin/employees/{id}
        // =====================================================
        [HttpGet("{id:long}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(long id)
        {
            var employee = await _service.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // =====================================================
        // POST: api/admin/employees
        // =====================================================
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> Create(
            [FromBody] CreateEmployeeRequest request)
        {
            var created = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.EmployeeId },
                created);
        }

        // =====================================================
        // PUT: api/employees/{id}
        // =====================================================
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(
            long id,
            [FromBody] UpdateEmployeeRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
                return NotFound();

            return NoContent(); // 204 chuẩn REST khi update thành công
        }

        // =====================================================
        // PATCH: api/employees/{id}/disable
        // =====================================================
        [HttpPatch("{id:long}/disable")]
        public async Task<IActionResult> Disable(long id)
        {
            var disabled = await _service.DisableAsync(id);

            if (!disabled)
                return NotFound();

            return NoContent(); // 204 chuẩn REST
        }
    }
}
