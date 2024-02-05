﻿using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;
using RestSharp;
using ReadEase_C_.Services;

namespace userController.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public UsersController(IConfiguration configuration,UserService service)
        {
            _configuration = configuration;
            _userService = service;
        }

        [HttpGet("users")]
        [Authorize(Roles ="Admin")]
        public IEnumerable<FormModel> getUsers()
        {
           return _userService.GetAllUsers();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(".AspNetCore.Cookies", new CookieOptions
            {
                Path = "/",
                Secure = true, 
                HttpOnly = true,
               SameSite = SameSiteMode.None
            });


            return Ok( "Logout successful" );
        }


        [HttpPost("register")]
        public async Task<IActionResult> PostUser([FromBody] FormModel model)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                // Check if the user with the same username already exists
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                int existingUser = await connection.QueryFirstOrDefaultAsync<int>(checkUserQuery, new { Username = model.Username });

                if (existingUser > 0)
                {
                    // User with the same username already exists, handle accordingly
                    return Conflict("Username already exists");
                }

                // Insert the data into the Users table and retrieve the generated Id using Dapper
                string insertQuery = "INSERT INTO Users (Username, Password, Role) OUTPUT INSERTED.Id VALUES (@Username, @Password, 'User')";
                int userId = await connection.QueryFirstOrDefaultAsync<int>(insertQuery, new { Username = model.Username, Password = model.Password });

                // Set the generated Id in the FormModel
                model.Id = userId;

                // Create claims for the registered user
                var claims = new List<Claim>
                {
            new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
            new Claim(ClaimTypes.Name, model.Username),
            // Add additional claims as needed
                };
                var authProperties = new AuthenticationProperties
                {
                    // Persist the cookie even after the browser is closed
                    IsPersistent = true
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // Sign in the user after registration
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,authProperties);

                return Ok(model);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] FormModel model)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                string selectQuery = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                var user = await connection.QueryFirstOrDefaultAsync<FormModel>(selectQuery, new { Username = model.Username, Password = model.Password });
                
                if (user != null)
                {
                    // Retrieve the role from the database
                        string role = await CheckIfUserIsAdminAsync(user.Id);

                    // Create claims for the authenticated user
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, role ?? "User") // Default to "User" if role is null
                    // Add additional claims as needed
                };

                        var authProperties = new AuthenticationProperties
                        {
                            // Persist the cookie even after the browser is closed
                            IsPersistent = true
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);

                        // Sign in the user
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                        user.Role = role ?? "User";
                        return Ok(user);
                    }
                    else
                    {
                        // Return 401 Unauthorized
                        return Unauthorized();
                    }
                }
            }


        private async Task<string> CheckIfUserIsAdminAsync(int userId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                string checkAdminQuery = "SELECT Role FROM Users WHERE Id = @Id";
                string role = await connection.QueryFirstOrDefaultAsync<string>(checkAdminQuery, new { Id = userId });

                // Check if the role is 'Admin'
                return role ?? string.Empty;
            }
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] FormModel model, int id)
        {

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @Id";
                var userCount = await connection.QueryFirstOrDefaultAsync<int>(checkUserQuery, new { Id = id });
                if (userCount == 0)
                {
                    // User with the given ID does not exist
                    return NotFound();
                }

                string updateQuery = "UPDATE Users SET Username = @Username WHERE Id = @Id";
                await connection.ExecuteAsync(updateQuery, new { Id = id, Username = model.Username });

                return Ok();
            }

        }


        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {

            _userService.DeleteUser(id);
            return Ok();    
        }


    }
}
