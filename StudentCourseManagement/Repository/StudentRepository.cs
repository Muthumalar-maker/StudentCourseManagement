
using Microsoft.Data.SqlClient;
using System.Data;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repositories
{
    public class StudentRepository
    {
        public async Task<int> AddAsync(Student student)
        {
            const string sql = """
                INSERT INTO dbo.Students
                    (Name, Email, Age, DepartmentId)
                VALUES
                    (@Name, @Email, @Age, @DepartmentId);

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();

            await using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                .Value = student.Name;

            command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
                .Value = student.Email;

            command.Parameters.Add("@Age", SqlDbType.Int)
                .Value = student.Age;

            command.Parameters.Add("@DepartmentId", SqlDbType.Int)
                .Value = student.DepartmentId;

            await connection.OpenAsync();

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        public async Task<List<Student>> GetAllAsync()
        {
            const string sql = """
                SELECT StudentId, Name, Email, Age,
                       DepartmentId, AdmissionDate, IsActive
                FROM dbo.Students
                ORDER BY StudentId;
                """;

            var students = new List<Student>();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();

            await using SqlCommand command = new SqlCommand(sql, connection);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                students.Add(MapStudent(reader));
            }

            return students;
        }

        public async Task<Student?> GetByIdAsync(int studentId)
        {
            const string sql = """
                SELECT StudentId, Name, Email, Age,
                       DepartmentId, AdmissionDate, IsActive
                FROM dbo.Students
                WHERE StudentId = @StudentId;
                """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();

            await using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@StudentId", SqlDbType.Int)
                .Value = studentId;

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapStudent(reader);
            }

            return null;
        }

        private static Student MapStudent(SqlDataReader reader)
        {
            return new Student
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
