﻿using System;
using System.Reflection;
using FCSCommon.Utilities;
using FCSDemo.Buildables;
using FCSDemo.Configuration;
using QModManager.API.ModLoading;

namespace FCSDemo
{
    [QModCore]
    public class QPatch
    {
        internal static ConfigFile Configuration { get; private set; } = new ConfigFile();

        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version: {QuickLogger.GetAssemblyVersion(Assembly.GetExecutingAssembly())}");

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
            QuickLogger.Debug("Debug logs enabled");
#endif
            Configuration = Mod.LoadConfiguration();
            FCSDemoBuidable.PatchHelper();

            try
            {
                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
