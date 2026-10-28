using StudentCourseManagement.Models;
using StudentCourseManagement.Repository;

namespace StudentCourseManagement.Services;

public class EnrollmentService
{
    private readonly EnrollmentRepository _repository;

    public EnrollmentService(EnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        if (enrollment == null)
            throw new ArgumentNullException(nameof(enrollment));

        if (enrollment.StudentId <= 0)
            throw new ArgumentException(
                "Student ID must be greater than zero.");

        if (enrollment.CourseId <= 0)
            throw new ArgumentException(
                "Course ID must be greater than zero.");

        enrollment.EnrollmentDate = DateTime.Today;
        enrollment.Status = "Active";

        await _repository.AddEnrollmentAsync(enrollment);
    }

    public Task<List<Enrollment>> GetAllEnrollmentsAsync()
    {
        return _repository.GetAllEnrollmentsAsync();
    }

    public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(
        int studentId)
    {
        if (studentId <= 0)
            throw new ArgumentException(
                "Student ID must be greater than zero.");

        return await _repository.GetEnrollmentsByStudentIdAsync(studentId);
    }

    public async Task CancelEnrollmentAsync(int enrollmentId)
    {
        if (enrollmentId <= 0)
            throw new ArgumentException(
                "Enrollment ID must be greater than zero.");

        bool cancelled =
            await _repository.CancelEnrollmentAsync(enrollmentId);

        if (!cancelled)
            throw new InvalidOperationException(
                "Enrollment not found or it is already cancelled.");
    }
}
