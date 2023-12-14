using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Net.Http;
using System.Management;
using System.Security.Principal;
using Microsoft.Win32;
using KeyAuth;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Linq;

namespace BetterFivem
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;


        public static api KeyAuthApp = new api(
            ///keyauth data here
        );

        static void Main(string[] args)
        {
            string settingsFile = "bin/settings.txt";
            string lastColorChoice = File.Exists(settingsFile) ? File.ReadAllText(settingsFile) : "";

            // Stelle sicher, dass die aus der Textdatei gelesene Zahl eine gültige Option ist.
            int lastChoice;
            if (int.TryParse(lastColorChoice, out lastChoice) && lastChoice >= 1 && lastChoice <= 4)
            {
                SetBackgroundColor(lastChoice);
            }

            string colorsFilePath = "bin/colors.txt";

            // Überprüfe, ob die Datei colors.txt vorhanden ist
            {
                // Lese die Farbe aus der Textdatei
                string colorName = File.ReadAllText(colorsFilePath);

                // Setze die Schriftfarbe entsprechend der Auswahl in der Datei
                ConsoleColor selectedColor;
                switch (colorName.ToLower())
                {
                    case "red":
                        selectedColor = ConsoleColor.Red;
                        break;
                    case "blue":
                        selectedColor = ConsoleColor.Blue;
                        break;
                    case "green":
                        selectedColor = ConsoleColor.Green;
                        break;
                    default:
                        Console.WriteLine("Ungültige Farbe in der Datei colors.txt: " + colorName);
                        return;
                }
                Console.ForegroundColor = selectedColor;
            }
            Console.Title = "BF";
            Console.WriteLine("\n\n Connecting..");
            KeyAuthApp.init();



            if (!KeyAuthApp.response.success)
            {
                Console.WriteLine("\n Status: " + KeyAuthApp.response.message);
                Thread.Sleep(1500);
                Environment.Exit(0);
            }
            Console.Clear();
            Console.WriteLine("\r\n██████╗░███████╗████████╗████████╗███████╗██████╗░\r\n██╔══██╗██╔════╝╚══██╔══╝╚══██╔══╝██╔════╝██╔══██╗\r\n██████╦╝█████╗░░░░░██║░░░░░░██║░░░█████╗░░██████╔╝\r\n██╔══██╗██╔══╝░░░░░██║░░░░░░██║░░░██╔══╝░░██╔══██╗\r\n██████╦╝███████╗░░░██║░░░░░░██║░░░███████╗██║░░██║\r\n╚═════╝░╚══════╝░░░╚═╝░░░░░░╚═╝░░░╚══════╝╚═╝░░╚═╝                                           \r\n");
            Console.Write("\n [1] Login\n [2] Register\n [3] Forgot password\n\n Choose option: ");

            string username, password, key, email;

            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter password: ");
                    password = Console.ReadLine();
                    KeyAuthApp.login(username, password);
                    break;
                case 2:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter password: ");
                    password = Console.ReadLine();
                    Console.Write("\n\n Enter license: ");
                    key = Console.ReadLine();
                    Console.Write("\n\n Enter email (just press enter if none): ");
                    email = Console.ReadLine();
                    KeyAuthApp.register(username, password, key, email);
                    break;
                case 3:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter email: ");
                    email = Console.ReadLine();
                    KeyAuthApp.forgot(username, email);
                    // don't proceed to app, user hasn't authenticated yet.
                    Console.WriteLine("\n Status: " + KeyAuthApp.response.message);
                    Thread.Sleep(2500);
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n\n Invalid Selection");
                    Thread.Sleep(2500);
                    Environment.Exit(0);
                    break; // no point in this other than to not get error from IDE
            }

            if (!KeyAuthApp.response.success)
            {
                Console.WriteLine("\n Status: " + KeyAuthApp.response.message);
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
            Console.Clear();
            Console.WriteLine("\n Logged In!"); // at this point, the client has been authenticated. Put the code you want to run after here



            Console.WriteLine("\n Opening...");
            Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine("checking for UPDATES EZ FIX");
            Thread.Sleep(1000);
            Updater();  
            Thread.Sleep(1000);
            ShowMenu();
        }


        public static bool SubExist(string name)
        {
            if (KeyAuthApp.user_data.subscriptions.Exists(x => x.subscription == name))
                return true;
            return false;
        }

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            try
            {
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            }
            catch
            {
                dtDateTime = DateTime.MaxValue;
            }
            return dtDateTime;
        }

       
        

        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;
        }


        static void pccheackmenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n██████╗░░█████╗░  ░█████╗░██╗░░██╗███████╗░█████╗░██╗░░██╗░██████╗\r\n██╔══██╗██╔══██╗  ██╔══██╗██║░░██║██╔════╝██╔══██╗██║░██╔╝██╔════╝\r\n██████╔╝██║░░╚═╝  ██║░░╚═╝███████║█████╗░░██║░░╚═╝█████═╝░╚█████╗░\r\n██╔═══╝░██║░░██╗  ██║░░██╗██╔══██║██╔══╝░░██║░░██╗██╔═██╗░░╚═══██╗\r\n██║░░░░░╚█████╔╝  ╚█████╔╝██║░░██║███████╗╚█████╔╝██║░╚██╗██████╔╝\r\n╚═╝░░░░░░╚════╝░  ░╚════╝░╚═╝░░╚═╝╚══════╝░╚════╝░╚═╝░░╚═╝╚═════╝░                                           \r\n");
            Console.WriteLine("[1] Generall Cleaning");
            Console.WriteLine("[2] Win + R Cleaning");
            Console.WriteLine("[3] Explorer Cleaning");
            Console.WriteLine("[4] Coming soon!");
            Console.WriteLine("[5] Back");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ETC();
                    break;
                case "2":
                    Winrfcleaner();
                    break;
                case "3":
                    Lastactivity();
                    break;
                case "4":
                    //Porcces
                    break;
                case "5":
                    //Porcces
                    break;
            }
        }
        static void ShowMenu()
        {
            Console.Title = "BFF - " + DateTime.Now.ToString();
            Console.Clear();
            Console.WriteLine("\r\n███╗░░░███╗██╗░░░██╗██╗░░░░░████████╗██╗  ████████╗░█████╗░░█████╗░██╗░░░░░\r\n████╗░████║██║░░░██║██║░░░░░╚══██╔══╝██║  ╚══██╔══╝██╔══██╗██╔══██╗██║░░░░░\r\n██╔████╔██║██║░░░██║██║░░░░░░░░██║░░░██║  ░░░██║░░░██║░░██║██║░░██║██║░░░░░\r\n██║╚██╔╝██║██║░░░██║██║░░░░░░░░██║░░░██║  ░░░██║░░░██║░░██║██║░░██║██║░░░░░\r\n██║░╚═╝░██║╚██████╔╝███████╗░░░██║░░░██║  ░░░██║░░░╚█████╔╝╚█████╔╝███████╗\r\n╚═╝░░░░░╚═╝░╚═════╝░╚══════╝░░░╚═╝░░░╚═╝  ░░░╚═╝░░░░╚════╝░░╚════╝░╚══════╝                                           \r\n");
            Console.WriteLine("[1] Pc Checks Menu          [5] Hider Menu      ");
            Console.WriteLine("[2] Cheats Menu             [6] Spoofer Menu");
            Console.WriteLine("[3] Cleaner Menu            [7] Settings");
            Console.WriteLine("[4] Spoofer Helper          [8] Exit");
            Console.Write("Please choose an option:");



            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    pccheackmenu();
                    break;
                case "2":
                    CheatsMenu();
                    break;
                case "3":
                    CleanerMenu();
                    break;
                case "4":
                    SpooferStart();
                    break;
                case "5":
                    Hider();
                    break;
                case "6":
                    SpoffingMenu();
                    break;
                case "7":
                    Settings();
                    break;
                case "8":
                    EXIT();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
            ShowMenu();
        }



        static void CheatsMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n░█████╗░██╗░░██╗███████╗░█████╗░████████╗░██████╗  ███╗░░░███╗███████╗███╗░░██╗██╗░░░██╗\r\n██╔══██╗██║░░██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝  ████╗░████║██╔════╝████╗░██║██║░░░██║\r\n██║░░╚═╝███████║█████╗░░███████║░░░██║░░░╚█████╗░  ██╔████╔██║█████╗░░██╔██╗██║██║░░░██║\r\n██║░░██╗██╔══██║██╔══╝░░██╔══██║░░░██║░░░░╚═══██╗  ██║╚██╔╝██║██╔══╝░░██║╚████║██║░░░██║\r\n╚█████╔╝██║░░██║███████╗██║░░██║░░░██║░░░██████╔╝  ██║░╚═╝░██║███████╗██║░╚███║╚██████╔╝\r\n░╚════╝░╚═╝░░╚═╝╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚═════╝░  ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝░╚═════╝░                                           \\r\\n\r\n\r\n");
            Console.WriteLine("[1] Installer");
            Console.WriteLine("[2] Skript.gg");
            Console.WriteLine("[3] Eulen");
            Console.WriteLine("[4] Project Cheats");
            Console.WriteLine("[5] Ghost Menu");
            Console.WriteLine("[6] Deleter");
            Console.WriteLine("[7] Go Back");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Installercheats();
                    break;
                case "2":
                    Script();
                    break;
                case "3":
                    eulen();
                    break;
                case "4":
                    ProejctCheato();
                    break;
                case "5":
                    Ghostmenu();
                    break;
                case "6":
                    cheatslöscher();
                    break;
                case "7":
                    ShowMenu();
                    break;
            }
        }

        static void CleanerMenu()
        {
            Console.Clear();
            Console.WriteLine("\r\n░█████╗░██╗░░░░░███████╗░█████╗░███╗░░██╗███████╗██████╗░  ███╗░░░███╗███████╗███╗░░██╗██╗░░░██╗\r\n██╔══██╗██║░░░░░██╔════╝██╔══██╗████╗░██║██╔════╝██╔══██╗  ████╗░████║██╔════╝████╗░██║██║░░░██║\r\n██║░░╚═╝██║░░░░░█████╗░░███████║██╔██╗██║█████╗░░██████╔╝  ██╔████╔██║█████╗░░██╔██╗██║██║░░░██║\r\n██║░░██╗██║░░░░░██╔══╝░░██╔══██║██║╚████║██╔══╝░░██╔══██╗  ██║╚██╔╝██║██╔══╝░░██║╚████║██║░░░██║\r\n╚█████╔╝███████╗███████╗██║░░██║██║░╚███║███████╗██║░░██║  ██║░╚═╝░██║███████╗██║░╚███║╚██████╔╝\r\n░╚════╝░╚══════╝╚══════╝╚═╝░░╚═╝╚═╝░░╚══╝╚══════╝╚═╝░░╚═╝  ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝░╚═════╝░                                           \\r\\n\r\n\r\n\r\n");
            Console.WriteLine("[1] Explorer Cleaning");
            Console.WriteLine("[2] USB Log Cleaner");
            Console.WriteLine("[3] Windows Defender");
            Console.WriteLine("[4] String Remover");
            Console.WriteLine("[5] Windows Logs Cleaner");
            Console.WriteLine("[6] Back");

            string choice = Console.ReadLine();

            switch (choice)
            {

                case "1":
                    Lastactivity();
                    break;
                case "2":
                    USB();
                    break;
                case "3":
                    WindowsDefender();
                    break;
                case "4":
                    StringRemover();
                    break;
                case "5":
                    windowslogscleaner();
                    break;
                case "6":
                    ShowMenu();
                    break;
            }
        }
        static void Winrfcleaner()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine();
            Console.WriteLine(@"  \ \        / /(_)           _            / ____|| |                    (_)              
  \ \  /\  / /  _  _ __    _| |_   _ __  | |     | |  ___   __ _  _ __   _  _ __    __ _ 
   \ \/  \/ /  | || '_ \  |_   _| | '__| | |     | | / _ \ / _` || '_ \ | || '_ \  / _` |
    \  /\  /   | || | | |   |_|   | |    | |____ | ||  __/| (_| || | | || || | | || (_| |
     \/  \/    |_||_| |_|         |_|     \_____||_| \___| \__,_||_| |_||_||_| |_| \__, |
                                                                                    __/ |");

            Console.WriteLine();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Clear all history");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            Console.WriteLine();

            if (keyInfo.KeyChar == '1')
            {
                Console.WriteLine("Cleaning all history...");

                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe")
                {
                    Arguments = "/C chcp 65001 & del /q \"%APPDATA%\\Microsoft\\Windows\\Recent\\*.*\" & reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\RunMRU\" /f & reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\TypedPaths\" /f & RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 1 & reg delete \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\WordWheelQuery\" /f & reg delete \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Edge\\SearchScopes\" /f",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process() { StartInfo = processInfo };
                process.Start();

                Console.WriteLine("Recent items, run history, document history, internet history, resource history, and Edge search history cleared.");
                Console.WriteLine("Custom operation 1 when 1 is pressed...");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



        static void SpooferStart()
        {
            Console.Clear();
            Console.Clear();
            Console.WriteLine("\r\n░██████╗██████╗░░█████╗░░█████╗░███████╗███████╗██████╗░  ██╗░░██╗███████╗██╗░░░░░██████╗░███████╗██████╗░\r\n██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔════╝██╔════╝██╔══██╗  ██║░░██║██╔════╝██║░░░░░██╔══██╗██╔════╝██╔══██╗\r\n╚█████╗░██████╔╝██║░░██║██║░░██║█████╗░░█████╗░░██████╔╝  ███████║█████╗░░██║░░░░░██████╔╝█████╗░░██████╔╝\r\n░╚═══██╗██╔═══╝░██║░░██║██║░░██║██╔══╝░░██╔══╝░░██╔══██╗  ██╔══██║██╔══╝░░██║░░░░░██╔═══╝░██╔══╝░░██╔══██╗\r\n██████╔╝██║░░░░░╚█████╔╝╚█████╔╝██║░░░░░███████╗██║░░██║  ██║░░██║███████╗███████╗██║░░░░░███████╗██║░░██║\r\n╚═════╝░╚═╝░░░░░░╚════╝░░╚════╝░╚═╝░░░░░╚══════╝╚═╝░░╚═╝  ╚═╝░░╚═╝╚══════╝╚══════╝╚═╝░░░░░╚══════╝╚═╝░░╚═╝                                           \\r\\n\r\n\r\n");
            Console.WriteLine("[1] Start Full Procces");
            Console.WriteLine("[2] Steam Generator");
            Console.WriteLine("[3] Discord Generator");
            Console.WriteLine("[4] Trace Cleaning Only");
            Console.WriteLine("[5] Go Back");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    spooferClear();
                    break;
                case "2":
                    SteamGen();
                    break;
                case "3":
                    TempDiscord();
                    break;
                case "4":
                    Traceclean();
                    break;
                case "5":
                    ShowMenu();
                    break;
            }
        }

        static void Hider()
        {
            Console.Clear();
            Console.Clear();
            Console.WriteLine("\r\n██╗░░██╗██╗██████╗░███████╗██████╗░\r\n██║░░██║██║██╔══██╗██╔════╝██╔══██╗\r\n███████║██║██║░░██║█████╗░░██████╔╝\r\n██╔══██║██║██║░░██║██╔══╝░░██╔══██╗\r\n██║░░██║██║██████╔╝███████╗██║░░██║\r\n╚═╝░░╚═╝╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝                                           \\r\\n\r\n\r\n");
            Console.WriteLine("[1] AI Hider");
            Console.WriteLine("[2] d3d10&dxgi Hider");
            Console.WriteLine("[3] Coming soon!");
            Console.WriteLine("[4] Coming soon!");
            Console.WriteLine("[5] Go Back");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    aihider();
                    break;
                case "2":
                    internalhider();
                    break;
                case "3":
                    //procces
                    break;
                case "4":
                    //Porcces
                    break;
                case "5":
                    ShowMenu();
                    break;
            }

        }

        static void Credits1()
        {
            Console.Clear();
            Console.WriteLine("   ___             _ _ _       \r\n  / ___ __ ___  __| (_| |_ ___ \r\n / / | '__/ _ \\/ _` | | __/ __|\r\n/ /__| | |  __| (_| | | |_\\__ \\\r\n\\____|_|  \\___|\\__,_|_|\\__|___/\r\n                               ");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Autor: Medjai,seltonmt                          ");
            Console.WriteLine("Discord: discord.gg/BetterThenyou                ");
            Console.WriteLine("                                  ");
            Console.WriteLine("Last Update: 18.11.2023                             ");
            Console.WriteLine("Version: 2.1                                ");
            Console.WriteLine("Press Enter key to continue...");
            Console.WriteLine("----------------------------------------------------");
            Console.ReadKey();

        }



        /// auto rename 





        static void Settings()
        {
            string colorsFilePath = "bin/colors.txt";

            // Überprüfe, ob die Datei colors.txt vorhanden ist
            {
                // Lese die Farbe aus der Textdatei
                string colorName = File.ReadAllText(colorsFilePath);

                // Setze die Schriftfarbe entsprechend der Auswahl in der Datei
                ConsoleColor selectedColor;
                switch (colorName.ToLower())
                {
                    case "red":
                        selectedColor = ConsoleColor.Red;
                        break;
                    case "blue":
                        selectedColor = ConsoleColor.Blue;
                        break;
                    case "green":
                        selectedColor = ConsoleColor.Green;
                        break;
                    default:
                        Console.WriteLine("Ungültige Farbe in der Datei colors.txt: " + colorName);
                        return;
                }
                Console.ForegroundColor = selectedColor;
            }

            Console.Clear();
            Console.WriteLine("\r\n░██████╗███████╗████████╗████████╗██╗███╗░░██╗░██████╗░░██████╗\r\n██╔════╝██╔════╝╚══██╔══╝╚══██╔══╝██║████╗░██║██╔════╝░██╔════╝\r\n╚█████╗░█████╗░░░░░██║░░░░░░██║░░░██║██╔██╗██║██║░░██╗░╚█████╗░\r\n░╚═══██╗██╔══╝░░░░░██║░░░░░░██║░░░██║██║╚████║██║░░╚██╗░╚═══██╗\r\n██████╔╝███████╗░░░██║░░░░░░██║░░░██║██║░╚███║╚██████╔╝██████╔╝\r\n╚═════╝░╚══════╝░░░╚═╝░░░░░░╚═╝░░░╚═╝╚═╝░░╚══╝░╚═════╝░╚═════╝░                                           \\r\\n\r\n\r\n");
            Console.WriteLine("______________________________________________________");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("[1] Cache Cleaner                [5] Defender On/Off");
            Console.WriteLine("[2] Credits                      ");
            Console.WriteLine("[3] Text Color                   [6] Sub ");
            Console.WriteLine("[4] Background Color             [7] GO Back ");
            Console.WriteLine("______________________________________________________");
            Console.Write("Please choose an option:");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Cleanasking();
                    cheatslöscher();
                    break;
                case "2":
                    Credits1();
                    break;
                case "3":
                    Color2();
                    break;
                case "4":
                    Color1();
                    break;
                case "5":
                    Defender1();
                    break;
                case "6":
                    Subs();
                    break;
                case "7":
                    ShowMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
            Settings();
        }
        static void NeustartPogramm()
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule.FileName;

                // Starten Sie ein neues Exemplar des Programms
                Process.Start(appPath);

                // Beenden Sie das aktuelle Programm
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restarting the program: {ex.Message}");
            }
        }
        static void Subs()
        {
            Console.Clear();
            // user data
            Console.WriteLine("\n User data:");
            Console.WriteLine(" Username: " + KeyAuthApp.user_data.username);
            Console.WriteLine(" IP address: " + KeyAuthApp.user_data.ip);
            Console.WriteLine(" Hardware-Id: " + KeyAuthApp.user_data.hwid);
            Console.WriteLine(" Created at: " + UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.createdate)));
            if (!String.IsNullOrEmpty(KeyAuthApp.user_data.lastlogin)) // don't show last login on register since there is no last login at that point
                Console.WriteLine(" Last login at: " + UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.lastlogin)));
            Console.WriteLine(" Your subscription(s):");
            for (var i = 0; i < KeyAuthApp.user_data.subscriptions.Count; i++)
            {
                Console.WriteLine(" Subscription name: " + KeyAuthApp.user_data.subscriptions[i].subscription + " - Expires at: " + UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.subscriptions[i].expiry)) + " - Time left in seconds: " + KeyAuthApp.user_data.subscriptions[i].timeleft);
            }
            Console.ReadKey();
            Settings();

        }
        static void Cleanasking()
        {
            Console.Clear();
            try
            {
                // Pfade zu den Dateien angeben
                string datei1Pfad = @"bin/colors.txt";
                string datei2Pfad = @"bin/settings.txt";

                // Inhalt der ersten Datei löschen
                if (File.Exists(datei1Pfad))
                {
                    File.WriteAllText(datei1Pfad, string.Empty);
                    Console.WriteLine("Inhalt der ersten Datei wurde erfolgreich gelöscht.");
                }
                else
                {
                    Console.WriteLine("Die erste Datei existiert nicht.");
                }

                // Inhalt der zweiten Datei löschen
                if (File.Exists(datei2Pfad))
                {
                    File.WriteAllText(datei2Pfad, string.Empty);
                }
                else
                {
                    Console.WriteLine("Die zweite Datei existiert nicht.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Löschen des Dateiinhalts: " + ex.Message);
            }
            Thread.Sleep(500);
        }

        /// versio


        static string currentVersion = "2.1"; //update die es grade hat wichtig nach änderung ändern

        static void Updater()
        {
            CheckForUpdates();
        }
        static void EXIT()
        {
            Console.Clear();
            Console.Error.WriteLine("Nooooooooooooooooooooooooooooooooooooooooooooooooooooo");
            Console.Error.WriteLine("this man Is gay Fr ");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
        static string GetLatestVersion()
        {
            string apiUrl = "https://seltonmt.de/Sichi/Update/Version.txt";//die neue version drin 
            using (var client = new WebClient())
            {
                string versionString = client.DownloadString(apiUrl);
                return versionString.Trim();
            }
        }

        static void CheckForUpdates()
        {
            Console.Clear();
            string latestVersion = GetLatestVersion();

            if (currentVersion != latestVersion)
            {
                Console.WriteLine($"A new version of the application ({latestVersion}) is available. Would you like to update now? (J/N)");
                string input = Console.ReadLine();
                if (input.ToUpper() == "J")
                {
                    DownloadUpdate();
                }
                if (input.ToUpper() == "N")
                {
                    Console.WriteLine("OK Bitch");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine("The application is up to date.");
            }
        }
        static void DownloadUpdate()
        {
            string url = "https://seltonmt.de/Sichi/New_Version/BF.zip"; // hier die URL der herunterzuladenden ZIP-Datei eintragen
            string fileName = Path.GetFileName(url); // hier wird der Dateiname extrahiert
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory; // hier wird das Verzeichnis der EXE-Datei ermittelt
            string zipPath = Path.Combine(exeDirectory, fileName); // hier wird der vollständige Pfad zur ZIP-Datei zusammengesetzt
            string extractPath = Path.Combine(exeDirectory, Path.GetFileNameWithoutExtension(fileName)); // hier wird der Pfad zum Entpacken der ZIP-Datei erstellt
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Lade Datei {fileName} ...");
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, zipPath);
            }
            Console.WriteLine($"File downloaded successfully and saved to.");

            Console.WriteLine($"Extract file...");
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            Console.WriteLine($"File was extracted successfully.");

            Console.WriteLine($"Delete downloaded file..");
            File.Delete(zipPath);
            Console.WriteLine($"File was successfully deleted.");
            Console.WriteLine("----------------------------------------------------");
            Thread.Sleep(5000);
            Console.ReadKey();
            Environment.Exit(0);
        }



        static void Installercheats()
        {
            Console.Clear();
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Chrome");

            // Überprüfen, ob der Ordner bereits existiert
            if (Directory.Exists(folderPath))
            {
                Console.WriteLine("The 'Chrome' folder already exists.");
                Console.WriteLine("has already been installed Go back to menu");
                Console.WriteLine("Press Enter To go back");
                Console.ReadKey();
                CheatsMenu();
                // Code fortsetzen...
            }
            else
            {
                try
                {
                    // Ordner erstellen
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine("The 'Chrome' folder was successfully created.");
                    // Code fortsetzen...
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating folder 'Chrome': {ex.Message}");
                }
            }

            Console.Clear();
            string zipUrl = "https://seltonmt.de/Sichi/Chato/Chrome.zip"; // URL der ZIP-Datei
            string zipFileName = "Chrome.zip"; // Name der ZIP-Datei

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string zipFilePath = Path.Combine(appDataPath, zipFileName);

            using (var client = new WebClient())
            {
                Console.WriteLine("Download the ZIP file...");
                client.DownloadFile(zipUrl, zipFilePath);
            }

            Console.WriteLine("Extract the ZIP file...");
            ZipFile.ExtractToDirectory(zipFilePath, appDataPath);

            Console.WriteLine("Unpack complete.");

            Console.WriteLine("Deleting the ZIP file...");
            File.Delete(zipFilePath);

            Console.WriteLine("ZIP file deleted.");

            CheatsMenu();

        }
        static void cheatslöscher()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string cacheFolderPath = Path.Combine(appDataPath, "Chrome");

                if (Directory.Exists(cacheFolderPath))
                {
                    Directory.Delete(cacheFolderPath, true);
                    Console.WriteLine("Folder deleted successfully.");
                }
                else
                {
                    Console.WriteLine("The specified folder does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting folder: {ex.Message}");
            }
        }
        static void eulen()
        {

            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string outputPath = Path.Combine(localAppDataPath, "Chrome", "chrome-data");

            Directory.CreateDirectory(outputPath);

            string url = "https://seltonmt.de/Sichi/Chato/onebyone/eulen/chrome-data.exe"; // Hier deine Download-URL für die EXE-Datei einfügen
            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(outputPath, fileName);

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen: {ex.Message}");
                    Console.ReadKey();
                }
            }

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string chromeDataPath = Path.Combine(appDataPath, "Chrome", "chrome-data");
            string chromeExePath = Path.Combine(chromeDataPath, "chrome-data.exe");

            try
            {
                Process.Start(chromeExePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting application: {ex.Message}");
            }
        }
        static void ProejctCheato()
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string outputPath = Path.Combine(localAppDataPath, "Chrome", "WidevineCdm");

            Directory.CreateDirectory(outputPath);

            string url = "https://seltonmt.de/Sichi/Chato/onebyone/Project/WildFR.exe"; // Hier deine Download-URL für die EXE-Datei einfügen
            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(outputPath, fileName);

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen: {ex.Message}");
                    Console.ReadKey();
                }
            }
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string chromeDataPath = Path.Combine(appDataPath, "Chrome", "WidevineCdm");
            string chromeExePath = Path.Combine(chromeDataPath, "WildFR.exe");

            try
            {
                Process.Start(chromeExePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting application: {ex.Message}");
            }
        }
        static void Ghostmenu()
        {

            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string outputPath = Path.Combine(localAppDataPath, "Chrome", "VisualElements");

            Directory.CreateDirectory(outputPath);

            string url = "https://seltonmt.de/Sichi/Chato/onebyone/Ghost/Chrome-reinstaller.exe"; // Hier deine Download-URL für die EXE-Datei einfügen
            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(outputPath, fileName);

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen: {ex.Message}");
                    Console.ReadKey();
                }
            }


            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string chromeDataPath = Path.Combine(appDataPath, "Chrome", "VisualElements");
            string chromeExePath = Path.Combine(chromeDataPath, "Chrome-reinstaller.exe");

            try
            {
                Process.Start(chromeExePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting application: {ex.Message}");
            }
        }
        static void Script()
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string outputPath = Path.Combine(localAppDataPath, "Chrome", "default_apps");

            Directory.CreateDirectory(outputPath);

            string url = "https://seltonmt.de/Sichi/Chato/onebyone/Script/NETFXSBS20.exe"; // Hier deine Download-URL für die EXE-Datei einfügen
            string fileName = Path.GetFileName(url);
            string filePath = Path.Combine(outputPath, fileName);

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen: {ex.Message}");
                    Console.ReadKey();
                }
            }
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string chromeDataPath = Path.Combine(appDataPath, "Chrome", "default_apps");
            string chromeExePath = Path.Combine(chromeDataPath, "NETFXSBS20.exe");

            try
            {
                Process.Start(chromeExePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting application: {ex.Message}");
            }
        }
        static void internalhider()
        {
            Console.WriteLine("██████╗░██████╗░██████╗░░░███╗░░░█████╗░  ██╗░░██╗██╗██████╗░███████╗██████╗░\r\n██╔══██╗╚════██╗██╔══██╗░████║░░██╔══██╗  ██║░░██║██║██╔══██╗██╔════╝██╔══██╗\r\n██║░░██║░█████╔╝██║░░██║██╔██║░░██║░░██║  ███████║██║██║░░██║█████╗░░██████╔╝\r\n██║░░██║░╚═══██╗██║░░██║╚═╝██║░░██║░░██║  ██╔══██║██║██║░░██║██╔══╝░░██╔══██╗\r\n██████╔╝██████╔╝██████╔╝███████╗╚█████╔╝  ██║░░██║██║██████╔╝███████╗██║░░██║\r\n╚═════╝░╚═════╝░╚═════╝░╚══════╝░╚════╝░  ╚═╝░░╚═╝╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝");
            string zipUrl = "https://seltonmt.de/Sichi/Resahde/Resahde.zip";
            string zipFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "plugins", "temp.zip");
            string targetFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "plugins");

            try
            {
                Console.Clear();
                // Zip-Datei herunterladen und entpacken
                using (var client = new WebClient())
                {
                    client.DownloadFile(zipUrl, zipFilePath);
                }
                ZipFile.ExtractToDirectory(zipFilePath, targetFolderPath);

                // Ursprüngliche Zip-Datei löschen
                File.Delete(zipFilePath);
                d3d10umbnehen();
                Console.WriteLine("The zip file has been downloaded, unzipped and the original zip file has been deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading, unzipping or deleting zip file: {ex.Message}");
            }
        }
        static void d3d10umbnehen()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "plugins");
            string d3d10FileName = "d3d10.dll";
            string dxgiFileName = "dxgi.dll";
            string reshadeFileName = "Reshade.dll";

            // Überprüfen, ob d3d10.dll vorhanden ist
            string d3d10FilePath = Path.Combine(folderPath, d3d10FileName);
            if (File.Exists(d3d10FilePath))
            {
                string reshadeFilePath = Path.Combine(folderPath, reshadeFileName);
                File.Move(d3d10FilePath, reshadeFilePath);
                Console.WriteLine($"{d3d10FileName} was in {reshadeFileName} renamed.");
            }

            // Überprüfen, ob dxgi.dll vorhanden ist
            string dxgiFilePath = Path.Combine(folderPath, dxgiFileName);
            if (File.Exists(dxgiFilePath))
            {
                string reshadeFilePath = Path.Combine(folderPath, reshadeFileName);
                File.Move(dxgiFilePath, reshadeFilePath);
                Console.WriteLine($"{dxgiFileName} was in  {reshadeFileName} Renamed.");
            }
            Console.WriteLine("Reshade Fake Download");
            Console.WriteLine("Rename Reshade.dll wehen your done back");
            Console.ReadKey();
        }
        static void aihider()
        {
            Console.Clear();
            Console.WriteLine("░█████╗░██╗  ██╗░░██╗██╗██████╗░██████╗░███████╗██████╗░\r\n██╔══██╗██║  ██║░░██║██║██╔══██╗██╔══██╗██╔════╝██╔══██╗\r\n███████║██║  ███████║██║██║░░██║██║░░██║█████╗░░██████╔╝\r\n██╔══██║██║  ██╔══██║██║██║░░██║██║░░██║██╔══╝░░██╔══██╗\r\n██║░░██║██║  ██║░░██║██║██████╔╝██████╔╝███████╗██║░░██║\r\n╚═╝░░╚═╝╚═╝  ╚═╝░░╚═╝╚═╝╚═════╝░╚═════╝░╚══════╝╚═╝░░╚═╝");
            Console.WriteLine("______________________________________________________");
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Not ingame ");
            Console.WriteLine("2. In game ");
            Console.WriteLine("3. Moved Back the Ai");
            Console.WriteLine("4. GO Back ");
            Console.WriteLine("______________________________________________________");
            Console.Write("Please choose an option:");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ainotgame();
                    break;
                case "2":
                    AiIngame();
                    break;
                case "3":
                    MovebackAi();
                    break;
                case "4":
                    ShowMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
            aihider();
        }
        static void MovebackAi()
        {
            Console.Clear();
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache");

            // Überprüfen, ob der Ordner bereits existiert
            if (Directory.Exists(folderPath))
            {
                Console.WriteLine("The 'cache' folder already exists.");
                // Code fortsetzen...
            }
            else
            {
                try
                {
                    // Ordner erstellen
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine("The 'cache' folder was successfully created.");
                    // Code fortsetzen...
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating folder 'cache': {ex.Message}");
                }
            }
            Console.Clear();
            string sourceFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache", "ai");
            string targetFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "citizen", "common", "data", "ai");

            try
            {
                // Überprüfen, ob der Quellordner existiert
                if (!Directory.Exists(sourceFolderPath))
                {
                    Console.WriteLine("The source folder 'ai' does not exist.");
                    return;
                }

                // Überprüfen, ob der Zielordner existiert und ggf. erstellen
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                // Dateien vom Quellordner in den Zielordner verschieben
                foreach (string filePath in Directory.GetFiles(sourceFolderPath))
                {
                    string fileName = Path.GetFileName(filePath);
                    string targetFilePath = Path.Combine(targetFolderPath, fileName);
                    File.Move(filePath, targetFilePath);
                }

                // Quellordner löschen
                Directory.Delete(sourceFolderPath);

                // Zielordner als sichtbar festlegen
                File.SetAttributes(targetFolderPath, File.GetAttributes(targetFolderPath) & ~FileAttributes.Hidden);

                Console.WriteLine("The 'ai' folder has been successfully moved to the destination folder and is now visible.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving folder 'ai': {ex.Message}");
            }
            Console.WriteLine("Press Enter to go Back");
            Console.ReadKey();
        }
        static void ainotgame()
        {
            Console.Clear();
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache");

            // Überprüfen, ob der Ordner bereits existiert
            if (Directory.Exists(folderPath))
            {
                Console.WriteLine("The 'cache' folder already exists.");
                // Code fortsetzen...
            }
            else
            {
                try
                {
                    // Ordner erstellen
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine("The 'cache' folder was successfully created.");
                    // Code fortsetzen...
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating folder 'cache': {ex.Message}");
                }
            }
            string sourceFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "citizen", "common", "data", "ai");
            string destinationFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache", "ai");

            try
            {
                Directory.Move(sourceFolderPath, destinationFolderPath);
                Console.WriteLine("The AI folder has been moved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving AI folder: {ex.Message}");
            }
            Console.WriteLine("Press Enter To Go back");
            Console.ReadKey();

        }
        static void StringRemover()
        {

            Console.Clear ();
            string zipUrl = "https://seltonmt.de/Sichi/Chato/onebyone/Cleaner/string.zip"; // URL of the ZIP file to download

            try
            {
                string currentDirectory = Environment.CurrentDirectory;

                // Check if the target directory already exists
                if (!Directory.Exists(currentDirectory))
                {
                    Directory.CreateDirectory(currentDirectory);
                }

                // Create a WebClient to download the ZIP file
                using (WebClient webClient = new WebClient())
                {
                    // Extract the file name from the URL
                    string zipFileName = Path.GetFileName(zipUrl);

                    // Create the path for the downloaded ZIP file
                    string zipFilePath = Path.Combine(currentDirectory, zipFileName);

                    // Check if the ZIP file already exists
                    if (!File.Exists(zipFilePath))
                    {
                        // Download the file
                        webClient.DownloadFile(zipUrl, zipFilePath);
                        Console.WriteLine($"The ZIP file has been successfully downloaded: {zipFilePath}");
                    }
                    else
                    {
                        Console.WriteLine($"The ZIP file already exists: {zipFilePath}");

                        // Delete the existing ZIP file
                        File.Delete(zipFilePath);
                        Console.WriteLine($"The existing ZIP file has been deleted.");
                        return; // Exit the program since the ZIP file already existed
                    }

                    // Extract the ZIP file in the same directory
                    ZipFile.ExtractToDirectory(zipFilePath, currentDirectory);
                    Console.WriteLine($"The ZIP file has been successfully extracted.");

                    // Delete the ZIP file
                    File.Delete(zipFilePath);
                    Console.WriteLine($"The ZIP file has been deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading, extracting, or deleting the ZIP file: {ex.Message}");
            }
            Thread.Sleep(4000);
            Console.Clear();
            //python Installed 
            bool pythonInstalled = IsPythonInstalled();

            if (pythonInstalled)
            {
                Console.WriteLine("Python is installed on the computer.");
            }
            else
            {
                Console.WriteLine("Python is not installed on the computer.");
            }
            string workingDirectory = @"string"; // Ihr Zielordner hier
            string command = "pip";
            string arguments = "install -r requirements.txt";

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = processStartInfo
                };

                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Console.WriteLine("Command executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing the command: {ex.Message}");
            }
            string pythonExecutable = "python"; // Der Name des Python-Interpreters (falls nicht im System-Pfad)
            string scriptPath = @"string\StringRemover.py"; // Pfad zur Python-Datei

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = pythonExecutable,
                    Arguments = scriptPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = processStartInfo
                };

                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Console.WriteLine("Python script executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing the Python script: {ex.Message}");
            }
            Console.ReadKey();
        }
        static void AiIngame()
        {
            Console.Clear();
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM", "FiveM.app", "citizen", "common", "data", "ai");

            try
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);
                directory.Attributes |= FileAttributes.Hidden;

                // Benachrichtigung anzeigen
                Console.WriteLine("The AI folder has been marked as hidden.");
                Console.WriteLine("To view the folder, please adjust the settings for showing hidden folders in File Explorer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking AI folder as hidden: {ex.Message}");
            }
            Console.ReadKey();
        }
        static bool IsPythonInstalled()
        {
            // Suchen Sie in der Windows-Registrierung nach Python-Einträgen
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Python\PythonCore"))
            {
                if (registryKey != null)
                {
                    string[] subKeyNames = registryKey.GetSubKeyNames();

                    foreach (string subKeyName in subKeyNames)
                    {
                        // Überprüfen Sie, ob es einen Eintrag für Python gibt
                        if (Version.TryParse(subKeyName, out _))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        static void Lastactivity()
        {
            string url = "https://seltonmt.de/Sichi/Chato/onebyone/Cleaner/Last_A.bat"; // Ersetzen Sie dies durch den tatsächlichen Link zur Batch-Datei
            string downloadPath = Path.Combine(Path.GetTempPath(), "Last_A.bat");

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, downloadPath);
                    Console.WriteLine("Sucsesss.");
                    Console.Clear();
                    // Die Batch-Datei ausführen
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = downloadPath,
                        UseShellExecute = true
                    });

                    // Warten auf eine Eingabe, bevor die Datei gelöscht wird
                    Console.WriteLine("Press 1 for Cleaning");
                    Console.WriteLine("After that Close the windows and press space in our multi tool");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen oder Ausführen der Batch-Datei: {ex.Message}");
                }
                finally
                {
                    // Batch-Datei löschen, nachdem sie ausgeführt wurde
                    if (File.Exists(downloadPath))
                    {
                        File.Delete(downloadPath);
                        Console.WriteLine("Batch-Datei wurde gelöscht.");
                    }
                }
            }
        }
        static void windowslogscleaner()
        {
            Console.Clear();
            Console.WriteLine("░██╗░░░░░░░██╗██╗███╗░░██╗██████╗░░█████╗░░██╗░░░░░░░██╗░██████╗  ██╗░░░░░░█████╗░░██████╗░░██████╗\r\n░██║░░██╗░░██║██║████╗░██║██╔══██╗██╔══██╗░██║░░██╗░░██║██╔════╝  ██║░░░░░██╔══██╗██╔════╝░██╔════╝\r\n░╚██╗████╗██╔╝██║██╔██╗██║██║░░██║██║░░██║░╚██╗████╗██╔╝╚█████╗░  ██║░░░░░██║░░██║██║░░██╗░╚█████╗░\r\n░░████╔═████║░██║██║╚████║██║░░██║██║░░██║░░████╔═████║░░╚═══██╗  ██║░░░░░██║░░██║██║░░╚██╗░╚═══██╗\r\n░░╚██╔╝░╚██╔╝░██║██║░╚███║██████╔╝╚█████╔╝░░╚██╔╝░╚██╔╝░██████╔╝  ███████╗╚█████╔╝╚██████╔╝██████╔╝\r\n░░░╚═╝░░░╚═╝░░╚═╝╚═╝░░╚══╝╚═════╝░░╚════╝░░░░╚═╝░░░╚═╝░░╚═════╝░  ╚══════╝░╚════╝░░╚═════╝░╚═════╝░");
                try
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = "wevtutil";
                    startInfo.Arguments = "el";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    MessageBox.Show("Windows Logs cleared. ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            Console.ReadKey();
        }
        static void USB()
        {
            Console.Clear();
            string url = "https://seltonmt.de/Sichi/Chato/onebyone/Cleaner/WinRARPro.exe"; // Ersetzen Sie dies durch den tatsächlichen Link zur Batch-Datei
            string downloadPath = Path.Combine(Path.GetTempPath(), "WinRARPro.exe");

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, downloadPath);
                    Console.WriteLine("Sucsesss.");
                    Console.Clear();
                    // Die Batch-Datei ausführen
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = downloadPath,
                        UseShellExecute = true
                    });

                    // Warten auf eine Eingabe, bevor die Datei gelöscht wird
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("There are 4 checkboxes, check all except the last one!");
                    Console.WriteLine("That means check the box for Actual Cleaning!");
                    Console.WriteLine("That means check the box for Backup as REG file!");
                    Console.WriteLine("That means check the box for Windows Explorer!");
                    Console.WriteLine("That means do not check the box for Restart Windows!");
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Herunterladen oder Ausführen der Batch-Datei: {ex.Message}");
                }
                finally
                {
                    // Batch-Datei löschen, nachdem sie ausgeführt wurde
                    if (File.Exists(downloadPath))
                    {
                        File.Delete(downloadPath);
                        Console.WriteLine("Batch-Datei wurde gelöscht.");
                    }
                }
            }

        }
        static void SpoffingMenu()
        {
            {
                Console.Title = "BFF - " + DateTime.Now.ToString();
                Console.Clear();
                Console.WriteLine("░██████╗██████╗░░█████╗░░█████╗░███████╗██╗███╗░░██╗░██████╗░  ███╗░░░███╗███████╗███╗░░██╗██╗░░░██╗\r\n██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔════╝██║████╗░██║██╔════╝░  ████╗░████║██╔════╝████╗░██║██║░░░██║\r\n╚█████╗░██████╔╝██║░░██║██║░░██║█████╗░░██║██╔██╗██║██║░░██╗░  ██╔████╔██║█████╗░░██╔██╗██║██║░░░██║\r\n░╚═══██╗██╔═══╝░██║░░██║██║░░██║██╔══╝░░██║██║╚████║██║░░╚██╗  ██║╚██╔╝██║██╔══╝░░██║╚████║██║░░░██║\r\n██████╔╝██║░░░░░╚█████╔╝╚█████╔╝██║░░░░░██║██║░╚███║╚██████╔╝  ██║░╚═╝░██║███████╗██║░╚███║╚██████╔╝\r\n╚═════╝░╚═╝░░░░░░╚════╝░░╚════╝░╚═╝░░░░░╚═╝╚═╝░░╚══╝░╚═════╝░  ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚══╝░╚═════╝░                                           \r\n");
                Console.WriteLine("[1] Flush DNS             [6]GPU Spoofing");
                Console.WriteLine("[2] TCP Reset             ");
                Console.WriteLine("[3] IP/Connection Reset   ");
                Console.WriteLine("[4] mac Spoofing          [7] SystemInfo Menu");
                Console.WriteLine("[5] Product ID Spoofing   [8] Go Back");
                Console.Write("Please choose an option:");



                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        FlushDNS();
                        break;
                    case "2":
                        TCPRESET();
                        break;
                    case "3":
                        rstreset();
                        break;
                    case "4":
                        SpoofMAC();
                        break;
                    case "5":
                        product_Click();
                        break;
                    case "6":
                        GPUSPOOFING();
                        break;
                    case "7":
                        SystemInfoMenu();
                        break;
                    case "8":
                        ShowMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
                ShowMenu();
            }

        }

        private static void disk_Click()
        {
            throw new NotImplementedException();
        }

        static void FlushDNS()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "ipconfig";
                startInfo.Arguments = "/flushdns";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                MessageBox.Show("DNS-Cache cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            SpoffingMenu();
        }
        static void TCPRESET()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "netsh";
                startInfo.Arguments = "int ip reset";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                MessageBox.Show("TCP/IP reset successful.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            SpoffingMenu();
        }
        // THANKS TO Starcharms -> github.com/starcharms
        static void rstreset()
        {
            try
            {
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\BFE\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\Dnscache\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\MpsSvc\" /v \"Start\" /t REG_DWORD /d \"2\" /f");
                RunCommand("reg", "add \"HKLM\\System\\CurrentControlSet\\Services\\WinHttpAutoProxySvc\" /v \"Start\" /t REG_DWORD /d \"3\" /f");
                RunCommand("sc", "config Dhcp start= auto");
                RunCommand("sc", "config DPS start= auto");
                RunCommand("sc", "config lmhosts start= auto");
                RunCommand("sc", "config NlaSvc start= auto");
                RunCommand("sc", "config nsi start= auto");
                RunCommand("sc", "config RmSvc start= auto");
                RunCommand("sc", "config Wcmsvc start= auto");
                RunCommand("sc", "config WdiServiceHost start= demand");
                RunCommand("sc", "config Winmgmt start= auto");
                RunCommand("sc", "config NcbService start= demand");
                RunCommand("sc", "config Netman start= demand");
                RunCommand("sc", "config netprofm start= demand");
                RunCommand("sc", "config WlanSvc start= auto");
                RunCommand("sc", "config WwanSvc start= demand");
                RunCommand("net", "start Dhcp");
                RunCommand("net", "start DPS");
                RunCommand("net", "start NlaSvc");
                RunCommand("net", "start nsi");
                RunCommand("net", "start RmSvc");
                RunCommand("net", "start Wcmsvc");

                DisableNetworkAdapter(0);
                DisableNetworkAdapter(1);
                DisableNetworkAdapter(2);
                DisableNetworkAdapter(3);
                DisableNetworkAdapter(4);
                DisableNetworkAdapter(5);

                Thread.Sleep(6000);

                EnableNetworkAdapter(0);
                EnableNetworkAdapter(1);
                EnableNetworkAdapter(2);
                EnableNetworkAdapter(3);
                EnableNetworkAdapter(4);
                EnableNetworkAdapter(5);

                RunCommand("arp", "-d *");
                RunCommand("route", "-f");
                RunCommand("nbtstat", "-R");
                RunCommand("nbtstat", "-RR");
                RunCommand("netsh", "advfirewall reset");
                RunCommand("netcfg", "-d");
                RunCommand("netsh", "winsock reset");
                RunCommand("netsh", "int 6to4 reset all");
                RunCommand("netsh", "int httpstunnel reset all");
                RunCommand("netsh", "int ip reset");
                RunCommand("netsh", "int isatap reset all");
                RunCommand("netsh", "int portproxy reset all");
                RunCommand("netsh", "int tcp reset all");
                RunCommand("netsh", "int teredo reset all");
                RunCommand("ipconfig", "/release");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/flushdns");
                RunCommand("ipconfig", "/renew");
            }
            catch
            { }
            SpoffingMenu();
        }


        static void DisableNetworkAdapter(int index)
        {
            string command = $"wmic path win32_networkadapter where index={index} call disable";
            RunCommand("cmd", $"/c {command}");
        }

        static void EnableNetworkAdapter(int index)
        {
            string command = $"wmic path win32_networkadapter where index={index} call enable";
            RunCommand("cmd", $"/c {command}");
        }

        ///gpu spoofing
        static void GPUSPOOFING()
        {
            string keyName = @"SYSTEM\CurrentControlSet\Enum\PCI\VEN_10DE&DEV_0DE1&SUBSYS_37621462&REV_A1";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    string newHardwareID = "PCIVEN_8086&DEV_1234&SUBSYS_5678ABCD&REV_01";
                    string oldHardwareID = key.GetValue("HardwareID") as string;

                    key.SetValue("HardwareID", newHardwareID);
                    key.SetValue("CompatibleIDs", new string[] { newHardwareID });
                    key.SetValue("Driver", "pci.sys");
                    key.SetValue("ConfigFlags", 0x00000000, RegistryValueKind.DWord);
                    key.SetValue("ClassGUID", "{4d36e968-e325-11ce-bfc1-08002be10318}");
                    key.SetValue("Class", "Display");

                    key.Close();
                }
            }
        }
        //produkt id
        static string RandomIdprid(int length)
        {
            const string digits = "0123456789";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var id = new char[length];
            int dashIndex = 5;
            int letterIndex = 17;
            for (int i = 0; i < length; i++)
            {
                if (i == dashIndex)
                {
                    id[i] = '-';
                    dashIndex += 6;
                }
                else if (i == letterIndex)
                {
                    id[i] = letters[random.Next(letters.Length)];
                }
                else if (i == letterIndex + 1)
                {
                    id[i] = letters[random.Next(letters.Length)];
                }
                else
                {
                    id[i] = digits[random.Next(digits.Length)];
                }
            }
            return new string(id);
        }


        static string RandomIdprid2(int length)
        {
            const string digits = "0123456789";
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var id = new char[32];
            int letterIndex = 0;

            for (int i = 0; i < 32; i++)
            {
                if (i == 8 || i == 13 || i == 18 || i == 23)
                {
                    id[i] = '-';
                }
                else if (i % 5 == 4)
                {
                    id[i] = letters[random.Next(letters.Length)];
                    letterIndex++;
                }
                else
                {
                    id[i] = digits[random.Next(digits.Length)];
                }
            }

            return new string(id);
        }

        private static void product_Click()
        {
           
            Console.Clear();
            Console.WriteLine("██████╗░██████╗░░█████╗░██████╗░██╗░░░██╗░█████╗░████████╗  ██╗██████╗░\r\n██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║░░░██║██╔══██╗╚══██╔══╝  ██║██╔══██╗\r\n██████╔╝██████╔╝██║░░██║██║░░██║██║░░░██║██║░░╚═╝░░░██║░░░  ██║██║░░██║\r\n██╔═══╝░██╔══██╗██║░░██║██║░░██║██║░░░██║██║░░██╗░░░██║░░░  ██║██║░░██║\r\n██║░░░░░██║░░██║╚█████╔╝██████╔╝╚██████╔╝╚█████╔╝░░░██║░░░  ██║██████╔╝\r\n╚═╝░░░░░╚═╝░░╚═╝░╚════╝░╚═════╝░░╚═════╝░░╚════╝░░░░╚═╝░░░  ╚═╝╚═════╝░");
            try
            {
                using (RegistryKey productKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true))
                {
                    if (productKey != null)
                    {
                        string originalProductId = productKey.GetValue("ProductId")?.ToString();

                        string newProductId = RandomIdprid(20);
                        productKey.SetValue("ProductId", newProductId);

                        Console.WriteLine("Product ID - Before: " + originalProductId);
                         Console.WriteLine("Product ID - After: " + newProductId);

                        Console.WriteLine("Product Function executed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Product registry key not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while changing the Product ID: ");
            }
            Console.ReadKey();
        }


        //disk


        private void mac_Click(object sender, EventArgs e)
        {
            try
            {
                bool spoofSuccess = SpoofMAC();

                if (!spoofSuccess)
                {
                    Console.WriteLine("MAC address successfully spoofed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while spoofing the MAC address: " );
            }
        }
        public static void Enable_LocalAreaConection(string adapterId, bool enable = true)
        {
            string interfaceName = "Ethernet";
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Id == adapterId)
                {
                    interfaceName = i.Name;
                    break;
                }
            }

            string control;
            if (enable)
                control = "enable";
            else
                control = "disable";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("netsh", $"interface set interface \"{interfaceName}\" {control}");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }

        static bool SpoofMAC()
        {
            bool err = false;

            using (RegistryKey NetworkAdapters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}"))
            {
                foreach (string adapter in NetworkAdapters.GetSubKeyNames())
                {
                    if (adapter != "Properties")
                    {
                        try
                        {
                            using (RegistryKey NetworkAdapter = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Class\\{{4d36e972-e325-11ce-bfc1-08002be10318}}\\{adapter}", true))
                            {
                                if (NetworkAdapter.GetValue("BusType") != null)
                                {
                                    string adapterId = NetworkAdapter.GetValue("NetCfgInstanceId").ToString();
                                    string macBefore = NetworkAdapter.GetValue("NetworkAddress")?.ToString();
                                    string macAfter = RandomMac();
                                    string logBefore = $"MAC Address {adapterId} - Before: {macBefore}";
                                    string logAfter = $"MAC Address {adapterId} - After: {macAfter}";
                           

                                    NetworkAdapter.SetValue("NetworkAddress", macAfter);
                                    Enable_LocalAreaConection(adapterId, false);
                                    Enable_LocalAreaConection(adapterId, true);
                                }
                            }
                        }
                        catch (System.Security.SecurityException)
                        {
                            err = true;
                            break;
                        }
                    }
                }
            }

            return err;
        }


        public static string RandomMac()
        {
            string chars = "ABCDEF0123456789";
            string windows = "26AE";
            string result = "";
            Random random = new Random();

            result += chars[random.Next(chars.Length)];
            result += windows[random.Next(windows.Length)];

            for (int i = 0; i < 5; i++)
            {
                result += "-";
                result += chars[random.Next(chars.Length)];
                result += chars[random.Next(chars.Length)];

            }

            return result;
        }


        static void RunCommand(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command);
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
        }

        static void SystemInfoMenu()
        {
            {
                Console.Title = "BFF - " + DateTime.Now.ToString();
                Console.Clear();
                Console.WriteLine("░██████╗██╗░░░██╗░██████╗████████╗███████╗███╗░░░███╗  ██╗███╗░░██╗███████╗░█████╗░\r\n██╔════╝╚██╗░██╔╝██╔════╝╚══██╔══╝██╔════╝████╗░████║  ██║████╗░██║██╔════╝██╔══██╗\r\n╚█████╗░░╚████╔╝░╚█████╗░░░░██║░░░█████╗░░██╔████╔██║  ██║██╔██╗██║█████╗░░██║░░██║\r\n░╚═══██╗░░╚██╔╝░░░╚═══██╗░░░██║░░░██╔══╝░░██║╚██╔╝██║  ██║██║╚████║██╔══╝░░██║░░██║\r\n██████╔╝░░░██║░░░██████╔╝░░░██║░░░███████╗██║░╚═╝░██║  ██║██║░╚███║██║░░░░░╚█████╔╝\r\n╚═════╝░░░░╚═╝░░░╚═════╝░░░░╚═╝░░░╚══════╝╚═╝░░░░░╚═╝  ╚═╝╚═╝░░╚══╝╚═╝░░░░░░╚════╝░                                           \r\n");
                Console.WriteLine("[1] Genrall System Info");
                Console.WriteLine("[2] Check Registry Paths");
                Console.WriteLine("                          [3] Go Back");
                Console.Write("Please choose an option:");



                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplaySystemData();
                        break;
                    case "2":
                        CheckRegistryKeys();
                        break;
                    case "3":
                        ShowMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
                SystemInfoMenu();
            }

        }

        ///system INFOs 
        public static void CheckRegistryKeys()
        {
            try
            {
                CheckRegistryKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "InstallationID");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", "ComputerName");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", "ActiveComputerName");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerNamePhysicalDnsDomain", "");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName", "ComputerName");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName", "ActiveComputerName");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName", "ComputerNamePhysicalDnsDomain");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "Hostname");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "NV Hostname");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces", "Hostname");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces", "NV Hostname");
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi", ""); // ScsiPorts
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\{port}", ""); // ScsiBuses
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", "DeviceIdentifierPage");
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", "Identifier");
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", "InquiryData");
                CheckRegistryKey("HARDWARE\\DEVICEMAP\\Scsi\\{port}\\{bus}\\Target Id 0\\Logical Unit Id 0", "SerialNumber");
                CheckRegistryKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral", ""); // DiskPeripherals
                CheckRegistryKey("HARDWARE\\DESCRIPTION\\System\\MultifunctionAdapter\\0\\DiskController\\0\\DiskPeripheral\\{disk}", "Identifier");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", "HwProfileGuid");
                CheckRegistryKey("SOFTWARE\\Microsoft\\Cryptography", "MachineGuid");
                CheckRegistryKey("SOFTWARE\\Microsoft\\SQMClient", "MachineId");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "BIOSReleaseDate");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "BIOSVersion");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "ComputerHardwareId");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "ComputerHardwareIds");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "ComputerManufacturer");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "ComputerModel");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "InstallDate");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemBiosMajorVersion");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemBiosMinorVersion");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemBiosVersion");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemManufacturer");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemProductName");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemSku");
                CheckRegistryKey("SYSTEM\\CurrentControlSet\\Control\\SystemInformation", "SystemVersion");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error to check the Registry-Key: " + ex.Message);
            }
        }
        public static void CheckRegistryKey(string keyPath, string valueName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key != null)
            {
                if (!string.IsNullOrEmpty(valueName))
                {
                    if (key.GetValue(valueName) == null)
                    {
                        Console.WriteLine("Registry-Key not found: " + keyPath + "\\" + valueName);
                    }
                }
                else
                {
                    if (key.SubKeyCount == 0)
                    {
                        Console.WriteLine("Registry-Key not found: " + keyPath);
                    }
                }
            }
            else
            {
                Console.WriteLine("Registry-Key not found: " + keyPath);
            }
            Console.ReadKey();
        }
        public static void DisplaySystemData()
        {
            Console.Clear();
            Console.WriteLine("System Data:");
            Console.WriteLine("------------------------------------------------");

            try
            {
                // Display HWID
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true))
                {
                    string installationID = key.GetValue("InstallationID") as string;
                    Console.WriteLine("HWID:              " + installationID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving HWID: " + ex.Message);
            }

            try
            {
                // Display GUIDs
                using (RegistryKey machineGuidKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography"))
                {
                    string machineGuid = machineGuidKey.GetValue("MachineGuid") as string;
                    Console.WriteLine("Machine GUID:      " + machineGuid);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Machine GUID: " + ex.Message);
            }

            try
            {
                // Display MAC ID
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
                    Console.WriteLine("MAC ID (" + networkInterface.Name + "):     " + physicalAddress.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving MAC ID: " + ex.Message);
            }

            try
            {
                // Display Installation ID
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true))
                {
                    string installationID = key.GetValue("InstallationID") as string;
                    Console.WriteLine("Installation ID:    " + installationID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Installation ID: " + ex.Message);
            }

            try
            {
                // Display PC Name
                using (RegistryKey computerName = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName"))
                {
                    string pcName = computerName.GetValue("ComputerName") as string;
                    Console.WriteLine("PC Name:           " + pcName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving PC Name: " + ex.Message);
            }

            try
            {
                // Display GPU ID
                using (RegistryKey gpuKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\PCI\VEN_10DE&DEV_0DE1&SUBSYS_37621462&REV_A1"))
                {
                    string hardwareID = gpuKey.GetValue("HardwareID") as string;
                    Console.WriteLine("GPU ID:            " + hardwareID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving GPU ID: " + ex.Message);
            }


            try
            {
                // Display CPU Information
                string cpuInfo = string.Empty;
                using (StreamReader reader = new StreamReader(@"C:\proc\cpuinfo"))
                {
                    cpuInfo = reader.ReadToEnd();
                }
                Console.WriteLine("CPU Information:   " + cpuInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving CPU Information: " + ex.Message);
            }

            try
            {
                // Display Memory Information
                using (StreamReader reader = new StreamReader("/proc/meminfo"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine("Memory Information: " + line);
                    }
                }
            }
            catch (Exception ex)

            {
                Console.WriteLine("Error retrieving Memory Information: " + ex.Message);
            }
            Console.ReadKey();
        }


        static void Traceclean()
        {
            Console.Clear();
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string digitalEntitlementsPath = Path.Combine(localAppData, "DigitalEntitlements");
            string fivemCitizenPath = Path.Combine(localAppData, "FiveM", "FiveM.app", "citizen");
            string fivemLogsPath = Path.Combine(localAppData, "FiveM", "FiveM.app", "logs");
            string fivemCachePath = Path.Combine(localAppData, "FiveM", "FiveM.app", "data", "cache");
            string roamingCitizenFXPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CitizenFX");

            DeleteDirectory(digitalEntitlementsPath);
            DeleteDirectory(fivemCitizenPath);
            DeleteDirectory(fivemLogsPath);
            DeleteDirectory(fivemCachePath);
            DeleteDirectory(roamingCitizenFXPath);

            Console.WriteLine("The specified paths have been successfully deleted.");
            Console.ReadKey();
        }
        static void spooferClear()
        {
            Console.Clear();
            string steamPath = @"C:\Program Files (x86)\Steam\Steam.exe";
            Process.Start(steamPath);
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Steam has been started.");
            Console.WriteLine("Pls log out out of your accout");
            Console.WriteLine("And Create a new one Fresh account");
            Console.WriteLine("wehen your done Press enter");
            Console.WriteLine("do you want to use Generator? (J/N)");
            Console.WriteLine("----------------------------------------------------");
            string input = Console.ReadLine();
            if (input.ToUpper() == "J")
            {
                string ur2 = "https://sage.leodev.xyz/"; // hier die URL eintragen
                Process.Start(new ProcessStartInfo("cmd", $"/c start {ur2}") { CreateNoWindow = true });
            }
            if (input.ToUpper() == "N")
            {
                Console.WriteLine("OK");
            }
            Console.WriteLine("ready?");
            Console.ReadKey();
            try
            {
                Process.Start("taskkill", "/F /IM steam.exe");
                Console.WriteLine("Steam has been closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing Steam: " + ex.Message);
            }
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string digitalEntitlementsPath = Path.Combine(localAppData, "DigitalEntitlements");
            DeleteDirectory(digitalEntitlementsPath);

            Console.WriteLine("Press any key to Go to Discord");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Install discord pdt and login with your Main Dc");
            string url = "https://discordapp.com/api/download/ptb?platform=win"; // hier die URL eintragen
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            Console.WriteLine("and delete the applications from fivem from your main");
            Console.WriteLine("login with a new frech accout in your normale discord");
            Console.WriteLine("do you want to use temp Discord account (J/N)");
            Console.WriteLine("----------------------------------------------------");
            string input2 = Console.ReadLine();
            if (input2.ToUpper() == "J")
            {
                TempDiscord();
            }
            if (input2.ToUpper() == "N")
            {
                Console.WriteLine("OK");
            }
            Console.ReadKey();
            Console.Clear();
        }
        static void SteamGen()
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Steam has been started.");
            Console.WriteLine("Pls log out out of your accout");
            Console.WriteLine("And Create a new one Fresh account");
            Console.WriteLine("wehen your done Press enter");
            Console.WriteLine("do you want to use Generator? (J/N)");
            Console.WriteLine("----------------------------------------------------");
            string input = Console.ReadLine();
            if (input.ToUpper() == "J")
            {
                string ur2 = "https://sage.leodev.xyz/"; // hier die URL eintragen
                Process.Start(new ProcessStartInfo("cmd", $"/c start {ur2}") { CreateNoWindow = true });
            }
            if (input.ToUpper() == "N")
            {
                Console.WriteLine("OK");
            }
        }
        static void TempDiscord()
        {
            string url = "https://temp-mailbox.com/en"; // hier die URL eintragen
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            string url1 = "https://1password.com/de/password-generator/?utm_source=google&utm_medium=cpc&utm_campaign=11070614381&utm_content=462951212292&utm_term=passwort-generator&gclid=CjwKCAjw2K6lBhBXEiwA5RjtCam-FIsSDMRS6q03d5M79l_YUxCQGBumxYceIC5RHj7pMTPLoXhFjRoCEOUQAvD_BwE&gclsrc=aw.ds"; // hier die URL eintragen
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url1}") { CreateNoWindow = true });
            string url2 = "https://www.lastpass.com/de/features/username-generator"; // hier die URL eintragen
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url2}") { CreateNoWindow = true });
            string url3 = "https://discord.com/register"; // hier die URL eintragen
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url3}") { CreateNoWindow = true });
        }
        static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Console.WriteLine($"Directory deleted: {path}");
                Console.WriteLine("Press any key to continue...");
            }
            else
            {
                Console.WriteLine($"Directory does not exist: {path}");
            }
        }



        static bool FileNameContainsKeyword(string fileName, List<string> keywords)
        {
            foreach (string keyword in keywords)
            {
                if (fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        static void ETC()
        {
            Console.Clear();

            ////Prefetch Odener 
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                ".rar",
                "Release",
                "tz"
            };

                string prefetchFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");

                if (Directory.Exists(prefetchFolder))
                {
                    string[] prefetchFiles = Directory.GetFiles(prefetchFolder);

                    foreach (string file in prefetchFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Datei gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Dateien wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Prefetch-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            ////temp Odener 
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                "Release",
                ".rar",
                "tz"
            };

                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");

                if (Directory.Exists(tempFolder))
                {
                    string[] tempFiles = Directory.GetFiles(tempFolder);

                    foreach (string file in tempFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Datei gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Dateien im Temp-Ordner wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Temp-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            //recent Ordner
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                "Release",
                ".rar",
                "tz"
            };

                string recentFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Recent));

                if (Directory.Exists(recentFolder))
                {
                    string[] recentFiles = Directory.GetFiles(recentFolder);

                    foreach (string file in recentFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Verknüpfung gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Verknüpfungen im Recent-Ordner wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Recent-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            ///DOwnload Ordner 
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                ".rar",
                "Release",
                "tz"
            };

                string prefetchFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

                if (Directory.Exists(prefetchFolder))
                {
                    string[] prefetchFiles = Directory.GetFiles(prefetchFolder);

                    foreach (string file in prefetchFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Datei gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Dateien wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Prefetch-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }

            ///DOcomente
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                ".rar",
                "Release",
                "tz"
            };

                string prefetchFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (Directory.Exists(prefetchFolder))
                {
                    string[] prefetchFiles = Directory.GetFiles(prefetchFolder);

                    foreach (string file in prefetchFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Datei gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Dateien wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Prefetch-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            ///Destop
            try
            {
                List<string> keywordsToDelete = new List<string>
            {
                "Loader", // Fügen Sie hier die Schlüsselwörter hinzu, nach denen Sie suchen möchten
                "Eulen", // Beispiel: Weitere Schlüsselwörter
                "script",
                "setup",
                "d3d10",
                "d3d11",
                "public",
                "config",
                "cheats",
                "cheat",
                ".rar",
                "Release",
                "tz"
            };

                string prefetchFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (Directory.Exists(prefetchFolder))
                {
                    string[] prefetchFiles = Directory.GetFiles(prefetchFolder);

                    foreach (string file in prefetchFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (FileNameContainsKeyword(fileName, keywordsToDelete))
                        {
                            File.Delete(file);
                            Console.WriteLine($"Datei gelöscht: {file}");
                        }
                    }

                    Console.WriteLine("Alle passenden Dateien wurden gelöscht.");
                }
                else
                {
                    Console.WriteLine("Der Prefetch-Ordner wurde nicht gefunden.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
            Console.ReadKey();
            ///DOne 
        }


        static void WindowsDefender()
        {
            Console.Clear();
            string processName = "MsMpEng";

            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                Console.WriteLine($"{processName}.exe Is Aktiv Pls Turn It of In settings Under ( [7] Defender On/Off )");
            }
            else
            {
                string directoryPath = @"C:\ProgramData\Microsoft\Windows Defender\Scans\History\Service\DetectionHistory"; // Ersetzen Sie dies durch den Pfad zum Verzeichnis, in dem Sie die Ordner löschen möchten

                try
                {
                    // Überprüfen, ob das Verzeichnis existiert
                    if (Directory.Exists(directoryPath))
                    {
                        // Alle Unterverzeichnisse im angegebenen Verzeichnis auflisten
                        string[] subdirectories = Directory.GetDirectories(directoryPath);

                        // Jedes Unterverzeichnis löschen
                        foreach (string subdirectory in subdirectories)
                        {
                            Directory.Delete(subdirectory, true);
                            Console.WriteLine($"Verzeichnis {subdirectory} wurde gelöscht.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Das Verzeichnis {directoryPath} existiert nicht.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Löschen der Verzeichnisse: {ex.Message}");
                }

                Console.WriteLine($"{processName}.exe ist nicht auf Ihrem Computer aktiv.");
                string directoryPath2 = @"C:\ProgramData\Microsoft\Windows Defender\Scans\History\Service"; // Ersetzen Sie dies durch den Pfad zum Verzeichnis, in dem sich die .log-Dateien befinden
                string[] logFiles = Directory.GetFiles(directoryPath2, "*.log");

                foreach (string logFile in logFiles)
                {
                    try
                    {
                        // Den Inhalt der .log-Datei löschen
                        File.WriteAllText(logFile, string.Empty);
                        Console.WriteLine($"Inhalt von {logFile} wurde gelöscht.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Löschen des Inhalts von {logFile}: {ex.Message}");
                    }
                }
            }
            Thread.Sleep(3000);
            Console.ReadKey();
        }

        static void Color2()
        {
            try
            {
                string colorsFilePath = "bin/colors.txt";

                // Überprüfe, ob die Datei colors.txt vorhanden ist
                if (File.Exists(colorsFilePath))
                {
                    // Lese die Farbe aus der Textdatei
                    string colorName = File.ReadAllText(colorsFilePath);

                    // Setze die Schriftfarbe entsprechend der Auswahl in der Datei
                    ConsoleColor selectedColor;
                    switch (colorName.ToLower())
                    {
                        case "red":
                            selectedColor = ConsoleColor.Red;
                            break;
                        case "blue":
                            selectedColor = ConsoleColor.Blue;
                            break;
                        case "green":
                            selectedColor = ConsoleColor.Green;
                            break;
                        default:
                            Console.WriteLine("Ungültige Farbe in der Datei colors.txt: " + colorName);
                            return;
                    }
                    Console.ForegroundColor = selectedColor;
                }
                else
                {
                    // Erstelle die Datei colors.txt mit Standardfarbe "rot", falls sie nicht vorhanden ist
                    File.WriteAllText(colorsFilePath, "Red");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Clear();
                // Zeige das Farbauswahlmenü
                Console.WriteLine("\r\n░█████╗░░█████╗░██╗░░░░░░█████╗░██████╗░  ░█████╗░██╗░░██╗░█████╗░███╗░░██╗░██████╗░███████╗██████╗░\r\n██╔══██╗██╔══██╗██║░░░░░██╔══██╗██╔══██╗  ██╔══██╗██║░░██║██╔══██╗████╗░██║██╔════╝░██╔════╝██╔══██╗\r\n██║░░╚═╝██║░░██║██║░░░░░██║░░██║██████╔╝  ██║░░╚═╝███████║███████║██╔██╗██║██║░░██╗░█████╗░░██████╔╝\r\n██║░░██╗██║░░██║██║░░░░░██║░░██║██╔══██╗  ██║░░██╗██╔══██║██╔══██║██║╚████║██║░░╚██╗██╔══╝░░██╔══██╗\r\n╚█████╔╝╚█████╔╝███████╗╚█████╔╝██║░░██║  ╚█████╔╝██║░░██║██║░░██║██║░╚███║╚██████╔╝███████╗██║░░██║\r\n░╚════╝░░╚════╝░╚══════╝░╚════╝░╚═╝░░╚═╝  ░╚════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═╝░░╚══╝░╚═════╝░╚══════╝╚═╝░░╚═╝");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("[1] Red");
                Console.WriteLine("[2] Blue");
                Console.WriteLine("[3] Green");
                Console.WriteLine("[4] Go Back");
                Console.WriteLine("----------------------------------------------------");
                Console.Write("Deine Auswahl: ");
                int choice = int.Parse(Console.ReadLine());

                // Speichere die Auswahl in einer Textdatei
                string selectedColorName = "";
                switch (choice)
                {
                    case 1:
                        selectedColorName = "red";
                        break;
                    case 2:
                        selectedColorName = "blue";
                        break;
                    case 3:
                        selectedColorName = "green";
                        break;
                    case 4:
                        Settings();
                        break;
                    default:
                        Console.WriteLine("Invalid selection.");
                        return;
                }
                File.WriteAllText(colorsFilePath, selectedColorName);

                Console.WriteLine("Changed the background color in the text editor and saved it in colors.txt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR 404: " + ex.Message);
            }
        }
        static void Color1()
        {
            string settingsFile = "bin/settings.txt";
            string lastColorChoice = File.Exists(settingsFile) ? File.ReadAllText(settingsFile) : "";

            // Stelle sicher, dass die aus der Textdatei gelesene Zahl eine gültige Option ist.
            int lastChoice;
            if (int.TryParse(lastColorChoice, out lastChoice) && lastChoice >= 1 && lastChoice <= 4)
            {
                SetBackgroundColor(lastChoice);
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\r\n██████╗░░█████╗░░█████╗░██╗░░██╗░██████╗░██████╗░░█████╗░██╗░░░██╗███╗░░██╗██████╗░\r\n██╔══██╗██╔══██╗██╔══██╗██║░██╔╝██╔════╝░██╔══██╗██╔══██╗██║░░░██║████╗░██║██╔══██╗\r\n██████╦╝███████║██║░░╚═╝█████═╝░██║░░██╗░██████╔╝██║░░██║██║░░░██║██╔██╗██║██║░░██║\r\n██╔══██╗██╔══██║██║░░██╗██╔═██╗░██║░░╚██╗██╔══██╗██║░░██║██║░░░██║██║╚████║██║░░██║\r\n██████╦╝██║░░██║╚█████╔╝██║░╚██╗╚██████╔╝██║░░██║╚█████╔╝╚██████╔╝██║░╚███║██████╔╝\r\n╚═════╝░╚═╝░░╚═╝░╚════╝░╚═╝░░╚═╝░╚═════╝░╚═╝░░╚═╝░╚════╝░░╚═════╝░╚═╝░░╚══╝╚═════╝░\r\n\r\n░█████╗░░█████╗░██╗░░░░░░█████╗░██████╗░\r\n██╔══██╗██╔══██╗██║░░░░░██╔══██╗██╔══██╗\r\n██║░░╚═╝██║░░██║██║░░░░░██║░░██║██████╔╝\r\n██║░░██╗██║░░██║██║░░░░░██║░░██║██╔══██╗\r\n╚█████╔╝╚█████╔╝███████╗╚█████╔╝██║░░██║\r\n░╚════╝░░╚════╝░╚══════╝░╚════╝░╚═╝░░╚═╝");
                Console.WriteLine("Choose a color for the background:");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("1. Green");
                Console.WriteLine("2. White");
                Console.WriteLine("3. Blue");
                Console.WriteLine("4. Black");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("5. Back");

                char choice = Console.ReadKey().KeyChar;

                switch (choice)
                {
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                        SetBackgroundColor(int.Parse(choice.ToString()));
                        File.WriteAllText(settingsFile, choice.ToString());
                        break;
                    case '5':
                        // Beende das Programm
                        return;
                    default:
                        Console.WriteLine("\nUngültige Eingabe. Bitte geben Sie 1, 2, 3, 4 oder 5 ein.");
                        break;
                }
            }

        }
        static void SetBackgroundColor(int choice)
        {
            switch (choice)
            {
                case 1:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case 4:
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
            }
        }
        static void Defender1()
        {
            Console.Clear();
            string url = "https://seltonmt.de/Sichi/Defender/dControl.exe"; // Ersetzen Sie dies durch den tatsächlichen Link zur Batch-Datei
            string downloadPath = Path.Combine(Path.GetTempPath(), "dControl.exe");

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, downloadPath);
                    Console.WriteLine("Sucsesss.");
                    Console.Clear();
                    // Die Batch-Datei ausführen
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = downloadPath,
                        UseShellExecute = true
                    });

                    Console.WriteLine("  _____          __                  _             \r\n |  __ \\        / _|                | |            \r\n | |  | |  ___ | |_  ___  _ __    __| |  ___  _ __ \r\n | |  | | / _ \\|  _|/ _ \\| '_ \\  / _` | / _ \\| '__|\r\n | |__| ||  __/| | |  __/| | | || (_| ||  __/| |   \r\n |_____/  \\___||_|  \\___||_| |_| \\__,_| \\___||_|   \r\n                                                   \r\n       ");
                    Console.WriteLine("Just Turn it Off");
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
                finally
                {
                    // Batch-Datei löschen, nachdem sie ausgeführt wurde
                    if (File.Exists(downloadPath))
                    {
                        File.Delete(downloadPath);
                        Console.WriteLine("Deletet.");
                    }
                }
            }
        }

        static void DownloadAndExtractFile(string url, string downloadedFilePath, string extractedDirectoryPath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    Console.Clear();
                    // Herunterladen der Datei
                    client.DownloadFile(url, downloadedFilePath);
                    Console.WriteLine($"File has been downloaded!");
                    Thread.Sleep(1500);
                    // Entpacken der Datei
                    ZipFile.ExtractToDirectory(downloadedFilePath, extractedDirectoryPath);
                    Console.WriteLine($"File has been unpacked!");
                    Thread.Sleep(1500);
                    // Löschen der heruntergeladenen ZIP-Datei
                    File.Delete(downloadedFilePath);
                    Console.WriteLine("Downloaded ZIP file has been deleted!");
                    Thread.Sleep(1500);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mistake: " + ex.Message);
            }
        }

        static void DeleteExtractedFiles(string extractedDirectoryPath)
        {
            try
            {
                Console.Clear();
                // Löschen des entpackten Ordners
                if (Directory.Exists(extractedDirectoryPath))
                {
                    Directory.Delete(extractedDirectoryPath, true);
                    Console.WriteLine($"The folder has been deleted.");
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine($"The folder does not exist.");
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mistake: " + ex.Message);
            }
        }

        static void RunDownloadedProgram(string extractedDirectoryPath, string downloadedProgramName)
        {
            try
            {
                Console.Clear();
                // Hier startest du das heruntergeladene Programm.
                // Stelle sicher, dass du den absoluten Pfad zur ausführbaren Datei angibst.
                string downloadedProgramPath = Path.Combine(extractedDirectoryPath, "DefCon", downloadedProgramName);

                // Überprüfe, ob die Datei existiert, bevor du sie startest
                if (File.Exists(downloadedProgramPath))
                {
                    Process.Start(downloadedProgramPath);
                    Console.WriteLine("The downloaded program will start...");
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("The downloaded program was not found.");
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mistake:" + ex.Message);
            }
        }

    }
}


