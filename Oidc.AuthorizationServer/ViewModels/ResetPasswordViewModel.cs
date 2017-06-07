using System.ComponentModel.DataAnnotations;

namespace lvl.Oidc.AuthorizationServer.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
