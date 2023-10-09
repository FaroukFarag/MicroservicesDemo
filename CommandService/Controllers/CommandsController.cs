using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();

            var command = _mapper.Map<Command>(commandDto);

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(
                nameof(GetCommandForPlatform), 
                new { platformId = platformId, commandId = commandReadDto.Id }, 
                commandReadDto);
        }

        [HttpGet]
        public IActionResult GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            if(!_repository.PlatformExists(platformId))
                return NotFound();

            return Ok(_mapper.Map<IReadOnlyList<CommandReadDto>>(_repository.GetCommandsForPlatform(platformId)));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public IActionResult GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }
    }
}
