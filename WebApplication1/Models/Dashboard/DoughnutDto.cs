namespace WebApplication1.Models.Dashboard
{
    public class DoughnutDto
    {
        public List<DoughnutSliceDto> Slices { get; set; } = new();

        public DoughnutDto() { }
        public DoughnutDto(List<DoughnutSliceDto> slices) => Slices = slices;

        // Optional helper
        public decimal Total => Slices?.Sum(s => s.Value) ?? 0m;
    }
}
