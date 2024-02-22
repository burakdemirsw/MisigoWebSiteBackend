namespace GoogleAPI.Domain.Entities.Common
{
    public class Log : BaseEntity
    {
        public string? Request { get; set; }
        public string? MessageHeader { get; set; }
        public string? Level { get; set; } //RequestPath
        public string? RequestPath { get; set; }

        public string? ExceptionText { get; set; }
        public string? LogEvent { get; set; }
        public string? UserName { get; set; }

    }
}
