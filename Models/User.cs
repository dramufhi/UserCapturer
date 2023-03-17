using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserCapturer.Models
{
    public enum Gender
    {
        Male,
        Female,
        Neutral
    }
    public class User
    {
        [Required(ErrorMessage = "ID is Required")]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "First name")]
        [MinLength(3)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [MinLength(3)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Cellphone")]
        [MaxLength(10)]
        [MinLength(10)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string Cellphone { get; set; }
      
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Sex { get; set; }
    }
}