using SharpPcap;
using System;
internal static class Menu
{
    internal static void MainMenu()
    {
        string input = "";
        while (input != "0")
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Begin packet sniffing");
            Console.WriteLine("2) Print CaptureDeviceList");
            Console.WriteLine("9) Exit");
            Console.WriteLine();
            Console.WriteLine("<<Default delay time for every capture device is 3 seconds");
            Console.Write("\r\nSelect an option: \n");
            input = Console.ReadLine() ?? "";
            string pattern = Utilities.GeneratePatternLine();


            switch (input)
            {
                case "1":
                    Console.WriteLine(pattern);
                    Utilities.StartPacketSniffing();
                    Console.WriteLine(pattern);
                    break;
                case "2":
                    var devices = CaptureDeviceList.Instance;
                    Console.WriteLine(pattern);
                    devices.ToList().ForEach(li =>
                    {
                        Console.WriteLine(li);
                        Console.WriteLine(pattern);
                    });
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                case "9":
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