using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResponse<EmployeeResponse>> SearchEmployeesAsync(EmployeeSearchRequest request);

        Task<EmployeeResponse?> GetByIdAsync(long id);

        Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

        Task<bool> UpdateAsync(long employeeId, UpdateEmployeeRequest request);

        Task<bool> DisableAsync(long employeeId);
    }
}
