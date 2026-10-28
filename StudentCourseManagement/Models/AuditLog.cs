namespace StudentCourseManagement.Models;

public class AuditLog
{
    public long AuditId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime ActionDate { get; set; } = DateTime.Now;
}
