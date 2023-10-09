using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepository
    {
        // Platform
        void CreatePlatform(Platform platform);
        IEnumerable<Platform> GetAllPlatforms();
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);

        // Commands
        void CreateCommand(int platformId, Command command);
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);

        bool SaveChanges();
    }
}
