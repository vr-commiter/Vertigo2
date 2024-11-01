using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(Vertigo2_TrueGear.BuildInfo.Description)]
[assembly: AssemblyDescription(Vertigo2_TrueGear.BuildInfo.Description)]
[assembly: AssemblyCompany(Vertigo2_TrueGear.BuildInfo.Company)]
[assembly: AssemblyProduct(Vertigo2_TrueGear.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + Vertigo2_TrueGear.BuildInfo.Author)]
[assembly: AssemblyTrademark(Vertigo2_TrueGear.BuildInfo.Company)]
[assembly: AssemblyVersion(Vertigo2_TrueGear.BuildInfo.Version)]
[assembly: AssemblyFileVersion(Vertigo2_TrueGear.BuildInfo.Version)]
[assembly: MelonInfo(typeof(Vertigo2_TrueGear.Vertigo2_TrueGear), Vertigo2_TrueGear.BuildInfo.Name, Vertigo2_TrueGear.BuildInfo.Version, Vertigo2_TrueGear.BuildInfo.Author, Vertigo2_TrueGear.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]