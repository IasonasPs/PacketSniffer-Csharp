using System;
using System.Threading;

public class Check
{
    static int delayTime = 50; // Default delay time in seconds
    static bool isSniffing = false; // To track if sniffing is ongoing
    static Thread sniffingThread; // Thread for sniffing

    public static void CheckMain()
    {
        string input = "";
        while (input != "0")
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Begin packet sniffing");
            Console.WriteLine("2) Exit");
            Console.Write("\r\nSelect an option: ");
            input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    StartPacketSniffing();
                    break;

                case "2":
                    Console.WriteLine("Exiting the application...");
                    ExitApplication();
                    break;

                default:
                    Console.WriteLine("Invalid option. Please select a valid option.");
                    break;
            }
        }
    }

    // Start packet sniffing in a separate thread
    static void StartPacketSniffing()
    {
        if (isSniffing)
        {
            Console.WriteLine("Packet sniffing is already in progress.");
            return;
        }

        isSniffing = true;
        sniffingThread = new Thread(PacketSniffingProcess);
        sniffingThread.Start();
    }

    // Simulated packet sniffing process
    static void PacketSniffingProcess()
    {
        Console.WriteLine("Packet sniffing started...");
        int sniffDuration = 0;

        // Start the sniffing loop
        while (isSniffing)
        {
            // Simulate sniffing work every 5 seconds
            Thread.Sleep(5000);  // This is where the sniffing happens, could be replaced with real sniffing code
            sniffDuration = 5;
            Console.WriteLine($"Sniffing... {sniffDuration} seconds elapsed");


            //Check if the user wants to change the delay time or stop sniffing
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.C)  // Change delay time
                {
                    ChangeDelayTime();
                }
                else if (key == ConsoleKey.S)  // Stop sniffing
                {
                    StopPacketSniffing();
                }
            }
        }

        Console.WriteLine("Packet sniffing has stopped.");
    }

    // Allow the user to change the check delay time while sniffing
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

    // Stop the sniffing process
    static void StopPacketSniffing()
    {
        Console.WriteLine("\nStopping packet sniffing...");
        isSniffing = false;
        sniffingThread.Join();  // Wait for the sniffing thread to finish
    }

    // Stop sniffing and exit the application
    static void ExitApplication()
    {
        isSniffing = false;
        sniffingThread?.Join();  // Ensure sniffing thread finishes before exiting
    }
}
