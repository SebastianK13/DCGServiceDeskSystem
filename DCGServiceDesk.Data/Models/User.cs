using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Data.Models
{
    public class User
    {
        public User() { }

        public User(object user)
        {
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}