using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Masya.TelegramBot.Api.Dtos;
using Masya.TelegramBot.DataAccess;
using Masya.TelegramBot.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Masya.TelegramBot.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var idClaim = User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier);
            if (long.TryParse(idClaim.Value, out long telegramUserId))
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.TelegramAccountId == telegramUserId);
                if (user is null)
                {
                    return BadRequest(new ResponseDto<object>("Invalid access token."));
                }
                var dto = new AccountDto(user);
                return Ok(dto);
            }

            return BadRequest(new ResponseDto<object>("Invalid access token."));
        }

        [HttpGet("/")]
        public async Task<IActionResult> LoadUsersAsync()
        {
            if (!User.HasPermission(Permission.SuperAdmin)) return Forbid();

            var users = await _dbContext.Users.Include(u => u.Agency).ToListAsync();
            var dtos = _mapper.Map<UserDto[]>(users);

            return Ok(dtos);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveUsersAsync(UserDto[] dtos)
        {
            if (!User.HasPermission(Permission.SuperAdmin)) return Forbid();
            var users = await _dbContext.Users.ToListAsync();

            foreach (var dto in dtos)
            {
                var user = users.FirstOrDefault(u => u.Id == dto.Id);
                if (user is null) continue;
                _mapper.Map(dto, user);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}