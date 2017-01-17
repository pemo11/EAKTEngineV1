// ========================================================
// File: AppInfo.cs
// ========================================================

using System;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Stellt allgemeine Angaben für die App zur Verfügung
    /// </summary>
    public class AppInfo
    {
        public string EAktName { get; set; }
        public string TempPath { get; set; }
        public string IndexPath { get; set; }

        public string FilterPath { get; set; }
        public string AppVersion { get; set; }

        public string AppName { get; set; }
    }
}
