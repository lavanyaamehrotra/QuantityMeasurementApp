using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QmaService.Models;
using QmaService.Services;

namespace QmaService.Controllers
{
    [ApiController]
    [Route("api/qma")]
    public class QmaController : ControllerBase
    {
        private readonly IMeasurementService _svc;

        public QmaController(IMeasurementService svc) => _svc = svc;

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : null;
        }

        // ── Operations — no login required ───────────────────────────────

        [HttpPost("compare")]
        [AllowAnonymous]
        public async Task<IActionResult> Compare([FromBody] TwoOperandRequest req)
            => Ok(await _svc.CompareAsync(req.Operand1, req.Operand2, GetUserId()));

        [HttpPost("convert")]
        [AllowAnonymous]
        public async Task<IActionResult> Convert([FromBody] ConvertRequest req)
            => Ok(await _svc.ConvertAsync(req.Operand1, req.Target, GetUserId()));

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] TwoOperandRequest req)
            => Ok(await _svc.AddAsync(req.Operand1, req.Operand2, GetUserId()));

        [HttpPost("subtract")]
        [AllowAnonymous]
        public async Task<IActionResult> Subtract([FromBody] TwoOperandRequest req)
            => Ok(await _svc.SubtractAsync(req.Operand1, req.Operand2, GetUserId()));

        [HttpPost("divide")]
        [AllowAnonymous]
        public async Task<IActionResult> Divide([FromBody] TwoOperandRequest req)
            => Ok(await _svc.DivideAsync(req.Operand1, req.Operand2, GetUserId()));

        // ── History — login required ──────────────────────────────────────

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetHistory()
            => Ok(await _svc.GetHistoryAsync());

        [HttpGet("history/operation/{operation}")]
        [Authorize]
        public async Task<IActionResult> GetByOperation(string operation)
            => Ok(await _svc.GetHistoryByOperationAsync(operation));

        [HttpGet("history/category/{category}")]
        [Authorize]
        public async Task<IActionResult> GetByCategory(string category)
            => Ok(await _svc.GetHistoryByCategoryAsync(category));

        [HttpGet("history/user")]
        [Authorize]
        public async Task<IActionResult> GetByUser()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();
            return Ok(await _svc.GetHistoryByUserAsync(userId.Value));
        }

        [HttpGet("history/count")]
        [Authorize]
        public async Task<IActionResult> GetCount()
            => Ok(new { count = await _svc.GetCountAsync() });

        [HttpDelete("history")]
        [Authorize]
        public async Task<IActionResult> ClearHistory()
        {
            await _svc.ClearHistoryAsync();
            return Ok(new { message = "History cleared." });
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health() => Ok(new { status = "healthy", service = "qma-service" });
    }
}