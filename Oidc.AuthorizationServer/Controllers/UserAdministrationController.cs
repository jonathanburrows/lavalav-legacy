using lvl.Oidc.AuthorizationServer.Models;
using lvl.Repositories;
using lvl.Web.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    /// <summary>
    ///     Exposes some api like functionality to administrators.
    /// </summary>
    [Route("oidc/user-administration")]
    [Authorize(Roles = "administrator")]
    public class UserAdministrationController : ControllerBase
    {
        private IRepository<User> UserRepository { get; }
        private ODataQueryParser ODataQueryParser { get; }

        public UserAdministrationController(IRepository<User> userRepository, ODataQueryParser odataQueryParser)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ODataQueryParser = odataQueryParser ?? throw new ArgumentNullException(nameof(odataQueryParser));
        }

        [HttpGet("odata")]
        public async Task<ODataResponse> QueryUsers()
        {
            var query = ODataQueryParser.Parse(Request.Query, typeof(User));

            var queryResult = await ((IRepository)UserRepository).GetAsync(query);
            return new ODataResponse
            {
                Context = Request.GetDisplayUrl(),
                Value = queryResult.Items,
                Count = queryResult.Count
            };
        }

        [HttpGet("api/{id}")]
        public async Task<User> Get(int id)
        {
            var user = await UserRepository.GetAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException($"No user was found with id {id}");
            }

            return user;
        }

        [HttpPut("api")]
        public async Task<IActionResult> Update([FromBody]User updating)
        {
            if (updating == null)
            {
                throw new ArgumentNullException(nameof(updating));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await UserRepository.UpdateAsync(updating);
            return Ok(updating);
        }
    }
}
