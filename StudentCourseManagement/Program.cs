using Microsoft.Data.SqlClient;
using StudentCourseManagement.Models;
using StudentCourseManagement.Repositories;
using StudentCourseManagement.Repository;
using StudentCourseManagement.Services;

string connectionString =
    @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

// Repositories
var studentRepository = new StudentRepository();
var courseRepository = new CourseRepository(connectionString);
var departmentRepository = new DepartmentRepository();
var enrollmentRepository = new EnrollmentRepository();
var auditLogRepository = new AuditLogRepository();

// Services
var studentService = new StudentService(studentRepository);
var courseService = new CourseService(courseRepository);
var departmentService = new DepartmentService(departmentRepository);
var enrollmentService = new EnrollmentService(enrollmentRepository);
var auditLogService = new AuditLogService(auditLogRepository);

while (true)
{
    Console.WriteLine("\n==================================");
    Console.WriteLine(" STUDENT COURSE MANAGEMENT SYSTEM");
    Console.WriteLine("==================================");
    Console.WriteLine("1. Student Management");
    Console.WriteLine("2. Department Management");
    Console.WriteLine("3. Course Management");
    Console.WriteLine("4. Enrollment Management");
    Console.WriteLine("5. Audit Logs");
    Console.WriteLine("6. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            // ================= STUDENT =================
            case "1":
                {
                    Console.WriteLine("\n--- STUDENT MANAGEMENT ---");
                    Console.WriteLine("1. Add Student");
                    Console.WriteLine("2. View All Students");
                    Console.WriteLine("3. Search Student By ID");
                    Console.WriteLine("4. Back to Main Menu");
                    Console.Write("Enter your choice: ");

                    string? studentChoice = Console.ReadLine();

                    switch (studentChoice)
                    {
                        case "1":
                            {
                                Console.Write("Student name: ");
                                string name = Console.ReadLine() ?? "";

                                Console.Write("Email: ");
                                string email = Console.ReadLine() ?? "";

                                Console.Write("Age: ");
                                if (!int.TryParse(Console.ReadLine(), out int age))
                                {
                                    Console.WriteLine("Invalid age.");
                                    break;
                                }

                                Console.WriteLine("\nAvailable Departments:");

                                var availableDepartments =
                                    await departmentService.GetAllDepartmentsAsync();

                                if (availableDepartments.Count == 0)
                                {
                                    Console.WriteLine("No departments found.");
                                    Console.WriteLine("Please add a department first.");
                                    break;
                                }

                                foreach (var department in availableDepartments)
                                {
                                    if (department.IsActive)
                                    {
                                        Console.WriteLine(
                                            $"{department.DepartmentId}. {department.DepartmentName}");
                                    }
                                }

                                Console.Write("Enter Department ID: ");

                                if (!int.TryParse(Console.ReadLine(), out int departmentId)
                                    || departmentId <= 0)
                                {
                                    Console.WriteLine("Invalid department ID.");
                                    break;
                                }

                                var selectedDepartment =
                                    await departmentService.GetDepartmentByIdAsync(
                                        departmentId);

                                if (selectedDepartment == null ||
                                    !selectedDepartment.IsActive)
                                {
                                    Console.WriteLine(
                                        "Invalid or inactive department ID.");
                                    break;
                                }

                                var student = new Student
                                {
                                    Name = name,
                                    Email = email,
                                    Age = age,
                                    DepartmentId = departmentId
                                };

                                int newStudentId =
                                    await studentService.RegisterAsync(student);

                                Console.WriteLine(
                                    $"Student added successfully. ID: {newStudentId}");

                                await auditLogService.AddAuditLogAsync(
                                    "Student",
                                    newStudentId,
                                    "Add Student",
                                    $"Student ID: {newStudentId}, Name: {name}");

                                break;
                            }

                        case "2":
                            {
                                var students =
                                    await studentService.GetAllStudentsAsync();

                                if (students.Count == 0)
                                {
                                    Console.WriteLine("No students found.");
                                    break;
                                }

                                foreach (var s in students)
                                {
                                    Console.WriteLine(
                                        $"{s.StudentId} | {s.Name} | {s.Email} | " +
                                        $"{s.Age} | Department: {s.DepartmentId}");
                                }

                                break;
                            }

                        case "3":
                            {
                                Console.Write("Student ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int searchStudentId))
                                {
                                    Console.WriteLine("Invalid student ID.");
                                    break;
                                }

                                var foundStudent =
                                    await studentService.GetStudentByIdAsync(
                                        searchStudentId);

                                if (foundStudent == null)
                                {
                                    Console.WriteLine("Student not found.");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        $"{foundStudent.StudentId} | " +
                                        $"{foundStudent.Name} | " +
                                        $"{foundStudent.Email} | " +
                                        $"{foundStudent.Age} | " +
                                        $"Department: {foundStudent.DepartmentId}");
                                }

                                break;
                            }

                        case "4":
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    break;
                }

            // ================= DEPARTMENT =================
            case "2":
                {
                    Console.WriteLine("\n--- DEPARTMENT MANAGEMENT ---");
                    Console.WriteLine("1. Add Department");
                    Console.WriteLine("2. View All Departments");
                    Console.WriteLine("3. Search Department By ID");
                    Console.WriteLine("4. Back to Main Menu");
                    Console.Write("Enter your choice: ");

                    string? departmentChoice = Console.ReadLine();

                    switch (departmentChoice)
                    {
                        case "1":
                            {
                                Console.Write("Department name: ");
                                string departmentName = Console.ReadLine() ?? "";

                                var department = new Department
                                {
                                    DepartmentName = departmentName,
                                    IsActive = true
                                };

                                await departmentService.AddDepartmentAsync(department);

                                Console.WriteLine("Department added successfully.");

                                var updatedDepartments =
                                    await departmentService.GetAllDepartmentsAsync();

                                int newDepartmentId = 0;

                                foreach (var item in updatedDepartments)
                                {
                                    if (item.DepartmentName.Equals(
                                        departmentName,
                                        StringComparison.OrdinalIgnoreCase)
                                        && item.DepartmentId > newDepartmentId)
                                    {
                                        newDepartmentId = item.DepartmentId;
                                    }
                                }

                                if (newDepartmentId > 0)
                                {
                                    await auditLogService.AddAuditLogAsync(
                                        "Department",
                                        newDepartmentId,
                                        "Add Department",
                                        $"Department Name: {departmentName}");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        "Department was added, but its audit log could not find the ID.");
                                }

                                break;
                            }

                        case "2":
                            {
                                var departments =
                                    await departmentService.GetAllDepartmentsAsync();

                                if (departments.Count == 0)
                                {
                                    Console.WriteLine("No departments found.");
                                    break;
                                }

                                foreach (var d in departments)
                                {
                                    Console.WriteLine(
                                        $"{d.DepartmentId} | {d.DepartmentName} | " +
                                        $"Active: {d.IsActive}");
                                }

                                break;
                            }

                        case "3":
                            {
                                Console.Write("Department ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int searchDepartmentId))
                                {
                                    Console.WriteLine("Invalid department ID.");
                                    break;
                                }

                                var foundDepartment =
                                    await departmentService.GetDepartmentByIdAsync(
                                        searchDepartmentId);

                                if (foundDepartment == null)
                                {
                                    Console.WriteLine("Department not found.");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        $"{foundDepartment.DepartmentId} | " +
                                        $"{foundDepartment.DepartmentName} | " +
                                        $"Active: {foundDepartment.IsActive}");
                                }

                                break;
                            }

                        case "4":
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    break;
                }

            // ================= COURSE =================
            case "3":
                {
                    Console.WriteLine("\n--- COURSE MANAGEMENT ---");
                    Console.WriteLine("1. Add Course");
                    Console.WriteLine("2. View All Courses");
                    Console.WriteLine("3. Search Course By ID");
                    Console.WriteLine("4. Back to Main Menu");
                    Console.Write("Enter your choice: ");

                    string? courseChoice = Console.ReadLine();

                    switch (courseChoice)
                    {
                        case "1":
                            {
                                Console.Write("Course name: ");
                                string courseName = Console.ReadLine() ?? "";

                                Console.Write("Duration in months: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int duration))
                                {
                                    Console.WriteLine("Invalid duration.");
                                    break;
                                }

                                Console.Write("Fee: ");

                                if (!decimal.TryParse(
                                    Console.ReadLine(), out decimal fee))
                                {
                                    Console.WriteLine("Invalid fee.");
                                    break;
                                }

                                // Part 2 continues from here.

                                var course = new Course
                                {
                                    CourseName = courseName,
                                    DurationMonths = duration,
                                    Fee = fee
                                };

                                await courseService.AddCourseAsync(course);

                                Console.WriteLine("Course added successfully.");

                                var updatedCourses =
                                    await courseService.GetAllCoursesAsync();

                                int newCourseId = 0;

                                foreach (var item in updatedCourses)
                                {
                                    if (item.CourseName.Equals(
                                        courseName,
                                        StringComparison.OrdinalIgnoreCase)
                                        && item.CourseId > newCourseId)
                                    {
                                        newCourseId = item.CourseId;
                                    }
                                }

                                if (newCourseId > 0)
                                {
                                    await auditLogService.AddAuditLogAsync(
                                        "Course",
                                        newCourseId,
                                        "Add Course",
                                        $"Course Name: {courseName}");
                                }

                                break;
                            }

                        case "2":
                            {
                                var courses =
                                    await courseService.GetAllCoursesAsync();

                                if (courses.Count == 0)
                                {
                                    Console.WriteLine("No courses found.");
                                    break;
                                }

                                foreach (var c in courses)
                                {
                                    Console.WriteLine(
                                        $"{c.CourseId} | {c.CourseName} | " +
                                        $"{c.DurationMonths} months | {c.Fee:N2}");
                                }

                                break;
                            }

                        case "3":
                            {
                                Console.Write("Course ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int searchCourseId))
                                {
                                    Console.WriteLine("Invalid course ID.");
                                    break;
                                }

                                var foundCourse =
                                    await courseService.GetCourseByIdAsync(
                                        searchCourseId);

                                if (foundCourse == null)
                                {
                                    Console.WriteLine("Course not found.");
                                }
                                else
                                {
                                    Console.WriteLine(
                                        $"{foundCourse.CourseId} | " +
                                        $"{foundCourse.CourseName} | " +
                                        $"{foundCourse.DurationMonths} months | " +
                                        $"{foundCourse.Fee:N2}");
                                }

                                break;
                            }

                        case "4":
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    break;
                }

            // ============ ENROLLMENT MANAGEMENT ============
            case "4":
                {
                    Console.WriteLine("\n--- ENROLLMENT MANAGEMENT ---");
                    Console.WriteLine("1. Enroll Student");
                    Console.WriteLine("2. View All Enrollments");
                    Console.WriteLine("3. View Enrollments By Student ID");
                    Console.WriteLine("4. Cancel Enrollment");
                    Console.WriteLine("5. Back to Main Menu");
                    Console.Write("Enter your choice: ");

                    string? enrollmentChoice = Console.ReadLine();

                    switch (enrollmentChoice)
                    {
                        case "1":
                            {
                                Console.Write("Student ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int enrollStudentId)
                                    || enrollStudentId <= 0)
                                {
                                    Console.WriteLine("Invalid student ID.");
                                    break;
                                }

                                Console.Write("Course ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int enrollCourseId)
                                    || enrollCourseId <= 0)
                                {
                                    Console.WriteLine("Invalid course ID.");
                                    break;
                                }

                                var enrollment = new Enrollment
                                {
                                    StudentId = enrollStudentId,
                                    CourseId = enrollCourseId,
                                    EnrollmentDate = DateTime.Today,
                                    Status = "Active"
                                };

                                await enrollmentService.AddEnrollmentAsync(enrollment);

                                Console.WriteLine("Student enrolled successfully.");

                                var updatedEnrollments =
                                    await enrollmentService.GetAllEnrollmentsAsync();

                                int newEnrollmentId = 0;

                                foreach (var item in updatedEnrollments)
                                {
                                    if (item.StudentId == enrollStudentId
                                        && item.CourseId == enrollCourseId
                                        && item.EnrollmentId > newEnrollmentId)
                                    {
                                        newEnrollmentId = item.EnrollmentId;
                                    }
                                }

                                if (newEnrollmentId > 0)
                                {
                                    await auditLogService.AddAuditLogAsync(
                                        "Enrollment",
                                        newEnrollmentId,
                                        "Add Enrollment",
                                        $"Student ID: {enrollStudentId}, " +
                                        $"Course ID: {enrollCourseId}");
                                }

                                break;
                            }

                        case "2":
                            {
                                var enrollments =
                                    await enrollmentService.GetAllEnrollmentsAsync();

                                if (enrollments.Count == 0)
                                {
                                    Console.WriteLine("No enrollments found.");
                                    break;
                                }

                                foreach (var e in enrollments)
                                {
                                    Console.WriteLine(
                                        $"{e.EnrollmentId} | Student: {e.StudentId} | " +
                                        $"Course: {e.CourseId} | {e.EnrollmentDate:d} | " +
                                        $"Status: {e.Status}");
                                }

                                break;
                            }

                        case "3":
                            {
                                Console.Write("Student ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int viewStudentId))
                                {
                                    Console.WriteLine("Invalid student ID.");
                                    break;
                                }

                                var studentEnrollments =
                                    await enrollmentService
                                        .GetEnrollmentsByStudentIdAsync(viewStudentId);

                                if (studentEnrollments.Count == 0)
                                {
                                    Console.WriteLine(
                                        "No enrollments found for this student.");
                                    break;
                                }

                                foreach (var e in studentEnrollments)
                                {
                                    Console.WriteLine(
                                        $"{e.EnrollmentId} | Student: {e.StudentId} | " +
                                        $"Course: {e.CourseId} | {e.EnrollmentDate:d} | " +
                                        $"Status: {e.Status}");
                                }

                                break;
                            }

                        case "4":
                            {
                                Console.Write("Enrollment ID to cancel: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int cancelEnrollmentId)
                                    || cancelEnrollmentId <= 0)
                                {
                                    Console.WriteLine("Invalid enrollment ID.");
                                    break;
                                }

                                await enrollmentService.CancelEnrollmentAsync(
                                    cancelEnrollmentId);

                                Console.WriteLine("Enrollment cancelled successfully.");

                                await auditLogService.AddAuditLogAsync(
                                    "Enrollment",
                                    cancelEnrollmentId,
                                    "Cancel Enrollment",
                                    $"Enrollment ID: {cancelEnrollmentId}");

                                break;
                            }

                        case "5":
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    break;
                }

            // ============ AUDIT LOGS ============
            case "5":
                {
                    Console.WriteLine("\n--- AUDIT LOGS ---");
                    Console.WriteLine("1. Add Audit Log");
                    Console.WriteLine("2. View All Audit Logs");
                    Console.WriteLine("3. Back to Main Menu");
                    Console.Write("Enter your choice: ");

                    string? auditChoice = Console.ReadLine();

                    switch (auditChoice)
                    {
                        case "1":
                            {
                                Console.Write("Entity name: ");
                                string entityName = Console.ReadLine() ?? "";

                                Console.Write("Entity ID: ");

                                if (!int.TryParse(
                                    Console.ReadLine(), out int entityId)
                                    || entityId <= 0)
                                {
                                    Console.WriteLine("Invalid entity ID.");
                                    break;
                                }

                                Console.Write("Action: ");
                                string action = Console.ReadLine() ?? "";

                                Console.Write("Details: ");
                                string details = Console.ReadLine() ?? "";

                                await auditLogService.AddAuditLogAsync(
                                    entityName,
                                    entityId,
                                    action,
                                    details);

                                Console.WriteLine("Audit log added successfully.");
                                break;
                            }

                        case "2":
                            {
                                var logs =
                                    await auditLogService.GetAllAuditLogsAsync();

                                if (logs.Count == 0)
                                {
                                    Console.WriteLine("No audit logs found.");
                                    break;
                                }

                                foreach (var log in logs)
                                {
                                    Console.WriteLine(
                                        $"{log.AuditId} | {log.EntityName} | " +
                                        $"Entity ID: {log.EntityId} | {log.Action} | " +
                                        $"{log.Details} | Created: {log.CreatedAt} | " +
                                        $"Action Date: {log.ActionDate}");
                                }

                                break;
                            }

                        case "3":
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    break;
                }

            // ============ EXIT ============
            case "6":
                Console.WriteLine("Application closed.");
                return;

            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Validation error: {ex.Message}");
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}