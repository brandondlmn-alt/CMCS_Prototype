CMCS Prototype - Contract Monthly Claim SystemOverviewThe Contract Monthly Claim System (CMCS) is a web-based application designed for independent contractor lecturers to submit monthly claims for hours worked. It allows lecturers to upload supporting documents, coordinators to review claims, and academic managers to approve or reject them. The system tracks claim statuses transparently and ensures data integrity through a relational database. This prototype was developed as part of PROG6212 POE Part 2, building on a non-functional UI from Part 1.The application uses ASP.NET Core MVC for the backend, Entity Framework Core with SQLite for data persistence, Bootstrap for responsive UI, and ASP.NET Core Identity for authentication and role-based access control.FeaturesUser Authentication: Role-based login for Lecturers, Coordinators, and Managers.
Claim Submission: Lecturers can submit claims with details (month, year, hours worked) and upload supporting documents (PDF, DOCX, XLSX; max 5MB per file).
Automatic Calculation: Total claim amount is calculated as HoursWorked × HourlyRate.
Claim Review and Approval: Coordinators review claims, add notes, and mark as reviewed. Managers approve or reject with notes.
Status Tracking: Claims progress through statuses (Pending, Reviewed, Approved, Rejected) with visual progress bars on dashboards.
Document Management: Uploaded documents are stored securely and viewable in modals or links during review.
Dashboards: Role-specific views showing claim summaries, pending reviews, and reports.
Error Handling: Graceful error management with user-friendly messages.
Unit Testing: Basic unit tests for key functionalities like claim submission and approval.

Technologies UsedFramework: ASP.NET Core MVC (.NET 8.0)
Database: Entity Framework Core with SQLite (for development; can be swapped to SQL Server)
Authentication: ASP.NET Core Identity
Frontend: Bootstrap 5.3 (via CDN), Razor Views
File Handling: IFormFile for uploads, stored in wwwroot/uploads
Testing: xUnit with Moq and InMemory database
Version Control: Git (with at least 5 commits as per requirements)

Prerequisites.NET SDK 8.0 or later
Visual Studio 2022 (or VS Code with .NET CLI)
SQLite (handled via EF Core)

Setup InstructionsClone the Repository:

git clone https://github.com/your-username/CMCS_Prototype.git
cd CMCS_Prototype

Restore Dependencies:

dotnet restore

Database Setup:Ensure appsettings.json has the connection string: "DefaultConnection": "DataSource=app.db;Cache=Shared".
Run migrations:

dotnet ef migrations add InitialCreate
dotnet ef database update

Seed Data (Optional):Manually add users via the app's registration page or implement a seeder in Program.cs using UserManager and RoleManager to create roles ("Lecturer", "Coordinator", "Manager") and sample users.
Example users:Lecturer: Email lecturer@example.com, Password Password123!, Role "Lecturer"
Coordinator: Email coord@example.com, Password Password123!, Role "Coordinator"
Manager: Email manager@example.com, Password Password123!, Role "Manager"

Add sample lecturers, coordinators, managers to the database via code or tools like SQLite Browser.

Run the Application:

dotnet run

Access at https://localhost:5001 (or the port shown).
Register/login to test features.

UsageLecturer Flow:Login as a lecturer.
Navigate to "Submit Claim" via the navbar.
Fill in form details and upload documents.
View status on Dashboard.

Coordinator/Manager Flow:Login with appropriate role.
Go to "Review Claims".
View pending claims, check documents, add notes, and approve/reject.

Tracking:Status updates in real-time on save (no live refresh; reload page).

TestingUnit Tests: Located in a separate project CMCS_Prototype.Tests.Run tests:

cd ../CMCS_Prototype.Tests
dotnet test

Covers claim calculation, status updates, etc.

Manual Testing: Use sample data to submit/review claims. Check for errors in file uploads (e.g., invalid types/sizes).

Assumptions and Constraints (from Part 1)Assumes predefined roles and automatic calculations.
Constraints: Non-functional in Part 1; now functional but limited to key operations. No real-time notifications or advanced security features.

Version ControlRepository: [GitHub Link] (provide your repo URL).
Commit History: At least 5 commits, e.g.:Added models and DbContext.
Implemented claim submission with file uploads.
Added review and approval logic.
Integrated dashboards and status tracking.
Added unit tests and error handling.

Lecturer Feedback ImplementationAs per Part 2 requirements, feedback from Part 1 (e.g., improve validation, enhance interactivity) was addressed:Added client-side validation in forms.
Used Bootstrap modals for document previews.
Restricted file uploads for security.

See Feedback.docx for details (included in repo).ReferencesASP.NET Core MVC: Microsoft Docs
Bootstrap: Bootstrap Docs
Entity Framework Core: EF Core Docs

