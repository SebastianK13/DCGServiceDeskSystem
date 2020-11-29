using DCGServiceDesk.Data.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Services
{
    public class Authorization : IAuthorization
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public Authorization(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }
        public async Task<User> Login(string username, string password)
        {
            User currentUser = await _userService.GetByName(username);

            if (currentUser is null)
            {

            }

            PasswordVerificationResult verificationResult = _passwordHasher.VerifyHashedPassword(currentUser.Password, password);

            return currentUser;
        }
    }
}