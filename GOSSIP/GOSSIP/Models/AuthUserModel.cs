using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Models
{
    public class AuthUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public AuthUserModel() { }
    }
}