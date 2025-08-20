namespace WebApplication1.Models.Dashboard
{
    public class SeriesDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Data { get; set; } = new();

        public SeriesDto() { }
        public SeriesDto(List<string> labels, List<decimal> data)
        {
            Labels = labels;
            Data = data;
        }
    }
}
