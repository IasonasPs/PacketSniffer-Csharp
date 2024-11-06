using System;
using System.IO;
using PacketDotNet;
using SharpPcap;

public class PcapWriter
{
    private FileStream _fileStream;
    private BinaryWriter _writer;
    private int packetSum = 0;

    public PcapWriter(string filePath)
    {
        try
        {
            _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,FileShare.None);

        }
        catch (Exception e)
        {
            Console.WriteLine("Bingo!");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException.Message);
        }
        _writer = new BinaryWriter(_fileStream);

        // Write the global header for pcap
        WriteGlobalHeader();
    }

    private void WriteGlobalHeader()
    {
        // Standard pcap global header
        _writer.Write(0xa1b2c3d4); // Magic number
        _writer.Write((short)2);   // Major version number
        _writer.Write((short)4);   // Minor version number
        _writer.Write(0);          // Time zone offset (GMT)
        _writer.Write(0);          // Accuracy of timestamps
        _writer.Write(65535);      // Max length of captured packets
        _writer.Write(1);          // Data link type (Ethernet)
    }

    public void CapturePacket(RawCapture e)
    {
        try
        {
            packetSum++;
            string pattern = GeneratePatternLine("_*");

            // Retrieve and parse the packet
            var packet = Packet.ParsePacket(e.LinkLayerType, e.Data);
            Console.WriteLine(pattern);
            Console.WriteLine($"Got {packetSum} packet");

            // Display packet details in console (optional)
            Console.WriteLine(packet.ToString());

            // If Ethernet packet, display MAC addresses (optional)
            if (packet is EthernetPacket ethernetPacket)
            {
                Console.WriteLine("Source MAC: " + ethernetPacket.SourceHardwareAddress);
                Console.WriteLine("Destination MAC: " + ethernetPacket.DestinationHardwareAddress);
            }
            Console.WriteLine(pattern);

            // Write packet header and data to the .pcap file
            WritePacket(e);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing packet: {ex.Message}");
        }
    }

    public void WritePacket(RawCapture rawCapture)
    {
        // Write the per-packet header
        var timestamp = rawCapture.Timeval;
        _writer.Write((int)timestamp.Seconds);          // Timestamp seconds
        _writer.Write((int)timestamp.MicroSeconds);     // Timestamp microseconds
        _writer.Write(rawCapture.Data.Length);          // Number of octets saved
        _writer.Write(rawCapture.Data.Length);          // Original length of the packet

        // Write the raw packet data
        _writer.Write(rawCapture.Data);
    }

    public void Close()
    {
        _writer.Close();
        _fileStream.Close();
    }

    private string GeneratePatternLine(string pattern)
    {
        return new string('_', 50); // Modify as needed to match your original pattern logic
    }
}
