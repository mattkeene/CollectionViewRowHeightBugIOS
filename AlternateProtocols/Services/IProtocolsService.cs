using AlternateProtocols.Models;

namespace AlternateProtocols.Services
{
    public interface IProtocolsService
    {
        public Protocol? GetProtocolByPath(string protocolPath);
        public List<ProtocolGroup> GetProtocolGroupList();
        public Task GetProtocolsFromConfigAsync();

    }
}
