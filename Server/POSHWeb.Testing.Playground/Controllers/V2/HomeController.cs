using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace POSHWeb.Testing.Playground.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class HomeController : Controller
    {
        [HttpGet()]
        /// <summary>
        /// Registers a new subscription.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ///<remarks>
        /// Here is a more detailed explanation
        /// </remarks>
        /// <response code="200">When the request has been successful.</response>
        /// <response code="500">When there was a problem processing the request.</response>
        [HttpGet()]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string Get()
        {
            return "Version 2 default";
        }
    }
}
