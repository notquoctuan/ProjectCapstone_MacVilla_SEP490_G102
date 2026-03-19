public class DashboardVM
{
    public int TotalActiveProducts { get; set; }
    public int NewOrdersToday { get; set; }
    public int TotalCustomers { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<RevenueChartDataVM> RevenueChart { get; set; } = new();
}
public class RevenueChartDataVM
{
    public string Date { get; set; }
    public decimal Value { get; set; }
}