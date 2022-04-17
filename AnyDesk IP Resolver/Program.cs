using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AnyDesk_IP_Resolver {
    internal class Program {

        static List<string> non_duplicates = new List<string>();

        static WebClient wc = new WebClient();
        static Stopwatch sw = new Stopwatch();

        static string date = "", ip = "";

        static void Main(string[] args) {
            Console.CursorVisible = false;

            if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.trace")) {
                sw.Start();

                File.Copy($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.trace",
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.traceCopy");

                string file = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.traceCopy");

                foreach (string line in file.Split('\n')) {
                    if (line.Contains("anynet.any_socket - Logged in from")) {
                        ip = line.Substring(0, line.Length - 1);
                        ip = ip.Substring(0, ip.Length - 25).Substring(112);

                        date = line.Substring(0, line.Length - 1);
                        date = date.Substring(0, date.Length - 117).Substring(8);

                        if (!non_duplicates.Contains(ip)) {
                            Console.WriteLine($" [#] {ip} ({date.Replace("-", "/").Substring(0, 10)})");

                            string[] ipData = wc.DownloadString($"http://ip-api.com/line/{ip}").Split('\n');

                            Console.WriteLine($"  - Country: {ipData.ElementAt(1)}");
                            Console.WriteLine($"  - State: {ipData.ElementAt(4)}");
                            Console.WriteLine($"  - City: {ipData.ElementAt(5)}");
                            Console.WriteLine($"  - ISP: {ipData.ElementAt(11)}\n");

                            non_duplicates.Add(ip);
                        }
                    }
                }

                Console.WriteLine($"Finished resolving in {sw.ElapsedMilliseconds}ms");
            } else {
                Console.WriteLine($"Failed to get AnyDesk file.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.traceCopy"))
                File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//AnyDesk/ad.traceCopy");

            Console.ReadLine();
        }
    }
}
