using Microsoft.AspNetCore.Mvc;
using Schnittpunkte.Services;
using Schnittpunkte.Models;

namespace Schnittpunkte.Controllers
{
    /// <summary>
    /// Controller for calculating intersections of geometric objects.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SchnittpunkteController : ControllerBase
    {
        private readonly SchnittpunkteService _schnittpunkteService;
        /// <summary>
        /// Initializes a new instance of the SchnittpunkteController class.
        /// </summary>
        public SchnittpunkteController()
        {
            _schnittpunkteService = new SchnittpunkteService();
        }
        /// <summary>
        /// Calculates the intersection of geometric objects based on the provided request.
        /// </summary>
        /// <param name="request">The request containing geometric objects.</param>
        /// <returns>An IActionResult representing the intersection result or an error response.</returns>
        [HttpPost("intersection")]
        public IActionResult CalculateIntersection([FromBody] RequestForm request)
        {
            Result intersects = _schnittpunkteService.CalculateIntersection(request);
            
            if (intersects == null) 
            {
                return BadRequest("Invalid Request");
            }
            else if (intersects.status=="Invalid")
            {
                return BadRequest(intersects);
            }
            return Ok(intersects);
        }
    }
}


