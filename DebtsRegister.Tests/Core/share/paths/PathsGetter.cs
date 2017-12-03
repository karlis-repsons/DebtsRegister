using System;
using System.IO;

namespace DebtsRegister.Tests.Core
{
    public static class PathsGetter
    {
        public static string ConfigurationRoot {
            get {
                if (configurationRoot == null) {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    configurationRoot = Path.Combine(basePath, "configuration");
                }

                return configurationRoot;
            }
        }


        private static string configurationRoot;
    }
}