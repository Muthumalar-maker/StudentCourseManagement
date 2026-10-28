using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class EnrollmentRepository
{
    private readonly string _connectionString =
        @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            INSERT INTO Enrollments
                (StudentId, CourseId, EnrollmentDate, Status)
            VALUES
                (@StudentId, @CourseId, @EnrollmentDate, @Status);";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@StudentId", enrollment.StudentId);
        command.Parameters.AddWithValue("@CourseId", enrollment.CourseId);
        command.Parameters.AddWithValue("@EnrollmentDate", enrollment.EnrollmentDate);
        command.Parameters.AddWithValue("@Status", enrollment.Status);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
    {
        List<Enrollment> enrollments = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            SELECT EnrollmentId, StudentId, CourseId,
                   EnrollmentDate, Status
            FROM Enrollments
            ORDER BY EnrollmentId;";

        using SqlCommand command = new SqlCommand(query, connection);

        await connection.OpenAsync();

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            enrollments.Add(new Enrollment
            {
                EnrollmentId = reader.GetInt32(
                    reader.GetOrdinal("EnrollmentId")),

                StudentId = reader.GetInt32(
                    reader.GetOrdinal("StudentId")),

                CourseId = reader.GetInt32(
                    reader.GetOrdinal("CourseId")),

                EnrollmentDate = reader.GetDateTime(
                    reader.GetOrdinal("EnrollmentDate")),

                Status = reader.GetString(
                    reader.GetOrdinal("Status"))
            });
        }

        return enrollments;
    }

    public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(
        int studentId)
    {
        List<Enrollment> enrollments = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            SELECT EnrollmentId, StudentId, CourseId,
                   EnrollmentDate, Status
            FROM Enrollments
            WHERE StudentId = @StudentId
            ORDER BY EnrollmentId;";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@StudentId", studentId);

        await connection.OpenAsync();

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            enrollments.Add(new Enrollment
            {
                EnrollmentId = reader.GetInt32(
                    reader.GetOrdinal("EnrollmentId")),

                StudentId = reader.GetInt32(
                    reader.GetOrdinal("StudentId")),

                CourseId = reader.GetInt32(
                    reader.GetOrdinal("CourseId")),

                EnrollmentDate = reader.GetDateTime(
                    reader.GetOrdinal("EnrollmentDate")),

                Status = reader.GetString(
                    reader.GetOrdinal("Status"))
            });
        }

        return enrollments;
    }

    public async Task<bool> CancelEnrollmentAsync(int enrollmentId)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            UPDATE Enrollments
            SET Status = 'Cancelled'
            WHERE EnrollmentId = @EnrollmentId
              AND Status = 'Active';";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@EnrollmentId", enrollmentId);

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}
