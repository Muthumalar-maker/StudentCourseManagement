namespace StudentCourseManagement.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.Today;

    public string Status { get; set; } = "Active";
}
