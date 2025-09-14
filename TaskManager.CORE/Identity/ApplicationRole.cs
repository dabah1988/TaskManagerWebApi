using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.Identity
{
    public class ApplicationRole:IdentityRole<Guid>
    {
        public string? RoleName { get; set; }
        public  string? DescriptionRole { get; set; }

    }
}
