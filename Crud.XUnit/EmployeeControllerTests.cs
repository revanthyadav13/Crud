using Crud.Controllers;
using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crud.XUnit
{
    public class EmployeeControllerTests
    {
        private readonly EmployeeController _controller;
        private readonly AppDbContext _context;

        public EmployeeControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                               .UseInMemoryDatabase("TestDatabase")
                               .Options;

            // Create a new instance of AppDbContext using the in-memory database
            _context = new AppDbContext(options);

            // Create the controller instance with the in-memory context
            _controller = new EmployeeController(_context);
        }

        [Fact]
        public async Task CreateEmployee()
        {
            // Arrange
            var newEmployee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1985, 5, 1),
                Position = "Software Developer"
            };

            // Act
            var result = await _controller.CreateEmployee(newEmployee);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Employee>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(201, createdAtActionResult.StatusCode);

            var createdEmployee = Assert.IsType<Employee>(createdAtActionResult.Value);
            Assert.Equal(newEmployee.FirstName, createdEmployee.FirstName);
            Assert.Equal(newEmployee.LastName, createdEmployee.LastName);
            Assert.Equal(newEmployee.Email, createdEmployee.Email);
            Assert.Equal(newEmployee.Position, createdEmployee.Position);
        }

        [Fact]
        public async Task GetEmployees()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1985, 5, 1),
                Position = "Software Developer"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetEmployees();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Employee>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Unwrap ActionResult to get the result

            // Assert that the value is of the expected type
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);

            // Convert to a list to make assertions
            var employeeList = employees.ToList();
            Assert.Equal(1, employeeList.Count);


        }

        [Fact]
        public async Task DeleteEmployee()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1985, 5, 1),
                Position = "Software Developer"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteEmployee(employee.Id);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, actionResult.StatusCode); // No Content status code
        }

        [Fact]
        public async Task UpdateEmployee()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1985, 5, 1),
                Position = "Software Developer"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Update the employee details
            employee.FirstName = "Jane";
            employee.LastName = "Smith";

            // Act
            var result = await _controller.UpdateEmployee(employee.Id, employee);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, actionResult.StatusCode); // No Content status code

            var updatedEmployee = await _context.Employees.FindAsync(employee.Id);
            Assert.NotNull(updatedEmployee);
            Assert.Equal("Jane", updatedEmployee.FirstName);
            Assert.Equal("Smith", updatedEmployee.LastName);
            Assert.Equal("john.doe@example.com", updatedEmployee.Email); // Check if email remains the same
            Assert.Equal("Software Developer", updatedEmployee.Position);
        }

    }
}