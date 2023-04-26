using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32;

namespace jetBrainProductsCracker
{
    public class Program
    {
        private static int _selectedIndex = -1;

        private static readonly List<string> JetBrainsProducts = new()
            {"Rider", "WebStorm", "PhpStorm", "CLion", "RubyMine", "GoLand"};

        private static bool IsAdmin()
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdminRights = principal.IsInRole(WindowsBuiltInRole.Administrator);
            return hasAdminRights;
        }

        private static string GetPath()
        {
            var username = Environment.UserName;
            string path = @$"C:\Users\{username}\AppData\Roaming\JetBrains";

            var programFolderName = Directory.GetDirectories(path)
                .FirstOrDefault(
                    f => f.Split('\\').Last().StartsWith($"{JetBrainsProducts[_selectedIndex]}")
                );

            path = @$"C:\Users\{username}\AppData\Roaming\JetBrains\{programFolderName}";
            return path;
        }

        private static void DeleteEvaluationKey()
        {
            string path = GetPath() + "\\eval";
            string[] files = Directory.GetFiles(path);

            var dotKeyFile = files.FirstOrDefault(f => f.EndsWith(".key"));

            if (dotKeyFile != null)
            {
                File.Delete(dotKeyFile);
            }
            else
            {
                throw new($"{nameof(DeleteEvaluationKey)} .key file is not present");
            }
        }

        private static void DeleteOtherOptions()
        {
            string path = GetPath() + "\\options\\other.xml";
            File.Delete(path);
        }

        private static void RegDeleteSubKeyTree()
        {
            string subKey = $"Software\\JavaSoft\\Prefs\\jetbrains\\{JetBrainsProducts[_selectedIndex-1].ToLower()}";
            using RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey, true);
            if (regKey != null)
            {
                Registry.CurrentUser.DeleteSubKeyTree(subKey, true);
            }
            else
            {
                throw new($"{nameof(RegDeleteSubKeyTree)}(): regKey is null!");
            }
        }

        private static void PrintProductListToConsole()
        {
            for (int i = 0; i < JetBrainsProducts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. " + JetBrainsProducts[i]);
            }
        }

        private static void EnterIndex()
        {
            Console.WriteLine("Enter needed number: ");

            int index;

            while (!int.TryParse(Console.ReadLine(), out index) ||
                   (index >= JetBrainsProducts.Count || index <= 0)
            )
            {
                Console.WriteLine("Entered number is incorrect");
                Console.WriteLine("Enter CORRECT number!");
            }

            _selectedIndex = index;
        }

        private static void Main(string[] args)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("This app needed admin rights");
                Console.ReadKey();
                return;
            }

            try
            {
                PrintProductListToConsole();
                EnterIndex();
                RegDeleteSubKeyTree();
                DeleteEvaluationKey();
                DeleteOtherOptions();

                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(
                    "Please, write about error to developer\n" +
                    "Telegram - qqq"
                );
            }

            Console.ReadKey();
        }
    }
}