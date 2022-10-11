using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
 
[assembly: AssemblyTitle("Player")]
[assembly: AssemblyDescription("Minimal mp3 player")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Andrei Vasilev")]
[assembly: AssemblyProduct("Minerva")]
[assembly: AssemblyCopyright("©  Andrei Vasilev")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
 
[assembly: ComVisible(false)]
 
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,  
    ResourceDictionaryLocation.SourceAssembly 
)]

[assembly: AssemblyVersion("0.0.2.*")] 

[assembly: InternalsVisibleTo("Player.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // Moq
