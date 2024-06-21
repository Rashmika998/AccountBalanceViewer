using AccountBalanceViewer.Authentication;
using AccountBalanceViewer.Models;
using AccountBalanceViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(User newUser)
        {
            var response = await _userService.Register(newUser);
            if(response.StatusCode==StatusCodes.Status400BadRequest)
                return Unauthorized(new Response { StatusCode = 400, Message = response.Message });

            return Ok(response.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(User loggedUser)
        {
            var response = await _userService.Login(loggedUser);
            if (response.StatusCode == StatusCodes.Status404NotFound)
                return Unauthorized(new Response { StatusCode = StatusCodes.Status400BadRequest, Message = response.Message });

            return Ok(new
            {
                token = response.Message.Split("VALIDTO")[0],
                expiration = response.Message.Split("VALIDTO")[1],
                userRole = loggedUser.UserRole
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string token)
        {
            var response = await _userService.Logout(token);
            if (response.StatusCode == StatusCodes.Status404NotFound)
                return Unauthorized(new Response { StatusCode = StatusCodes.Status400BadRequest, Message = response.Message });

            return Ok( new Response { StatusCode = StatusCodes.Status200OK, Message = response.Message });
        }
    }
}
