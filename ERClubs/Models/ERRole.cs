using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERClubs.Models
{
    public class ERRole
    {
        [Required]
        public string RoleName { get; set; }
    }
}
