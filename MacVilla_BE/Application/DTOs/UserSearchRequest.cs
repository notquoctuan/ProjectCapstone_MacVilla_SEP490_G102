using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserSearchRequest
    {
        // Filter
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; } // Admin, Staff, Customer
        public bool? IsActive { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
