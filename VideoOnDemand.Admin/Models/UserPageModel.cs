using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoOnDemand.Admin.Models
{
    public class UserPageModel
    {
        [Required]
        [Display(Name = "User Id")]
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }
    }
}
