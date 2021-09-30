using System.Linq;
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
    [Authorize]
    [Route("api/[controller]")]
    public sealed class MinMaxController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public MinMaxController(
            ApplicationDbContext dbContext,
            IMapper mapper
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetValuesAsync()
        {
            if (!User.HasPermission(Permission.SuperAdmin))
            {
                return Forbid();
            }

            return Ok(new
            {
                Prices = await _dbContext.Prices.ToListAsync(),
                Floors = await _dbContext.Floors.ToListAsync(),
            });
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveValuesAsync(ValuesDto dto)
        {
            if (!User.HasPermission(Permission.SuperAdmin))
            {
                return Forbid();
            }

            var prices = await _dbContext.Prices.ToListAsync();
            var floors = await _dbContext.Floors.ToListAsync();

            var pricesIdsToDelete = prices
                .Select(p => p.Id)
                .Except(
                    dto.Prices.Where(dp => dp.Id.HasValue).Select(dp => dp.Id.Value)
                );

            var pricesToDelete = prices.Where(p => pricesIdsToDelete.FirstOrDefault(id => p.Id == id) != default);

            var floorsIdsToDelete = floors
                .Select(f => f.Id)
                .Except(
                    dto.Floors.Where(df => df.Id.HasValue).Select(df => df.Id.Value)
                );

            var floorsToDelete = floors.Where(f => floorsIdsToDelete.FirstOrDefault(id => f.Id == id) != default);

            foreach (var priceDto in dto.Prices)
            {
                if (!priceDto.Id.HasValue) continue;

                var price = prices.FirstOrDefault(p => p.Id == priceDto.Id.Value);

                if (price is null)
                {
                    _dbContext.Prices.Add(_mapper.Map<Price>(priceDto));
                    continue;
                }

                _mapper.Map(price, priceDto);
            }

            foreach (var floorDto in dto.Floors)
            {
                if (!floorDto.Id.HasValue) continue;

                var floor = floors.FirstOrDefault(f => f.Id == floorDto.Id.Value);

                if (floor is null)
                {
                    _dbContext.Floors.Add(_mapper.Map<Floor>(floorDto));
                    continue;
                }

                _mapper.Map(floor, floorDto);
            }

            _dbContext.Prices.RemoveRange(pricesToDelete);
            _dbContext.Floors.RemoveRange(floorsToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}