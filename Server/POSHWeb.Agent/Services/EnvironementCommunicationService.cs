using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Build.Tasks;
using POSHWeb.Agent.Environment;
using POSHWeb.Client.Environment;

namespace POSHWeb.Agent.Services;

[Authorize]
public class EnvironementCommunicationService: ExecuterService.ExecuterServiceBase
{
    private readonly EnvironmentManager _manager;

    public EnvironementCommunicationService(EnvironmentManager manager)
    {
        _manager = manager;
    }
        
    public override Task<RegistrationResponse> Registration(RegistrationRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        var id = GetId(user);
        var settings = _manager.Registration(id);
        return Task.FromResult(new RegistrationResponse
        {
            MinRunspaces = settings.MinRunspaces,
            MaxRunspaces = settings.MaxRunspaces,
            OperationMode = settings.OperationMode,
            ThreadType = settings.ThreadType
        });
    }

    public override async Task ExecutionRequest(Empty request, IServerStreamWriter<ExecutionResponse> responseStream, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        var id = GetId(user);

        if (_manager.HasStream(id))
        {
            throw new RpcException(
                new Status(StatusCode.AlreadyExists, "Environment is already registred for commands"));
        }

        _manager.RegisterStream(id, responseStream);
        try
        {
            await Task.Delay(-1, context.CancellationToken);
        }
        catch (TaskCanceledException)
        {
            _manager.Remove(id);
        }
    }

    public override Task<Empty> Log(LogRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        var id = GetId(user);
        Console.WriteLine(request.Messge);
        return Task.FromResult(new Empty());
    }

    public override Task<SingleInputResponse> SingleInput(Empty request, ServerCallContext context)
    {
        Console.WriteLine("Waiting for Input");
        return Task.FromResult(new SingleInputResponse {Value = Console.ReadLine()});
    }

    private string GetId(ClaimsPrincipal user)
    {
        return user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}