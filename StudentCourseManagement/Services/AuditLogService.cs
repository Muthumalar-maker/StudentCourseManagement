using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;

    public AuditLogService(AuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task AddAuditLogAsync(
        string entityName,
        int entityId,
        string action,
        string details)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity name is required.");

        if (entityId <= 0)
            throw new ArgumentException("Entity ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required.");

        AuditLog auditLog = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            Details = details,
            CreatedAt = DateTime.Now,
            ActionDate = DateTime.Now
        };

        await _repository.AddAuditLogAsync(auditLog);
    }

    public Task<List<AuditLog>> GetAllAuditLogsAsync()
    {
        return _repository.GetAllAuditLogsAsync();
    }
}
