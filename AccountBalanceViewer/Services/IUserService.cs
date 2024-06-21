using AccountBalanceViewer.Authentication;
using AccountBalanceViewer.Models;

namespace AccountBalanceViewer.Services
{
    public interface IUserService
    {
        Task<Response> Register(User newUser);

        Task<Response> Login(User loggedUser);

        Task<Response> Logout(string token);
    }
}
