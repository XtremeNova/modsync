﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace modsync
{
    static class Locations
    {
        // locations on local pc
        public static string LocalFolderName_Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string LocalFolderName_Roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string LocalFolderName_Minecraft = LocalFolderName_Roaming + "\\.minecraft";
        public static string LocalFolderName_TempDir = LocalFolderName_Minecraft;
        public static string LocalFolderName_Versions = LocalFolderName_Minecraft + "\\versions";
        public static string LocalFolderName_Launcher = LocalFolderName_Minecraft + "\\minecraft launcher";
        public static string LocalFolderName_Saves = LocalFolderName_Minecraft + "\\saves";
        public static string LocalFolderName_SavesBackup = LocalFolderName_Minecraft + ".saves";
        public static string Launcher = LocalFolderName_Launcher + "\\Minecraft Launcher.exe";
        public static string Launcher_Jar = LocalFolderName_Launcher + "\\game\\launcher.jar";
        public static string LauncherProfiles = LocalFolderName_Minecraft + "\\launcher_profiles.json";
        public static string ServersFile = LocalFolderName_Minecraft + "\\servers.dat";
        public static string Java; //set from registery
        public static string Javaw; //set from registery

        static string ProgramFilesx86()
        {
            if ((IntPtr.Size == 8) || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
    }
}
