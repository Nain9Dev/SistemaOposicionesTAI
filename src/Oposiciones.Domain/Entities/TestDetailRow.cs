namespace Oposiciones.Domain.Entities
{
    public class TestDetailRow
    {
        public long TestId { get; set; }
        public string Title { get; set; } = string.Empty;

        public long QuestionId { get; set; }
        public string Statement { get; set; } = string.Empty;

        public long OptionId { get; set; }
        public byte SortOrder { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }
}