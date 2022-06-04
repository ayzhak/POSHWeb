#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSHWeb.DAL;
using POSHWeb.Model;

namespace POSHWeb.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ScriptController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;

    public ScriptController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PSScript>>> Get()
    {
        return _unitOfWork.ScriptRepository.Get(include: scripts =>scripts 
            .Include(script => script.Help)
            .Include(script => script.Parameters)
            .ThenInclude(parameter => parameter.Options)
            .Include(script => script.Parameters)
            .ThenInclude(parameter => parameter.Default))
            .ToList();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PSScript>> Get(int id)
    {
        var script = _unitOfWork.ScriptRepository.Get(
                filter: psScript => psScript.Id == id,
                include: scripts =>scripts 
                .Include(script => script.Help)
                .Include(script => script.Parameters)
                .ThenInclude(parameter => parameter.Options)
                .Include(script => script.Parameters)
                .ThenInclude(parameter => parameter.Default))
            .First();

        if (script == null) return NotFound();

        return script;
    }
    
    [NonAction]
    public async Task<IActionResult> Put(int id, PSScript psScript)
    {
        if (id != psScript.Id) return BadRequest();
        
        try
        {
            _unitOfWork.ScriptRepository.Update(psScript);
            _unitOfWork.Save();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_unitOfWork.ScriptRepository.Exist(script => script.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }
    
    [NonAction]
    public async Task<ActionResult<PSScript>> Post(PSScript psScript)
    {
        _unitOfWork.ScriptRepository.Update(psScript);
        _unitOfWork.Save();

        return CreatedAtAction("Get", new {id = psScript.Id}, psScript);
    }
    
    
    [NonAction]
    public async Task<IActionResult> Delete(int id)
    {
        _unitOfWork.ScriptRepository.Delete(id);
        _unitOfWork.Save();
        return NoContent();
    }
}