using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketSniffer_Csharp_
{
    internal class timeCheck
    {
        public static async Task TimeMain()
        {

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    Console.WriteLine("workin...");
                }

            }



        }
        private static async Task<bool> WaitASync(int delayTime)
        {
            await Task.Delay(delayTime);
            return true;
        }
    }
}
