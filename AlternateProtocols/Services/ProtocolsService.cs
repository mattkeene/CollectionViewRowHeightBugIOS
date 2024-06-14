using AlternateProtocols.Models;
using System.Diagnostics;
using System.Text.Json;

namespace AlternateProtocols.Services
{
    public class ProtocolsService : IProtocolsService
    {
        private List<Protocol> AllProtocols { get; set; }
        private List<ProtocolGroup> ProtocolGroupList { get; set; }

        public ProtocolsService()
        {
            AllProtocols = new List<Protocol>();
            ProtocolGroupList = new List<ProtocolGroup>();
        }
        public async Task GetProtocolsFromConfigAsync()
        {
            Debug.WriteLine("GetProtocolsFromConfigAsync() called.");
            try
            {
                var configFile = "config.json";
                using var stream = await FileSystem.OpenAppPackageFileAsync(configFile);
                using var reader = new StreamReader(stream);
                string config = reader.ReadToEnd();
                var configObject = JsonSerializer.Deserialize<List<Protocol>>(config);

                if (configObject != null) AllProtocols = configObject;
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exception
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }
            catch (IOException ex)
            {
                // Handle file IO exception
                Console.WriteLine("Error reading file: " + ex.Message);
            }

            // If protocols is not empty
            if (AllProtocols.Count != 0)
            {
                // Use the protocols object and create a new ProtocolGroup object with all the protocols in the list. The group name should be set to the value of ProtocolCategory within each protocol, and every protocol with the same ProtocolCategory should be in the same group.
                foreach (var protocol in AllProtocols)
                {
                    var group = ProtocolGroupList.FirstOrDefault(g => g.GroupName == protocol.ProtocolCategory);
                    if (group == null)
                    {
                        protocol.ProtocolCategory ??= "Other Protocols";
                        group = new ProtocolGroup(protocol.ProtocolCategory, true, new List<Protocol>());
                        ProtocolGroupList.Add(group);
                    }
                    group.Add(protocol);
                }
                Debug.WriteLine($"ProtocolGroupList created with {ProtocolGroupList.Count} protocols. AllProtocols contains {AllProtocols.Count} protocols.");
            }
            else
            {
                // Return the protocols from the config file
                return;
            }

        }

        public Protocol? GetProtocolByPath(string protocolPath)
        {
            try
            {
                var protocol = AllProtocols.FirstOrDefault(p => p.ProtocolPath == protocolPath);
                if (protocol != null)
                {
                    return protocol;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting protocol: " + ex.Message);
            }
            return null;
        }

        public List<ProtocolGroup> GetProtocolGroupList()
        {
            Debug.WriteLine("GetProtocolGroupList() called.");
            if (ProtocolGroupList.Count == 0)
            {
                GetProtocolsFromConfigAsync().Wait();
            }
            return ProtocolGroupList;
        }

    }
}
