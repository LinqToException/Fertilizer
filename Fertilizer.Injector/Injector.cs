using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Fertilizer
{
    public static class Injector
    {
        public static void Inject()
        {
            using var fs = new FileStream("fertilizer.log", FileMode.Create, FileAccess.Write);
            using var sw = new StreamWriter(fs);

            void Log(string level, string message)
                => sw!.WriteLine($"[{DateTime.Now} {level}] {message}");

            void Info(string message)
                => Log("INF", message);

            void Error(string message)
                => Log("ERR", message);

            Info($"Initializing {typeof(Injector).Assembly.GetName().FullName}...");

            var modsDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "mods"));

            if (!modsDirectory.Exists)
            {
                Info("Mods directory did not exist; created it.");
                modsDirectory.Create();
            }

            List<Mod> mods = new();
            foreach (var dir in modsDirectory.EnumerateDirectories())
            {
                var modsFile = dir.GetFiles(dir.Name + ".dll");
                if (modsFile.Length == 0)
                {
                    Error($"Did not load {dir.Name}: No matching assembly ({dir.Name}.dll) found.");
                    continue;
                }

                try
                {
                    Info($"Load {dir.Name}...");
                    var assembly = Assembly.LoadFrom(modsFile[0].FullName);

                    foreach (var modType in assembly.GetExportedTypes())
                    {
                        if (typeof(Mod).IsAssignableFrom(modType) && modType.IsClass && !modType.IsAbstract)
                        {
                            Info($"Instantiate {dir.Name}/{modType.FullName}...");
                            var instance = (Mod)Activator.CreateInstance(modType);
                            instance.OnEnable();
                            mods.Add(instance);
                            Info($"{dir.Name}/{modType.FullName} enabled.");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Error($"Could not load {dir.Name}!");
                    foreach (var loaderException in ex.LoaderExceptions)
                        Error($"Loader exception: {loaderException}");
                    continue;
                }
                catch (Exception ex)
                {
                    Error($"Could not load {dir.Name}: {ex}");
                    continue;
                }
            }

            Info($"Loaded {mods.Count} mods.");
        }
    }
}