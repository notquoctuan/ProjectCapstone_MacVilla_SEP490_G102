using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RevenueChartData
    {
        public string Date { get; set; } = null!; // Định dạng "dd/MM"
        public decimal Value { get; set; }
    }
}
