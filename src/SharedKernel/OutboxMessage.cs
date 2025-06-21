namespace SharedKernel
{
    public sealed class OutboxMessage
    {
        public OutboxMessage(string type, string content, DateTime occurredOnUtc)
        {
            Id = Guid.NewGuid();
            Type = type;
            Content = content;
            OccurredOnUtc = occurredOnUtc;
        }

        public Guid Id { get; init; }
        public string Type { get; init; }
        public string Content { get; init; }
        public DateTime OccurredOnUtc { get; init; }
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }

        private OutboxMessage() { }
    }
}