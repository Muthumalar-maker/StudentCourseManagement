
using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService(DepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task AddDepartmentAsync(Department department)
    {
        if (department == null)
            throw new ArgumentNullException(nameof(department));

        if (string.IsNullOrWhiteSpace(department.DepartmentName))
            throw new ArgumentException("Department name is required.");

        await _repository.AddDepartmentAsync(department);
    }

    public Task<List<Department>> GetAllDepartmentsAsync()
    {
        return _repository.GetAllDepartmentsAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Department ID must be greater than zero.");

        return await _repository.GetDepartmentByIdAsync(id);
    }
}
