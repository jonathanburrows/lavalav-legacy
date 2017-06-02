using lvl.Oidc.AuthorizationServer.Services;
using lvl.Oidc.AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    /// <summary>
    ///     Exposes the PersonDetailsEditor service.
    /// </summary>
    [Route("oidc/personal-details")]
    [Authorize]
    public class PersonalDetailsController : ControllerBase
    {
        private PersonalDetailsEditor PersonalDetailsEditor { get; }

        public PersonalDetailsController(PersonalDetailsEditor personalDetailsEditor)
        {
            PersonalDetailsEditor = personalDetailsEditor ?? throw new ArgumentNullException(nameof(personalDetailsEditor));
        }

        /// <summary>
        ///     Will return flat model containing user claims.
        /// </summary>
        /// <returns>A model containing the current user's claims.</returns>
        [HttpGet]
        public async Task<PersonalDetailsViewModel> Get()
        {
            var user = await PersonalDetailsEditor.GetCurrentUserAsync();
            return PersonalDetailsEditor.GetPersonalDetails(user);
        }

        /// <summary>
        ///     Updates a users claims based on the flat model given.
        /// </summary>
        /// <param name="model">A flat model of the claims to be created/updated/removed.</param>
        /// <returns>
        ///     If the claims are invalid, then model errors.
        ///     
        ///     Otherwise, an Ok status.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="model"/> is null.</exception>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]PersonalDetailsViewModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await PersonalDetailsEditor.GetCurrentUserAsync();
            await PersonalDetailsEditor.UpdateUsersPersonalDetailsAsync(user, model);
            return Ok();
        }
    }
}
