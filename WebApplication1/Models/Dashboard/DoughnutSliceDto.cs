namespace WebApplication1.Models.Dashboard
{
    public class DoughnutSliceDto
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }

        public DoughnutSliceDto() { }
        public DoughnutSliceDto(string label, decimal value)
        {
            Label = label;
            Value = value;
        }
    }
}
