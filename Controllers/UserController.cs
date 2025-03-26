using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using UserManagementApi.Models;
using UserManagementApi.Services; // Include the TokenService namespace

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Mock in-memory database
        private static List<User> users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" }
        };

        private readonly TokenService _tokenService;

        public UsersController()
        {
            _tokenService = new TokenService(); // Ideally, use Dependency Injection
        }

        // Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Mock user validation (replace with actual validation logic, e.g., database lookup)
            var user = users.FirstOrDefault(u => u.Email == loginRequest.Email);
            if (user != null && loginRequest.Password == "password") // Use proper password validation
            {
                var token = _tokenService.GenerateToken(user.Id.ToString(), user.Name);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Invalid email or password." });
        }

        // Retrieve all users with pagination
        [HttpGet]
        public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page and pageSize must be greater than zero." });
                }

                // Optimize query with pagination
                var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Return paginated results with metadata
                var result = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = users.Count,
                    Data = pagedUsers
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving users.", Details = ex.Message });
            }
        }

        // Retrieve a user by ID
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Details = ex.Message });
            }
        }

        // Create a new user
        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Enforce unique email constraint
                if (users.Any(u => u.Email == user.Email))
                {
                    return BadRequest(new { Message = "A user with this email address already exists." });
                }

                user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                users.Add(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the user.", Details = ex.Message });
            }
        }

        // Update an existing user
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                // Update user fields
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user.", Details = ex.Message });
            }
        }

        // Delete a user by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                users.Remove(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the user.", Details = ex.Message });
            }
        }
    }

    // Model for LoginRequest
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}