
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;

namespace StudentCourseManagement.Services
{
    public class StudentService
    {
        private readonly StudentRepository _repository;

        public StudentService(StudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> RegisterAsync(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
                throw new ArgumentException("Student name is required.");

            if (string.IsNullOrWhiteSpace(student.Email))
                throw new ArgumentException("Student email is required.");

            if (student.Age < 17 || student.Age > 60)
                throw new ArgumentException("Age must be between 17 and 60.");

            if (student.DepartmentId <= 0)
                throw new ArgumentException("Please select a valid department.");

            return await _repository.AddAsync(student);
        }

        public Task<List<Student>> GetAllStudentsAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Student?> GetStudentByIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("Invalid student ID.");

            return _repository.GetByIdAsync(studentId);
        }
    }
}
