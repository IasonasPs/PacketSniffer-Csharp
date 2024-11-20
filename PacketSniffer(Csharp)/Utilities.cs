using PacketDotNet;
using SharpPcap;
using System.Text;

internal class Utilities
{
    static double timeFrame = 3f; // Default delay time in seconds
    static bool isSniffing = false; // To track if sniffing is ongoing
    private static int packetSum = 0;
    private static PcapWriter? pCapWriter;

    internal static void StartPacketSniffing()
    {
        if (isSniffing)
        {
            Console.WriteLine("Packet sniffing is already in progress. \n");
            return;
        }

        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.pcap");


        pCapWriter = new PcapWriter(filePath);
        isSniffing = true;
        //Task.Factory.StartNew(async () => await PacketSniffingProcess());
        PacketSniffingProcess();

    }

    internal static void PacketSniffingProcess()
    {
        Console.WriteLine("Packet sniffing started...\n");
        // Start the sniffing loop
        while (isSniffing)
        {
            InitializeCaptureDevicesCycle();  // This is where the sniffing happens

            //Check if the user wants to change timeFrame or stop sniffing
            Console.WriteLine("Press Enter to continue sniffing , or any key for other options ");

            var control = Console.ReadKey();
            if (control.Key != ConsoleKey.Enter)
            {
                Console.WriteLine("Press spaceBar to change timeFrame, or any key to exit");
                control = Console.ReadKey();
                if (control.Key == ConsoleKey.Spacebar)  // Change delay time
                {
                    ChangeTimeFrame();
                }
                else  // Stop sniffing
                {
                    StopPacketSniffing();
                }
            }
        }
        Console.WriteLine("Packet sniffing has stopped.");
    }

    internal static void InitializeCaptureDevicesCycle()
    {
        packetSum = 0;
        var devices = CaptureDeviceList.Instance;

        foreach (var item in devices) //This is a whole cycle of sniffing
        {
            if (item == null)
                return;

            var device = item;
            try
            {
                device.Open(DeviceModes.Promiscuous);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening device: {ex.Message}");
                return;
            }

            //Register an event handler for packet arrival
            device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);

            // Start the capture
            device.StartCapture();

            //Console.WriteLine($"Capturing for {timeFrame} seconds...");
            System.Threading.Thread.Sleep((int)timeFrame * 1000);

            device.StopCapture();
            device.Close();
        }
        pCapWriter?.Close();
        Console.WriteLine();
    }

    internal static void OnPacketArrival(object sender, PacketCapture e)
    {
        RawCapture? rawCapture = null;
        string pattern = GeneratePatternLine("_*");
        try
        {
            packetSum++;

            // Get the raw packet data
            rawCapture = e.GetPacket();  // Retrieve the captured packet
            var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Console.WriteLine(pattern);
            Console.WriteLine($"Got {packetSum} packet");

            // Display packet details
            Console.WriteLine(packet.ToString());

            // Example: If the packet is an Ethernet packet, print more details
            if (packet is EthernetPacket ethernetPacket)
            {
                Console.WriteLine("Source MAC: " + ethernetPacket.SourceHardwareAddress);
                Console.WriteLine("Destination MAC: " + ethernetPacket.DestinationHardwareAddress);
            }
            Console.WriteLine(pattern);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing packet: {ex.Message}");
        }
        if (rawCapture is not null)
        {
            pCapWriter?.WritePacket(rawCapture);
        }
        else
        {
            Console.WriteLine("Packet is null..");
        }
    }

    static void ChangeTimeFrame()
    {
        Console.Write("Enter new timeFrame in seconds: ");
        int newFrame;
        if (int.TryParse(Console.ReadLine(), out newFrame) && newFrame > 0)
        {
            timeFrame = newFrame;
            Console.WriteLine($"timeFrame changed to {timeFrame} seconds.");
        }
        else
        {
            Console.WriteLine("Invalid timeFrame entered. Please enter a positive number.");
        }
    }

    internal static string GeneratePatternLine(string pattern = "_-")
    {
        int width = Console.WindowWidth;
        // Calculate the required repetitions of the pattern to fill the console width
        int repeatCount = (width / pattern.Length) + 1;
        // Repeat the pattern and truncate it to the exact console width
        string output = string.Concat(Enumerable.Repeat(pattern, repeatCount)).Substring(0, width);
        return output;
    }

    internal static void StopPacketSniffing()
    {
        Console.WriteLine("\nStopping packet sniffing...");
        isSniffing = false;
    }
}