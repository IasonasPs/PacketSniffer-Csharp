using PacketDotNet;
using SharpPcap.LibPcap;
using SharpPcap;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using PacketSniffer_Csharp_;

class Program
{
    static int delayTime = 5; // Default delay time in seconds
    static bool isSniffing = false; // To track if sniffing is ongoing

    public static async Task Main()
    {
        MainMenu();
    }

    private static void MainMenu()
    {
        string input = "";
        while (input != "0")
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Begin packet sniffing");
            Console.WriteLine("2) Exit");
            Console.Write("\r\nSelect an option: \n");
            input = Console.ReadLine() ?? "";

            switch (input)
            {
                case "1":
                    string pattern = GeneratePatternLine();
                    Console.WriteLine(pattern);
                    StartPacketSniffing();
                    Console.WriteLine(pattern);
                    break;
                case "2":
                    Console.WriteLine("Exiting the application...");
                    //ExitApplication();
                    return;
                default:
                    Console.WriteLine("Invalid option. Please select a valid option.");
                    break;
            }
        }
    }

    private static string GeneratePatternLine()
    {
        int width = Console.WindowWidth;
        // Define the pattern
        string pattern = "_-";
        // Calculate the required repetitions of the pattern to fill the console width
        int repeatCount = (width / pattern.Length) + 1;
        // Repeat the pattern and truncate it to the exact console width
        string output = string.Concat(Enumerable.Repeat(pattern, repeatCount)).Substring(0, width);
        return output;
    }

    static void StartPacketSniffing()
    {
        if (isSniffing)
        {
            Console.WriteLine("Packet sniffing is already in progress. \n");
            return;
        }

        isSniffing = true;
        //Task.Factory.StartNew(async () => await PacketSniffingProcess());
        PacketSniffingProcess();
    }

    static void PacketSniffingProcess()
    {
        Console.WriteLine("Packet sniffing started...\n");
        // Start the sniffing loop
        while (isSniffing)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(delayTime)))
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    InitializeCaptureDevicesCycle();  // This is where the sniffing happens, could be replaced with real 
                    //Thread.Sleep(1000);
                    //Console.WriteLine("working\n");
                }
            }
            Console.WriteLine($"Sniffing... {delayTime} seconds elapsed");

            //Check if the user wants to change the delay time or stop sniffing
            Console.WriteLine("Press Enter to continue sniffing , or any key for other options ");

            var control = Console.ReadKey();
            if (control.Key != ConsoleKey.Enter)
            {
                Console.WriteLine("Press spaceBar to change delay time, or any key to exit");
                control = Console.ReadKey();
                if (control.Key == ConsoleKey.Spacebar)  // Change delay time
                {
                    ChangeDelayTime();
                }
                else  // Stop sniffing
                {
                    StopPacketSniffing();
                }
            }
        }
        Console.WriteLine("Packet sniffing has stopped.");
    }

    static void StopPacketSniffing()
    {
        Console.WriteLine("\nStopping packet sniffing...");
        isSniffing = false;
        //sniffingThread.Join();  // Wait for the sniffing thread to finish
    }

    static void ChangeDelayTime()
    {
        Console.Write("Enter new delay time in seconds: ");
        int newDelay;
        if (int.TryParse(Console.ReadLine(), out newDelay) && newDelay > 0)
        {
            delayTime = newDelay;
            Console.WriteLine($"Delay time changed to {delayTime} seconds.");
        }
        else
        {
            Console.WriteLine("Invalid delay time entered. Please enter a positive number.");
        }
    }
    private static void InitializeCaptureDevicesCycle(double timeFrame = 1f)
    {
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
    }

    private static void OnPacketArrival(object sender, PacketCapture e)
    {
        try
        {
            // Get the raw packet data
            var rawCapture = e.GetPacket();  // Retrieve the captured packet
            var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Got one packet");

            // Display packet details
            Console.WriteLine(packet.ToString());

            // Example: If the packet is an Ethernet packet, print more details
            if (packet is EthernetPacket ethernetPacket)
            {
                Console.WriteLine("Source MAC: " + ethernetPacket.SourceHardwareAddress);
                Console.WriteLine("Destination MAC: " + ethernetPacket.DestinationHardwareAddress);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing packet: {ex.Message}");
        }
    }
}
