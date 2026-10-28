
using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Repository;

public class DepartmentRepository
{
    private readonly string _connectionString =
        @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task AddDepartmentAsync(Department department)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            INSERT INTO Departments (DepartmentName, IsActive)
            VALUES (@DepartmentName, @IsActive);";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(
            "@DepartmentName", department.DepartmentName);

        command.Parameters.AddWithValue(
            "@IsActive", department.IsActive);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        List<Department> departments = new();

        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            SELECT DepartmentId, DepartmentName, IsActive
            FROM Departments
            ORDER BY DepartmentId;";

        using SqlCommand command = new SqlCommand(query, connection);

        await connection.OpenAsync();

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Department department = new()
            {
                DepartmentId = reader.GetInt32(
                    reader.GetOrdinal("DepartmentId")),

                DepartmentName = reader.GetString(
                    reader.GetOrdinal("DepartmentName")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive"))
            };

            departments.Add(department);
        }

        return departments;
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        using SqlConnection connection =
            new SqlConnection(_connectionString);

        string query = @"
            SELECT DepartmentId, DepartmentName, IsActive
            FROM Departments
            WHERE DepartmentId = @DepartmentId;";

        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@DepartmentId", id);

        await connection.OpenAsync();

        using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Department
            {
                DepartmentId = reader.GetInt32(
                    reader.GetOrdinal("DepartmentId")),

                DepartmentName = reader.GetString(
                    reader.GetOrdinal("DepartmentName")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive"))
            };
        }

        return null;
    }
}
