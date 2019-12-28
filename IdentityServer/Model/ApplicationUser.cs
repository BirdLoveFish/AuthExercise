using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Model
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser(){ }
        public ApplicationUser(string username):base(username)
        { }
        public string CustomTag { get; set; }
    }
}
