using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using DCGServiceDesk.Session.CurrentUser;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Services
{
    public class Authorization : IAuthorization
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILoggedUser _loggedUser;

        public string CurrentUsername
        {
            get
            {
                return _loggedUser.ActiveUser;
            }
            set
            {
                _loggedUser.ActiveUser = value;
                StateChanged?.Invoke();
            }
        }
        public bool IsLogged => CurrentUsername != null;

        public event Action StateChanged;

        public Authorization(IUserService userService, IPasswordHasher<User> passwordHasher, ILoggedUser loggedUser)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _loggedUser = loggedUser;
        }
        public async Task<User> Login(string username, string password)
        {
            User user = new User();
            if (CurrentUsername is null)
            {
                user = await _userService.GetByName(username);

                if(user != null)
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
                    if (result.ToString() == "Failed")
                    {
                        return null;
                    }
                    else
                    {
                        _loggedUser.ActiveUser = user.Username;
                    }

                }

            }

            return user;
        }
    }
}