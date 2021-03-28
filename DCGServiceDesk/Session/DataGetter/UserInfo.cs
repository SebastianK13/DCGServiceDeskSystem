using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Data.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Session.DataGetter
{
    class UserInfo:IUserInfo
    {
        private readonly IUserService _userService;
        public UserInfo(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<List<CommunicationInfo>> GetUserName(List<string> eIdContact, List<string> eIdRequested) =>
            await _userService.GetUserName(eIdContact, eIdRequested);

        public async Task<string> GetUserId(string username) =>
            await _userService.GetUserId(username);
    }
}
