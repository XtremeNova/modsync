﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using EnterpriseDT.Net.Ftp;

namespace modsync
{
    class Program
    {
        #region Variables

        static FtpLib ftp;
        static FTPConnection ftpcon;

        #endregion

        #region Main

        static void Main(string[] args)
        {
            // check for clickonce updates
            if (Update.ClickOnceUpdate())
            {
                return;
            }
            
            // add embedded dll
            Resources.RegisterEmbeddedResources();
            
            // start program
            MainLoad(args);
        }

        static void MainLoad(string[] args)
        {
            // create directory if missing
            if (!Directory.Exists(Locations.LocalFolderName_Minecraft))
            {
                Directory.CreateDirectory(Locations.LocalFolderName_Minecraft);
            }

            // read ftp link from file
            Config.FtpLinkRead();

            // parse simple command line options
            bool push = false;
            foreach (string arg in args)
            {
                if (arg == "updateserver")
                {
                    push = true;
                }
                else if (arg.StartsWith("ftp://"))
                {
                    Config.FtpLinkParse(arg);
                }
            }

            // require ftp server from here
            // have user input the details if required
            if (Config.ftpsettings.FtpServerAddress == "")
            {
                Config.FtpLinkInput();
            }

            // open ftp
            if (!FtpOpen())
            {
                return;
            }

            // update config from server
            Config.FtpUpdate(ref ftpcon);

            // check for executable updates
            Update.ExecutableUpdate(ref ftpcon, args);

            // handle push
            if (push)
            {
                Mods.Push(ref ftp);
                Exit(false);
            }

            // check for java
            Java.Check(ref ftpcon);

            // check minecraft
            Minecraft.Check(ref ftpcon);

            // check forge
            Forge.Check(ref ftpcon);

            // sync folders
            Mods.Pull(ref ftp);

            // update servers file
            Servers.Check();

            // exit launching game
            Exit(true);
        }

        public static void Exit(bool launchgame)
        {
            // disconnect
            if (ftp != null)
            {
                if (ftp.IsConnected())
                {
                    ftp.DisConnect();
                }
            }

            if (launchgame)
            {
                // start extracted launcher with desired java version if possible
                string jar = "";
                if (File.Exists(Locations.Launcher_Install_Jar))
                {
                    jar = Locations.Launcher_Install_Jar;
                }
                else if (File.Exists(Locations.Launcher_Download_Jar))
                {
                    jar = Locations.Launcher_Download_Jar;
                }
                if (File.Exists(Locations.Javaw) && (jar != ""))
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = Locations.Javaw;
                    psi.WorkingDirectory = Path.GetDirectoryName(jar);
                    psi.Arguments = "-cp \"" + jar + "\" net.minecraft.launcher.Main";
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                }
                // start installed launcher
                else if (File.Exists(Locations.Launcher_Install))
                {
                    Process.Start(Locations.Launcher_Install);
                }
                // start downloaded launcher
                else if (File.Exists(Locations.Launcher_Download))
                {
                    Process.Start(Locations.Launcher_Download);
                }
            }
            Environment.Exit(0);
        }

        #endregion

        #region FTP

        static bool FtpOpen()
        {
            // create connection
            try
            {
                ftpcon = new FTPConnection();
                ftpcon.UserName = Config.ftpsettings.FtpUserName;
                ftpcon.Password = Config.ftpsettings.FtpPassword;
                ftpcon.ServerAddress = Config.ftpsettings.FtpServerAddress;
                ftpcon.ServerPort = int.Parse(Config.ftpsettings.FtpServerPort);
                ftpcon.TransferNotifyInterval = (long)4096;
                ftp = new FtpLib(ref ftpcon);
            }
            catch (Exception)
            {
                Console.WriteLine(Strings.Get("FtpBadConfig"));
                Console.ReadKey();
                return false;
            }

            // try to connect
            try
            {
                ftp.LogIn();
            }
            catch (Exception)
            {
                Console.WriteLine(Strings.Get("FtpNoConnection"));
                Console.ReadKey();
                return false;
            }

            // check if folder exists
            if (!Config.ftpsettings.FtpServerFolder.StartsWith("/"))
            {
                Config.ftpsettings.FtpServerFolder = "/" + Config.ftpsettings.FtpServerFolder;
            }
            if (!ftpcon.DirectoryExists(Config.ftpsettings.FtpServerFolder))
            {
                Console.WriteLine(Strings.Get("FtpBadFolder") + Config.ftpsettings.FtpServerFolder);
                Console.ReadKey();
                return false;
            }
            return true;
        }

        #endregion
    }
}
