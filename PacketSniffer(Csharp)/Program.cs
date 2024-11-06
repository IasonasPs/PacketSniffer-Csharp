using PacketDotNet;
using SharpPcap.LibPcap;
using SharpPcap;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

class Program : Utilities
{
    

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
                    string pattern = Utilities.GeneratePatternLine();
                    Console.WriteLine(pattern);
                    Utilities.StartPacketSniffing();
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
}
