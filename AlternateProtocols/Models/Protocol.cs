namespace AlternateProtocols.Models
{
    public class Protocol
    {
        public string? ProtocolCategory { get; set; }
        public string? ProtocolTitle { get; set; }
        public string? ProtocolDescription { get; set; }
        public string? ProtocolPath { get; set; }
        public bool IsPDF { get; set; }

        public override string ToString()
        {
            return $"Category: {ProtocolCategory}, Title: {ProtocolTitle}, Description: {ProtocolDescription}, Path: {ProtocolPath}, IsPDF: {IsPDF} \n";
        }

    }
}
