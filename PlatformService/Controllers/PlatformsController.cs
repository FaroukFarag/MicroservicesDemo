using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepository repository, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platform = _mapper.Map<Platform>(platformCreateDto);

            _repository.CreatePlatform(platform);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            // Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }

            catch (Exception ex)
            {
                Console.Write($"--> Could not send synchronously: {ex.Message}");
            }

            // Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);

                platformPublishedDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }

            catch(Exception ex)
            {
                Console.Write($"--> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetById), new { Id = platformReadDto.Id }, platformReadDto);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("--> Getting Platforms...");

            return Ok(_mapper.Map<IReadOnlyList<PlatformReadDto>>(_repository.GetAllPlatforms()));
        }

        [HttpGet("{id}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var platform = _repository.GetPlatformById(id);

            if (platform == null)
                return NotFound();

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }
    }
}
