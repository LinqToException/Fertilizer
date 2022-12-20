**Fertilizer** is a mod loader for [Kynseed](https://store.steampowered.com/app/758870/Kynseed/) inspired by IPA. It consists of three parts:

- **Fertilizer** itself is a game patcher. It will modify the game's executable to call the injector.
- **Fertilizer.Injector** is the actual mod loader. It will load mods that are inside the `mods/` folder.
- **Fertilizer.Mod** is the interface mods have to provide in order to be loaded.

# What Is Fertilizer?
Right now, Fertilizer is just a mod (to be more specific: assembly) loader. Due to this, it's not a complete modding toolkit, nor is it even remotely comparable to those existing for similar games. It has one job, and that is to load assemblies.

That is not to say that it's not possible for it to head down this path - just that it isn't doing it at the moment and there are no real plans to add any of this functionality either right now.

# Getting Started
The installation process is kept relatively simple for users:

1. Download the current release (not the source code!) from the [releases page](https://github.com/LinqToException/Fertilizer/releases/latest).
2. Unzip the contents of the archive into your Kynseed directory. If you're not sure where Kynseed has been installed, on Steam you can open its folder by right-clicking the game in the library, selecting _Properties_, then _Local Files_, then clicking on _Browse..._.
3. Double click the Fertilizer.exe that you've just extracted inside the Kynseed-Folder. A command prompt ("black window") will pop up. If everything is going according to plan, it'll tell you that the game has been patched in a green font. If something went wrong, it'll tell you what in a red font.
4. Put any mods you wish to install into the `mods/` directory inside your Kynseed folder - this folder is not created by the game itself. Currently, the assembly name of a mod and the folder must match in order for it to be loaded, e.g. `mods/Fertilizer.Example/Fertilizer.Example.dll`.

**Important:** Step 3 has to be repeated after every game update and after verifying the game files. Failure to do so will result in no mods being loaded.

Fertilizer creates a `fertilizer.log` file in the same folder. It is used by the injector to log information about what mods are loaded and what went wrong while loading mods. When having mod trouble, take a look inside and/or send that log file to the mod developer.

# For Developers
Developing mods with/for Fertilizer is somewhat straight forward. There's the basic requirements and the slightly more advanced setup:

## Basic Setup
This is the more-or-less minimum requirement to get a mod working with Fertilizer. It's not the most comfortable one.
1. Create a new .NET 4.5 or higher library. Fertilizer itself uses .NET 4.6 because 4.6 or higher can be reasonably well be expected to be installed on machines nowadays.
1. Reference `Fertilizer.Mod`, either as csproj or the compiled assembly in the game directory.
1. Reference whatever else you need - usually this will include `Kynseed` and perhaps `MonoProtoFramework`. Note that a lot of Kynseed's logic is `internal`, so you'll need to work around that by either reflection or something like [aelji's IgnoreAccessChecksToGenerator](https://github.com/aelij/IgnoresAccessChecksToGenerator).
1. Create a new class that inherits from `Fertilizer.Mod`. Implement your mod's logic in `OnEnable`. While you can also use the constructor, for future-proofness it's recommended to do all game-changing logic in `OnEnable`.
1. Compile your mod and put it into the `mods/` folder inside your Kynseed directory. The folder name and the assembly name must match in order for Fertilizer to load it.
1. Once done, you can zip up the folder of your mod and distribute it.

## Advanced Setup
This setup is taking advantage of some options MSBuild offers to simplify the modding process. `ExampleMod/AutoPrice` is the example, minimal setup for a mod plus one Harmony patch.

1. Create a new solution.
1. Copy the `Directory.Build.props`, `Directory.Build.targets` and `Paths.user.example` from this repository in `ExampleMod/` to your mod's solution directory. Re-start Visual Studio once done. This only has to be done once.
1. Rename the `Paths.user.example` to `Paths.user` and adjust the settings inside for you.
1. Probably remove or change the `<Version>`-element in the `Directory.Build.props`. This file is more or less providing default values for all .csprojs inside the solution.
1. Create a new .NET 4.5 or higher library. Fertilizer itself uses .NET 4.6 because 4.6 or higher can be reasonably well be expected to be installed on machines nowadays.
1. Reference `Fertilizer.Mod`, either as csproj or the compiled assembly in the game directory. When you want to reference the compiled assembly, you can use `<GameAssembly Include="Fertilizer.Mod" />` to do so.
1. In order for the `internal`-bypass to work, you'll also need to reference the `IgnoresAccessChecksToGenerator` NuGet.
1. Reference whatever other game assembly you require to reference using `<GameAssembly Include="..." />`.
1. Set the project's `OutDir` to `$(GameDir)/mods/$(AssemblyName)` to put your mod's files in the right directory straight away. Unless you have a non-mod project in your solution, you can move this instruction to the `Directory.Build.props`.
1. Create a new class that inherits from `Fertilizer.Mod`. Implement your mod's logic in `OnEnable`. While you can also use the constructor, for future-proofness it's recommended to do all game-changing logic in `OnEnable`.

`<GameAssembly>` is a way of referencing game assemblies which is both independent of the installation folder (thanks to the Paths.user), and has the additional benefit of automatically registering the assembly as a target for `IgnoreAccessChecksTo` if the generator has been installed. In order for this setup to work, at least the `Directory.Build.targets` and `Paths.user` has to exist at a solution level.