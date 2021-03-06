﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Gebruikersnaam")]
        [Required(ErrorMessage = "Vul gebruikersnaam in")]
        [Remote("CheckUserName", "Account", ErrorMessage = "Uw gebruikersnaam is incorrect, controleer dit aub")]
        public string UserName { get; set; }
    }
}