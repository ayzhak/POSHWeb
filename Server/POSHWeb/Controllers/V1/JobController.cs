using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSHWeb.Data;
using POSHWeb.Model;

namespace POSHWeb.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class JobController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public JobController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/ScriptJobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.Include(job => job.Parameters).ToListAsync();
        }

        // GET: api/ScriptJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetScriptJob(int id)
        {
            var scriptJob = await _context.Jobs.Include(job => job.Parameters).FirstAsync(job => job.Id == id);

            if (scriptJob == null)
            {
                return NotFound();
            }

            return scriptJob;
        }

        // PUT: api/ScriptJobs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [NonAction]
        public async Task<IActionResult> PutScriptJob(int id, Job scriptJob)
        {
            if (id != scriptJob.Id)
            {
                return BadRequest();
            }

            _context.Entry(scriptJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScriptJobExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ScriptJobs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        [NonAction]
        public async Task<ActionResult<Job>> PostScriptJob(Job scriptJob)
        {
            _context.Jobs.Add(scriptJob);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScriptJob", new { id = scriptJob.Id }, scriptJob);
        }

        // DELETE: api/ScriptJobs/5
        //[HttpDelete("{id}")]
        [NonAction]
        public async Task<IActionResult> DeleteScriptJob(int id)
        {
            var scriptJob = await _context.Jobs.Include(job => job.Parameters).FirstAsync(job => job.Id == id);
            if (scriptJob == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(scriptJob);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ScriptJobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
