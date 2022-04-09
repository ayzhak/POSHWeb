#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerShell.Commands;
using NJsonSchema.Validation;
using POSHWeb.Data;
using POSHWeb.Enum;
using POSHWeb.Exceptions;
using POSHWeb.Model;
using POSHWeb.ScriptRunner;
using POSHWeb.Services;
using JobState = POSHWeb.Enum.JobState;


namespace POSHWeb.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class ScriptController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ScriptExecuter _scriptExecuter;

        public ScriptController(DatabaseContext context, ScriptExecuter scriptExecuter)
        {
            _scriptExecuter = scriptExecuter;
            _context = context;
        }

        // GET: api/Scripts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PSScript>>> Get()
        {
            return _context.Script
                .Include(script => script.Parameters)
                .ThenInclude(parameter => parameter.Options)
                .ToList();
        }

        // GET: api/Scripts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PSScript>> Get(int id)
        {
            var script = _context.Script
                .Include(psScript => psScript.Parameters)
                .ThenInclude(parameter => parameter.Options)
                .First(psScript => psScript.Id.Equals(id));

            if (script == null)
            {
                return NotFound();
            }

            return script;
        }

        [HttpPost("{id}/run")]
        public async Task<ActionResult<Model.Job>> Run(int id, List<SimpleInputParameter> parameters)
        {
            var script = _context.Script
                .Include(psScript => psScript.Parameters)
                .ThenInclude(parameter => parameter.Options)
                .First(psScript => psScript.Id.Equals(id));

            var inputParameters = new List<InputParameter>();
            var errorList = new List<string>();
            if (parameters.Select(p => p.Name).Distinct().Count() !=
                parameters.Select(parameter => parameter.Name).Count())
                return Problem(statusCode: (int) HttpStatusCode.BadRequest, type: "PARAMETER_NOT_UNIQUE", title: "Parameter names are not unique.");

            foreach (SimpleInputParameter parameter in parameters)
            {
                try
                {
                    inputParameters.Add(new InputParameter
                    {
                        Type = FindMatchingType(script.Parameters, parameter),
                        Name = parameter.Name,
                        Value = parameter.Value,
                        State = JobParameterState.NotValidated
                    });
                }
                catch (InvalidOperationException e)
                {
                    errorList.Add(parameter.Name);
                }
            }

            if (errorList.Count > 0)
            {
                return Problem(statusCode: (int)HttpStatusCode.BadRequest, type: "PARAMETER_NOT_EXIST", title: "One or more validation error.", detail: $"These parameters doesn't exist: {String.Join(" ", errorList)}");
            }

            var job = new Model.Job
            {
                FileName = script.FileName,
                FullPath = script.FullPath,
                Content = script.Content,
                ContentHash = script.ContentHash,
                Parameters = inputParameters,
                Log = ""
            };

            _context.Jobs.Add(job);
            _context.SaveChanges();

            await _scriptExecuter.QueueScriptExecution(job, script);
            return job;
        }

        public class ErrorModel
        {
            public ErrorModel(HttpStatusCode statusCode, string message)
            {
                StatusCode = (int)statusCode;
                Message = message;
                ValidationErrors = new Dictionary<string, ModelErrorCollection>();
            }

            public ErrorModel(HttpStatusCode statusCode)
            {
                StatusCode = (int)statusCode;
                ValidationErrors = new Dictionary<string, ModelErrorCollection>();
            }

            public string Message { get; set; }
            public int StatusCode { get; set; }
            public Dictionary<string, ModelErrorCollection> ValidationErrors { get; set; }
            public Exception Exception { get; set; }
        }

        public class ParameterError
        {
            public string Name { get; set; }
            public string Message {get; set;}
        }

        private string FindMatchingType(ICollection<PSParameter> scriptParameters,
            SimpleInputParameter simpleInputParameter)
        {
            var param = scriptParameters.First(parameter => simpleInputParameter.Name.Equals(parameter.Name));
            return param.Type;
        }

        // PUT: api/Scripts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        [NonAction]
        public async Task<IActionResult> Put(int id, PSScript psScript)
        {
            if (id != psScript.Id)
            {
                return BadRequest();
            }

            _context.Entry(psScript).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
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

        // POST: api/Scripts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        [NonAction]
        public async Task<ActionResult<PSScript>> Post(PSScript psScript)
        {
            _context.Script.Add(psScript);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new {id = psScript.Id}, psScript);
        }

        // DELETE: api/Scripts/5
        //[HttpDelete("{id}")]
        [NonAction]
        public async Task<IActionResult> Delete(int id)
        {
            var script = await _context.Script.FindAsync(id);
            if (script == null)
            {
                return NotFound();
            }

            _context.Script.Remove(script);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.Script.Any(e => e.Id == id);
        }
    }
}