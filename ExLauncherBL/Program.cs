using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ExLauncherBL
{


    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AllocConsole();
            if (args.Length > 0)
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("UserIDs");

                var lines = File.ReadAllLines(args[0]);
                foreach (var line in lines)
                {
                    var split = line.Split(' ');
                    var username = split[0];
                    var password = split[1];

                    Console.WriteLine($"{username} | {password}");

                    var response = LauncherForm.GetServerResponse(username, password);
                    var authToken = response.Element("ex-auth");
                    if (authToken?.Element("error")?.Value == "true")
                        Console.WriteLine($"{username}: Invalid password");
                    else
                    {
                        var session = authToken?.Element("session")?.Value;

                        XElement user = new XElement("user");
                        user.SetAttributeValue("name", username);
                        user.Value = session ?? "";
                        root.Add(user);
                    }
                }

                doc.Add(root);
                doc.Save("UserIDS.xml");
                Console.ReadKey();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LauncherForm());
            }
        }
    }
}
