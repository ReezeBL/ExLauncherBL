using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ExLauncherBL
{
    class JavaLoader
    {
        XElement config;
        string folder;
        readonly string exjava = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/exjava/jvm/bin/java";
        readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        readonly string myjava = Environment.CurrentDirectory + "/Java_64/bin/java.exe";

        public JavaLoader(XDocument config, string folder)
        {
            Console.WriteLine(config);
            XElement root = config.Element("ex-servers");
            this.config = root?.Elements("server").FirstOrDefault(e => e.Element("dir")?.Value == folder);
            this.folder = "%appdata%/.exclient/" + folder;
        }

        public void Load(String username, String session, String uid)
        {
            string javaLibPath = "-Djava.library.path=\""+folder+"/bin/natives\"";
            string extraArguments = config?.Element("extra-arguments")?.Value.Replace("@RAM@", "2048M");
            string unknownShit = "-XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy";
            string arguments = config?.Element("arguments")?.Value.Replace("@SESSION@", session).Replace("@USER@", username);
            string version = config?.Element("version")?.Value;
            string mainClass = config?.Element("main-class")?.Value;
            StringBuilder classPath = new StringBuilder();
            Array.ForEach(config?.Element("class-path")?.Elements("path").ToArray(), e =>
                classPath.Append($"{folder}/{e.Value};"));
            //C:\Users\Шилкин\AppData\Roaming\exjava\jvm\bin\java.exe -Xmx1024m -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.library.path=C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\natives -cp C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exblforforge.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exblforge.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exauth.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\liteloader.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\excomplet.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\hitech.jar net.minecraft.launchwrapper.Launch --accessToken 3f564062e975286ae1edca1609b77a18 --username Siamant --session 3f564062e975286ae1edca1609b77a18 --tweakClass cpw.mods.fml.common.launcher.FMLTweaker --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --gameDir C:\Users\Шилкин\AppData\Roaming\.exclient\hitech --version 1.7.10 --assetsDir C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\assets --uuid 780e469a6f64172c834a8d2de068918c --userProperties {} --assetIndex 1.7.10
            string javaParams = $"%appdata%\\exjava\\jvm\\bin\\java.exe {extraArguments} {unknownShit} {javaLibPath} -cp \"{classPath}\" {mainClass} {arguments} --version {version} --gameDir {folder} --assetsDir {folder}/assets --uuid {uid} --userProperties {{}} --assetIndex {version}";

            StringBuilder toFile = new StringBuilder();
            toFile.AppendLine($"cd {folder}");
            toFile.AppendLine(javaParams);
            toFile.AppendLine("pause");

            File.WriteAllText("start.bat",toFile.ToString());
            Process.Start("start.bat");         
        }
    }
}
