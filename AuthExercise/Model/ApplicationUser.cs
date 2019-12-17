using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string CustomTag { get; set; }
    }
}
