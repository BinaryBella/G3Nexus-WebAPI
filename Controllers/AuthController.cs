using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using G3NexusBackend.Models;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService,IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginDTO userModel)
    {
        var response = new ApiResponse
        {
            Status = true
        };

        try
        {
            // Validate the incoming request model
            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.Error = "Invalid Data";
                return BadRequest(response);
            }

            // Authenticate the user (this should return a user object if authentication is successful)
            var user = _authService.IsAuthenticated(userModel.EmailAddress, userModel.Password);

            if (user != null)
            {
                
                // Ensure the role from the DTO matches the user's role
                // if (user.Role != userModel.Role)
                {
                    response.Status = false;
                    response.Message = "Unauthorized: Incorrect role";
                    return new JsonResult(response);
                }

                // Define valid job titles for authentication based on the user's role
                string[] validRoles;
                string errorMessage;

                if (user.Role == "Company-Admin" || user.Role == "Employee")
                {
                    validRoles = new[] { "Company-Admin", "Employee" };
                    errorMessage = "Unauthorized: Only Company-Admin or Employee can login";
                }
                else if (user.Role == "Client-Admin" || user.Role == "Client")
                {
                    validRoles = new[] { "Client-Admin", "Client" };
                    errorMessage = "Unauthorized: Only Client-Admin or Client can login";
                }
                else
                {
                    response.Status = false;
                    response.Message = "Unauthorized: Your role does not have access to this endpoint";
                    return new JsonResult(response);
                }

                // Check if the user's role is valid for this endpoint
                if (validRoles.Contains(user.Role))
                {
                    // Generate access token with user's email and roles
                    var accessToken = _jwtService.GenerateAccessToken(user.EmailAddress, new[] { user.Role });

                    // Generate a refresh token
                    var refreshToken = _jwtService.GenerateRefreshToken();

                    // Create a new RefreshToken model to store in the database
                    var refreshTokenModel = new RefreshToken
                    {
                        Token = refreshToken,
                        Expires = DateTime.UtcNow.AddMinutes(60), // Adjust token expiration as per your requirement
                        IsRevoked = false
                    };

                    // Save the refresh token in the database asynchronously
                    await _jwtService.AddRefreshTokenAsync(refreshTokenModel);

                    // Return access token, refresh token, and user information in the response
                    response.Data = new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Role = user.Role,
                        UserId = user.UserId,
                        EmailAddress = user.EmailAddress
                    };

                    return new JsonResult(response);
                }
                else
                {
                    // Role is not authorized for this action
                    response.Status = false;
                    response.Message = errorMessage;
                    return new JsonResult(response);
                }
            }

            // If the user is not authenticated, return invalid email/password error
            response.Status = false;
            response.Message = "Invalid email or password";
            return new JsonResult(response);
        }
        catch (Exception error)
        {
            response.Status = false;
            response.Error = "An internal error occurred";
            return StatusCode(500, response);
        }
    }
}
