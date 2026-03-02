using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IUserRepository _userRepo;

        public EmployeeService(
            IEmployeeRepository employeeRepo,
            IUserRepository userRepo)
        {
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
        }

        // =====================================================
        // SEARCH + PAGINATION
        // =====================================================
        public async Task<PagedResponse<EmployeeResponse>> SearchEmployeesAsync(EmployeeSearchRequest request)
        {
            var employees = await _employeeRepo.GetAllAsync();

            // Filter Name
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                employees = employees
                    .Where(e => e.User!.FullName.Contains(request.Name))
                    .ToList();
            }

            // Filter Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                employees = employees
                    .Where(e => e.User!.Status == request.Status)
                    .ToList();
            }

            var totalCount = employees.Count;

            var pagedData = employees
                .OrderByDescending(e => e.EmployeeId)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EmployeeResponse
                {
                    EmployeeId = e.EmployeeId,
                    Position = e.Position,
                    UserId = e.UserId,
                    Email = e.User!.Email,
                    FullName = e.User!.FullName,
                    Phone = e.User!.Phone
                })
                .ToList();

            return new PagedResponse<EmployeeResponse>
            {
                Data = pagedData,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        // =====================================================
        // GET BY ID
        // =====================================================
        public async Task<EmployeeResponse?> GetByIdAsync(long id)
        {
            var employee = await _employeeRepo.GetByIdAsync(id);

            if (employee == null)
                return null;

            return new EmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                Position = employee.Position,
                UserId = employee.UserId,
                Email = employee.User!.Email,
                FullName = employee.User!.FullName,
                Phone = employee.User!.Phone
            };
        }

        // =====================================================
        // CREATE EMPLOYEE
        // =====================================================
        public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
        {
            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                Role = "Employee",
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveChangesAsync();

            var credential = new UserCredential
            {
                UserId = user.UserId,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            await _userRepo.AddCredentialAsync(credential);
            await _userRepo.SaveChangesAsync();

            var employee = new Employee
            {
                UserId = user.UserId,
                Position = request.Position
            };

            await _employeeRepo.AddAsync(employee);
            await _employeeRepo.SaveChangesAsync();

            return new EmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                Position = employee.Position,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone
            };
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // =====================================================
        // UPDATE EMPLOYEE
        // =====================================================
        public async Task<bool> UpdateAsync(long employeeId, UpdateEmployeeRequest request)
        {
            var employee = await _employeeRepo.GetByIdAsync(employeeId);

            if (employee == null)
                return false;

            employee.Position = request.Position;
            employee.User!.FullName = request.FullName;
            employee.User.Email = request.Email;
            employee.User.Phone = request.Phone;

            await _employeeRepo.UpdateAsync(employee);

            return true;
        }

        // =====================================================
        // DISABLE EMPLOYEE
        // =====================================================
        public async Task<bool> DisableAsync(long employeeId)
        {
            return await _employeeRepo.SetIsActiveAsync(employeeId, false);
        }
    }
}
