using System.Threading;
using System.Threading.Tasks;
using LibraryManagement.Application.Features.Dashboard.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.API.Controllers;
[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "ADMIN")]
public class AdminDashboardController : ControllerBase
{
    private readonly ISender _sender;
    public AdminDashboardController(ISender sender) => _sender = sender;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _sender.Send(new GetDashboardQuery(), ct));
}
