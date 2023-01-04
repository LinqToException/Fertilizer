using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace Fertilizer
{
    public class Program
    {
        private const string InjectorAssemblyName = "Fertilizer.Injector";

        private static bool nonInteractive = false;

        public static void Main(string[] args)
        {
            nonInteractive = args.Contains("--non-interactive");

            var executable = new FileInfo("Kynseed.exe");
            if (!executable.Exists)
                Fail("Could not find Kynseed.exe");

            using var module = ModuleDefinition.ReadModule(executable.FullName, new ReaderParameters
            {
                InMemory = true
            });

            if (module.AssemblyReferences.Any(p => p.Name == InjectorAssemblyName))
                Fail("Game is already patched.");

            var nameReference = new AssemblyNameReference(InjectorAssemblyName, new Version(1, 0, 0, 0));
            var injectorPath = Path.Combine(executable.Directory.FullName, InjectorAssemblyName + ".dll");
            if (!File.Exists(injectorPath))
                Fail($"Could not find {injectorPath}. Did you unzip everything?");

            module.AssemblyReferences.Add(nameReference);

            var program = module.GetType("TopDownEditor.Program");
            if (program == null)
                Fail("Could not find entry type");

            var targetMethod = program!.Methods.FirstOrDefault(p => p.Name == "Main");
            if (targetMethod == null)
                Fail("Could not find Main method");

            var injectorModule = ModuleDefinition.ReadModule(injectorPath);
            var injectorMethod = injectorModule.GetType("Fertilizer.Injector").Methods.First(p => p.Name == "Inject");

            var methodReference = module.ImportReference(injectorMethod);
            targetMethod!.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, methodReference));

            module.Write(executable.FullName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully patched the game!");
            Pause();
        }

        private static void Pause()
        {
            if (!nonInteractive)
            {
                Console.Error.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void Fail(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Pause();
            Environment.Exit(1);
        }
    }
}