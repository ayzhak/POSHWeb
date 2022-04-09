using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POSHWeb.Controllers.V1;

[Route("api/config")]
[ApiController]
public class ApplicationController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public ApplicationSettings Get()
    {
        return new ApplicationSettings();
    }

    [HttpPut]
    public ApplicationSettings Get(ApplicationSettings applicationSettings)
    {
        return new ApplicationSettings();
    }
} 

public class ApplicationSettings
{
    public string ApiBaseUri { get; set; }
    public string SignalRUri { get; set; }
    public string ApplicationName { get; set; } = "POSHWeb";
    public string OrganizationName { get; set; } = "Public Testing API";
    public bool MaintenanceEnabled { get; set; } = false;
    public string ServerMessage { get; set; }
    public string AuthenticationMethod { get; set; } = "NoAuthentication";
}