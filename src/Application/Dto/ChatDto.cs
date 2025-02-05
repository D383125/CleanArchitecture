namespace Application.Dto
{
    public record ChatDto
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public required string Message { get; set; }
    }
}
