namespace Oposiciones.Domain.Entities
{
    public class SyllabusTopic
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public int TopicNumber { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}