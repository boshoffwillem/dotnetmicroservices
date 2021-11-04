using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepository repository,
                                   IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
        {
            Console.WriteLine("--> Getting platforms");
            IEnumerable<Platform> platforms = await _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            Platform platform = await _repository.GetPlatformById(id);

            if (platform is null)
            {
                // Return 404 status.
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto newPlatform)
        {
            Platform platformModel = _mapper.Map<Platform>(newPlatform);
            await _repository.CreatePlatform(platformModel);
            _ = await _repository.SaveChanges();
            PlatformReadDto returnPlatform = _mapper.Map<PlatformReadDto>(platformModel);
            return CreatedAtRoute(
                nameof(GetPlatformById),
                new
                {
                    returnPlatform.Id
                },
                returnPlatform);

        }
    }
}
