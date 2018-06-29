using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReStart2.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModel
    {
            [Required]
        [StringLength(7, ErrorMessage = "1235", MinimumLength = 6)]
        [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
    }
}
