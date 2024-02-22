namespace GoogleAPI.Domain.Models.ViewModels
{
    public class Log_VM
    {
        public int Id { get; set; }
        public string? MessageHeader { get; set; }
        public string? Level { get; set; }
        public string? Request { get; set; }// RequestPath
        public string? RequestPath { get; set; }// RequestPath

        public string? ExceptionText { get; set; }
        public string? LogEvent { get; set; }
        public string? UserName { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
