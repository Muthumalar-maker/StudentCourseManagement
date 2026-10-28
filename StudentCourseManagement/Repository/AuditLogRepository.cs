using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class AuditLogRepository
{
    private readonly string _connectionString =
        @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task AddAuditLogAsync(AuditLog auditLog)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            INSERT INTO AuditLogs
                (EntityName, EntityId, Action, Details, CreatedAt, ActionDate)
            VALUES
                (@EntityName, @EntityId, @Action, @Details, @CreatedAt, @ActionDate);";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@EntityName", auditLog.EntityName);
        command.Parameters.AddWithValue("@EntityId", auditLog.EntityId);
        command.Parameters.AddWithValue("@Action", auditLog.Action);

        command.Parameters.AddWithValue(
            "@Details",
            (object?)auditLog.Details ?? DBNull.Value);

        command.Parameters.AddWithValue("@CreatedAt", auditLog.CreatedAt);
        command.Parameters.AddWithValue("@ActionDate", auditLog.ActionDate);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<AuditLog>> GetAllAuditLogsAsync()
    {
        List<AuditLog> logs = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            SELECT AuditId, EntityName, EntityId, Action,
                   Details, CreatedAt, ActionDate
            FROM AuditLogs
            ORDER BY AuditId DESC;";

        using SqlCommand command = new SqlCommand(query, connection);

        await connection.OpenAsync();

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            logs.Add(new AuditLog
            {
                AuditId = reader.GetInt64(
                    reader.GetOrdinal("AuditId")),

                EntityName = reader.GetString(
                    reader.GetOrdinal("EntityName")),

                EntityId = reader.GetInt32(
                    reader.GetOrdinal("EntityId")),

                Action = reader.GetString(
                    reader.GetOrdinal("Action")),

                Details = reader.IsDBNull(
                    reader.GetOrdinal("Details"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Details")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                ActionDate = reader.GetDateTime(
                    reader.GetOrdinal("ActionDate"))
            });
        }

        return logs;
    }
}
