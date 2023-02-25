﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using Unowhy_Tools_WPF.ViewModels;
using System.ComponentModel;
using Unowhy_Tools_WPF.Views;
using System.Windows.Forms;
using Unowhy_Tools_WPF.Views.Pages;
using System.Security.Principal;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Runtime.CompilerServices;
using Unowhy_Tools_WPF.Views.Windows;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace Unowhy_Tools
{
    public partial class UT
    {
        #region DLL
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int state, int value);
        #endregion

        public static string ver = "20.0";
        public static int verfull = 2000;
        public static int verbuild = 2422143;
        public static bool verisdeb = true;

        public class version
        {
            public static string getver()
            {
                return ver;
            }

            public static int getverfull()
            {
                return verfull;
            }

            public static bool isdeb()
            {
                return verisdeb;
            }

            public static int getverbuild()
            {
                return verbuild;
            }

            public static async Task<bool> newver()
            {
                var web = new HttpClient();
                string newver = await web.GetStringAsync("https://bit.ly/UTnvTXT");
                int newverint = Convert.ToInt32(newver);
                if (verfull < newverint)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public class serv
        {
            public static async Task<bool> exist(string service)
            {
                Write2Log("Check " + service);
                string s = await RunReturn("sc", "query \"" + service + "\"");
                if (s.Contains("1060"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public static async Task stop(string service)
            {
                Write2Log("Stop " + service);
                await RunMin("net", "stop \"" + service + "\" /y");
            }

            public static async Task start(string service)
            {
                Write2Log("Start " + service);
                await RunMin("net", "start \"" + service + "\"");
            }

            public static async Task auto(string service)
            {
                Write2Log("Enable " + service);
                await RunMin("sc", "config \"" + service + "\" start=auto");
            }

            public static async Task dis(string service)
            {
                Write2Log("Disable " + service);
                await serv.stop(service);
                await RunMin("sc", "config \"" + service + "\" start=disabled");
            }

            public static async Task del(string service)
            {
                Write2Log("Delete " + service);
                await serv.stop(service);
                await RunMin("sc", "delete \"" + service + "\"");
            }
        }

        public class waitstatus
        {
            public async static Task close()
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as Unowhy_Tools_WPF.Views.Container;
                await mainWindow.HideWait();
                Write2Log("Close wait");
            }

            public async static Task open()
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as Unowhy_Tools_WPF.Views.Container;
                await mainWindow.ShowWait();
                Write2Log("Open wait");
            }
        }

        public class splashstatus
        {
            public static void close()
            {
                Write2Log("Close splash");
            }

            public static void open()
            {
                Write2Log("Open splash");
            }
        }

        public static void Delay(int Time_delay)
        {
            int i = 0;
            System.Timers.Timer _delayTimer = new System.Timers.Timer();
            _delayTimer.Interval = Time_delay;
            _delayTimer.AutoReset = false;
            _delayTimer.Elapsed += (s, args) => i = 1;
            _delayTimer.Start();
            while (i == 0) { };
        }

        public static async Task<string> RunReturn(string file, string args)
        {
            IntPtr wow64Value = IntPtr.Zero;
            Wow64DisableWow64FsRedirection(ref wow64Value);

            Write2Log("ReturnCMD " + file + " " + args);

            string output = await Task.Run(() =>
            {
                Process get = new Process();
                get.StartInfo.FileName = file;
                get.StartInfo.Arguments = args;
                get.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                get.StartInfo.UseShellExecute = false;
                get.StartInfo.RedirectStandardOutput = true;
                get.StartInfo.CreateNoWindow = true;
                get.Start();
                get.WaitForExit();
                return get.StandardOutput.ReadToEnd();
            });

            Write2Log("Done ReturnCMD " + file + " " + args + " => " + output);

            return output;
        }

        public static string GetLine(string text, int line)
        {
            int line2 = line - 1;
            var lines = text.Split('\n');
            return lines[line2].Replace("\n", "").Replace("\r", "");
        }

        public static string GetLang(string name)
        {
            //Check the current saved language
            string resxFile;
            RegistryKey utl = Registry.CurrentUser.OpenSubKey(@"Software\STY1001\Unowhy Tools", false);
            string utls = utl.GetValue("Lang").ToString();
            string enresx = @".\lang\en.resx";
            string frresx = @".\lang\fr.resx";
            //Chose the ResX file
            if (utls == "EN") resxFile = enresx;    //English   
            else resxFile = frresx;               //French
            ResXResourceSet resxSet = new ResXResourceSet(resxFile);
            Write2Log("Get lang " + name + " => " + resxSet.GetString(name));
            return resxSet.GetString(name);
        }

        public async static Task RunMin(string file, string args)
        {
            IntPtr wow64Value = IntPtr.Zero;
            Wow64DisableWow64FsRedirection(ref wow64Value);

            Write2Log("RunMin " + file + " " + args);

            await Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.FileName = file;
                p.StartInfo.Arguments = args;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
            });

            Write2Log("Done RunMin " + file + " " + args);
        }

        public static void Write2Log(string log)
        {
            if (!File.Exists(Path.GetTempPath() + "\\UT_Logs.txt"))
            {
                var f = File.CreateText(Path.GetTempPath() + "\\UT_Logs.txt");
                f.Close();
            }

            File.AppendAllText(Path.GetTempPath() + "\\UT_Logs.txt", DateTime.Now.ToString() + " : " + log + Environment.NewLine);
        }

        public static void applylang_global()
        {
            
        }

        public static bool CheckAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RunAdmin(string args)
        {
            if (!CheckAdmin())
            {
                // Restart and run as admin
                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.Verb = "runas";
                startInfo.Arguments = $"{args}";
                Process.Start(startInfo);
                System.Windows.Application.Current.Shutdown();
            }
        }

        public static bool CheckInternet()
        {
            int Out;
            if (InternetGetConnectedState(out Out, 0) == true) return true;
            else return false;
        }

        public static async Task Check()
        {
            Data UTdata = new Data();

            Write2Log("Getting PC Infos");

            string hn = await RunReturn("hostname", "");
            string mf = await RunReturn("wmic", "computersystem get manufacturer");
            string md = await RunReturn("wmic", "computersystem get model");
            string os = await RunReturn("wmic", "os get caption");
            string bios = await RunReturn("wmic", "bios get smbiosbiosversion");
            string sn = await RunReturn("wmic", "bios get serialnumber");

            UTdata.HostName = hn.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            UTdata.mf = GetLine(mf, 2);
            UTdata.md = GetLine(md, 2);
            UTdata.os = GetLine(os, 2);
            UTdata.bios = GetLine(bios, 2);
            UTdata.sn = GetLine(sn, 2);

            if (UTdata.UserID.Contains(UTdata.HostName.ToLower()))
            {
                UTdata.User = UTdata.UserID.Replace(UTdata.HostName.ToLower() + "\\", "");
            }
            else if (UTdata.UserID.Contains("AzureAD"))
            {
                UTdata.User = UTdata.UserID;
                UTdata.AADUser = true;
            }

            Write2Log(UTdata.HostName);
            Write2Log(UTdata.User);
            Write2Log(UTdata.UserID);
            Write2Log(UTdata.mf);
            Write2Log(UTdata.md);
            Write2Log(UTdata.os);
            Write2Log(UTdata.bios);
            Write2Log(UTdata.sn);

            Write2Log("Done");

            Write2Log("====== Dynamic Buttons ======");

            string preazure = await RunReturn("powershell", "start-process -FilePath \"dsregcmd\" -ArgumentList \"/status\" -nonewwindow");
            string preadmins = await RunReturn("net", "localgroup Administrateurs"); ;
            string users = await RunReturn("net", "user");
            string bcd = await RunReturn("bcdedit", "");

            string admins = preadmins.ToLower();
            string azure = GetLine(preazure, 6);

            #region Hisqool Manager

            Write2Log("=== HSM service ===");
            bool hsme = await UT.serv.exist("HiSqoolManager");
            if (hsme)
            {
                ServiceController sc = new ServiceController();
                sc.ServiceName = "HiSqoolManager";

                if (sc.StartType == ServiceStartMode.Automatic)
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        UTdata.HSMRunning = true;
                        UTdata.HSMEnabled = true;
                        UTdata.HSMExist = true;
                        UTdata.HSMExeExist = true;
                        Write2Log("HSM is running");
                    }
                    else
                    {
                        UTdata.HSMRunning = false;
                        UTdata.HSMEnabled = true;
                        UTdata.HSMExist = true;
                        UTdata.HSMExeExist = true;
                        Write2Log("HSM is stopped");
                    }
                }
                else
                {
                    UTdata.HSMRunning = false;
                    UTdata.HSMEnabled = false;
                    UTdata.HSMExist = true;
                    UTdata.HSMExeExist = true;
                    Write2Log("HSM is disabled");
                }
                if (!File.Exists("C:\\Program Files\\Unowhy\\HiSqool Manager\\HiSqoolManager.exe"))
                {
                    UTdata.HSMRunning = true;
                    UTdata.HSMEnabled = true;
                    UTdata.HSMExist = true;
                    UTdata.HSMExeExist = false;
                    Write2Log("HSM is deleted");
                }
            }
            else
            {
                UTdata.HSMRunning = false;
                UTdata.HSMEnabled = false;
                UTdata.HSMExist = false;
                UTdata.HSMExeExist = false;
                Write2Log("HSM services is not present");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region Azure

            Write2Log("=== Azure AD ===");
            if (azure.Contains("NO"))
            {
                UTdata.AAD = false;
                Write2Log("Azure AD joined: NO");
            }
            else
            {
                UTdata.AAD = true;
                Write2Log("Azure AD joined: YES");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region Admins

            Write2Log("=== Admins ===");
            if (admins.Contains(UTdata.User))
            {
                UTdata.Admin = true;
                Write2Log("User is admin");
            }
            else
            {
                UTdata.Admin = false;
                Write2Log("User is not admin");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region Folders

            Write2Log("=== Folders ===");
            if (Directory.Exists("C:\\ProgramData\\RIDF") == false)
            {
                UTdata.RIDFFolderExist = false;
                Write2Log("RIDF: False");
            }
            else
            {
                UTdata.RIDFFolderExist = true;
                Write2Log("RIDF: True");
            }
            if (Directory.Exists("C:\\ProgramData\\ENT") == false)
            {
                UTdata.ENTFolderExist = false;
                Write2Log("ENT: False");
            }
            else
            {
                UTdata.ENTFolderExist = true;
                Write2Log("ENT: True");
            }
            if (Directory.Exists("C:\\Windows\\system32\\OEM") == false)
            {
                UTdata.OEMFolderExist = false;
                Write2Log("OEM: False");
            }
            else
            {
                UTdata.OEMFolderExist = true;
                Write2Log("OEM: True");
            }
            if (Directory.Exists("C:\\Program Files\\Unowhy\\TO_INSTALL") == false)
            {
                UTdata.TIFolderExist = false;
                Write2Log("TO_INSTALL: False");
            }
            else
            {
                UTdata.TIFolderExist = true;
                Write2Log("TO_INSTALL: True");
            }
            if (Directory.Exists("C:\\Program Files\\Unowhy\\HiSqool Manager") == false)
            {
                UTdata.HSQMFolderExist = false;
                Write2Log("Hisqool Manager: False");
            }
            else
            {
                UTdata.HSQMFolderExist = true;
                Write2Log("Hisqool Manager: True");
            }
            if (Directory.Exists("C:\\Program Files\\Unowhy\\HiSqool") == false)
            {
                UTdata.HSMExist = false;
                Write2Log("Hisqool: False");
            }
            else
            {
                UTdata.HSMExist = true;
                Write2Log("Hisqool: True");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region ENT user

            Write2Log("=== ./ENT ===");
            if (users.Contains("ENT"))
            {
                UTdata.ENTUser = true;
                Write2Log("ENT User is present");
            }
            else
            {
                UTdata.ENTUser = false;
                Write2Log("ENT User is not present");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region Windows Hello Enterprise

            Write2Log("=== Win Hello Ent ===");
            Write2Log("Open Key");
            RegistryKey whe1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\PassportForWork");
            //RegistryKey whe2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WinBio\\Credential Provider");
            RegistryKey whe3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\PassportForWork\\DynamicLock");
            Write2Log("Open Key End");
            if (whe1 == null || /*whe2 == null ||*/ whe3 == null)
            {
                UTdata.WHE = false;
                Write2Log("WHE No Key");
            }
            else
            {
                Write2Log("Open Val");
                var val5 = whe3.GetValue("DynamicLock");
                //string val2 = whe2.GetValue("Domain Accounts");
                var val1 = whe1.GetValue("Enabled");
                var val3 = whe1.GetValue("RequireSecurityDevice");
                //string val4 = whe1.GetValue("UseCertificateForOnPremAuth").ToString();
                Write2Log("Open Val End");

                if (val1 == null || val3 == null || val5 == null)
                {
                    UTdata.WHE = false;
                    Write2Log("WHE No Value");
                }
                else
                {
                    string val21 = val1.ToString();
                    string val23 = val3.ToString();
                    string val25 = val5.ToString();
                    Write2Log("Read Val");
                    if (val21.Contains("1") && /*val2.Contains("1") &&*/ val23.Contains("1") && /*val4.Contains("1") &&*/ val25.Contains("1"))
                    {
                        UTdata.WHE = true;
                        Write2Log("WHE OK");
                    }
                    else
                    {
                        UTdata.WHE = false;
                        Write2Log("WHE Not OK");
                    }
                    Write2Log("Read Val End");
                }
            }
            if (whe1 != null)
            {
                whe1.Close();
            }
            /*if (whe2 != null)
            {
                whe2.Close();
            }*/
            if (whe3 != null)
            {
                whe3.Close();
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region TI Start

            Write2Log("=== Auto script of TI ===");
            DirectoryInfo dir = new DirectoryInfo("C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
            FileInfo[] files = dir.GetFiles("silent_" + "*.*");
            if (files.Length > 0)
            {
                UTdata.TIStartup = true;
                Write2Log("Present");
            }
            else
            {
                UTdata.TIStartup = false;
                Write2Log("Not present");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region BootIM

            Write2Log("=== BootIM ===");
            RegistryKey bim1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\bootim.exe");
            if (bim1 == null)
            {
                UTdata.BIM = true;
                Write2Log("OK");
            }
            else
            {
                UTdata.BIM = false;
                Write2Log("Redirected");
            }
            if (bim1 != null)
            {
                bim1.Close();
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region BCDedit

            Write2Log("=== BCD ===");
            if (bcd.Contains("IgnoreAllFailures"))
            {
                UTdata.BCD = false;
                Write2Log("BCD patched");
            }
            else
            {
                UTdata.BCD = true;
                Write2Log("BCD normal");
            }
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            #region Shell

            Write2Log("=== Shell ===");
            RegistryKey shellkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            string shellval = shellkey.GetValue("Shell").ToString();
            if (shellval == "explorer.exe")
            {
                UTdata.ShellOK = true;
            }
            else
            {
                UTdata.ShellOK = false;
            }
            Write2Log($"Shell: {shellval}");
            Write2Log("=== End ===" + Environment.NewLine);

            #endregion

            Write2Log("====== End ======");
        }
        

        public static bool DialogQShow(string msg, string img)
        {
            //var mainWindow = Window.GetWindow(this) as Unowhy_Tools_WPF.Views.Container;
            Write2Log("DialogQ: " + msg + " " + img);
            var mainWindow = System.Windows.Application.Current.MainWindow as Unowhy_Tools_WPF.Views.Container;
            if (mainWindow.ShowDialogQ(msg, GetImgSource(img)))
            {
                Write2Log("Result: Yes");
                return true;
            }
            else
            {
                Write2Log("Result: No");
                return false;
            }
        }

        public static void DialogIShow(string msg, string img)
        {
            Write2Log("DialogI: " + msg + " " + img);
            var mainWindow = System.Windows.Application.Current.MainWindow as Unowhy_Tools_WPF.Views.Container;
            mainWindow.ShowDialogI(msg, GetImgSource(img)) ;
        }

        public static BitmapImage GetImgSource(string resname)
        {
            BitmapImage bmp = new BitmapImage(new System.Uri("pack://application:,,,/Resources/" + resname));
            return bmp;
        }

        public class Data : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private static string _hostname;
            private static string _userid;
            private static string _user;
            private static string _sn;
            private static string _mf;
            private static string _md;
            private static string _os;
            private static string _bios;
            private static bool _admin;
            private static bool _aad;
            private static bool _aaduser;
            private static bool _bcd;
            private static bool _entuser;
            private static bool _whe;
            private static bool _bim;
            private static bool _shellok;
            private static bool _tistartup;
            private static bool _hsmexist;
            private static bool _hsmenabled;
            private static bool _hsmrunning;
            private static bool _hsmexeexist;
            private static bool _ridffolderexist;
            private static bool _entfolderexist;
            private static bool _oemfolderexist;
            private static bool _tifolderexist;
            private static bool _hsqmfolderexist;
            private static bool _hsqfolderexist;
            private static bool _winre;

            public string HostName 
            {
                get { return _hostname; }
                set
                {
                    _hostname = value;
                    OnPropertyChanged();
                }
            }
            public string UserID
            {
                get { return _userid; }
                set
                {
                    _userid = value;
                    OnPropertyChanged();
                }
            }
            public string User
            {
                get { return _user; }
                set
                {
                    _user = value;
                    OnPropertyChanged();
                }
            }
            public string sn
            {
                get { return _sn; }
                set
                {
                    _sn = value;
                    OnPropertyChanged();
                }
            }
            public string mf
            {
                get { return _mf; }
                set
                {
                    _mf = value;
                    OnPropertyChanged();
                }
            }
            public string md
            {
                get { return _md; }
                set
                {
                    _md = value;
                    OnPropertyChanged();
                }
            }
            public string os
            {
                get { return _os; }
                set
                {
                    _os = value;
                    OnPropertyChanged();
                }
            }
            public string bios
            {
                get { return _bios; }
                set
                {
                    _bios = value;
                    OnPropertyChanged();
                }
            }
            public bool Admin
            {
                get { return _admin; }
                set
                {
                    _admin = value;
                    OnPropertyChanged();
                }
            }
            public bool AAD
            {
                get { return _aad; }
                set
                {
                    _aad = value;
                    OnPropertyChanged();
                }
            }
            public bool AADUser
            {
                get { return _aaduser; }
                set
                {
                    _aaduser = value;
                    OnPropertyChanged();
                }
            }
            public bool BCD
            {
                get { return _bcd; }
                set
                {
                    _bcd = value;
                    OnPropertyChanged();
                }
            }
            public bool ENTUser
            {
                get { return _entuser; }
                set
                {
                    _entuser = value;
                    OnPropertyChanged();
                }
            }
            public bool WHE
            {
                get { return _whe; }
                set
                {
                    _whe = value;
                    OnPropertyChanged();
                }
            }
            public bool BIM
            {
                get { return _bim; }
                set
                {
                    _bim = value;
                    OnPropertyChanged();
                }
            }
            public bool ShellOK
            {
                get { return _shellok; }
                set
                {
                    _shellok = value;
                    OnPropertyChanged();
                }
            }
            public bool TIStartup
            {
                get { return _tistartup; }
                set
                {
                    _tistartup = value;
                    OnPropertyChanged();
                }
            }
            public bool HSMExist
            {
                get { return _hsmexist; }
                set
                {
                    _hsmexist = value;
                    OnPropertyChanged();
                }
            }
            public bool HSMEnabled
            {
                get { return _hsmenabled; }
                set
                {
                    _hsmenabled = value;
                    OnPropertyChanged();
                }
            }
            public bool HSMRunning
            {
                get { return _hsmrunning; }
                set
                {
                    _hsmrunning = value;
                    OnPropertyChanged();
                }
            }
            public bool HSMExeExist
            {
                get { return _hsmexeexist; }
                set
                {
                    _hsmexeexist = value;
                    OnPropertyChanged();
                }
            }
            public bool RIDFFolderExist
            {
                get { return _ridffolderexist; }
                set
                {
                    _ridffolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool ENTFolderExist
            {
                get { return _entfolderexist; }
                set
                {
                    _entfolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool OEMFolderExist
            {
                get { return _oemfolderexist; }
                set
                {
                    _oemfolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool TIFolderExist
            {
                get { return _tifolderexist; }
                set
                {
                    _tifolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool HSQMFolderExist
            {
                get { return _hsqmfolderexist; }
                set
                {
                    _hsqmfolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool HSQFolderExist
            {
                get { return _hsqfolderexist; }
                set
                {
                    _hsqfolderexist = value;
                    OnPropertyChanged();
                }
            }
            public bool WinRE
            {
                get { return _winre; }
                set
                {
                    _winre = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}