namespace WebApplication1.Models.Dashboard
{
    public class DashboardCardsDto
    {
        public decimal TodaysRevenue { get; set; }
        public int TodaysOrder { get; set; }

        public decimal AvgRevenue { get; set; }

        public DashboardCardsDto() { }
        public DashboardCardsDto(decimal todaysRevenue, int todaysOrder, decimal avgRevenue)
        {
            TodaysRevenue = todaysRevenue;
            TodaysOrder = todaysOrder;
          
            AvgRevenue = avgRevenue;
        }
    }
}
