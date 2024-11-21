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
            Console.WriteLine("1 : Begin packet sniffing with a 3 seconds default \"time frame\" for every capture device ");
            Console.WriteLine("2 : Print CaptureDeviceList");
            Console.WriteLine("3 : What is time frame");
            Console.WriteLine("9 : Exit");
            Console.WriteLine();
            //Console.WriteLine("<<Default delay time for every capture device is 3 seconds>>");
            Console.Write("\r\nSelect an option: \n");
            input = Console.ReadLine() ?? "";
            string pattern = Utilities.GeneratePatternLine();


            switch (input)
            {
                case "1":
                    Console.Clear();
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
                case "3":
                    Console.Clear();
                    Console.WriteLine(pattern);
                    var timeFrameDefinition = "Time frame is the time in seconds that the application will wait  before stopping the packet sniffing process for an individual capture device. This time is used to capture packets from the network. The default delay time is 3 seconds.";
                    Console.WriteLine(timeFrameDefinition);
                    Console.WriteLine(pattern);
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