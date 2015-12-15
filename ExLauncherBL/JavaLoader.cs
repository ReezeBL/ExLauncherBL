using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExLauncherBL
{
    class JavaLoader
    {
        XElement config;
        string folder;
        readonly string exjava = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/exjava/jvm/bin/java.exe";
        readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        readonly string myjava = Environment.CurrentDirectory + "/Java_64/bin/java.exe";
        public JavaLoader(XDocument config, string folder)
        {
            XElement root = config.Element("ex-servers");
            this.config = root.Elements("server").FirstOrDefault(e => e.Element("dir").Value == folder);
            this.folder = "%appdata%/.exclient/" + folder;
        }

        public void Load(String username, String session, String uid)
        {
            string javaLibPath = "-Djava.library.path=\""+folder+"/bin/natives\"";
            string extraArguments = config.Element("extra-arguments").Value.Replace("@RAM@", "1024M");
            string arguments = config.Element("arguments").Value.Replace("@SESSION@", session).Replace("@USER@", username);
            string version = config.Element("version").Value;
            string mainClass = config.Element("main-class").Value;
            StringBuilder classPath = new StringBuilder();
            Array.ForEach(config.Element("class-path").Elements("path").ToArray(), e => classPath.AppendFormat("{0}/{1};",folder, e.Value));
            //C:\Users\Шилкин\AppData\Roaming\exjava\jvm\bin\java.exe -Xmx1024m -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.library.path=C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\natives -cp C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exblforforge.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exblforge.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\exauth.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\liteloader.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\excomplet.jar;C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\bin\hitech.jar net.minecraft.launchwrapper.Launch --accessToken 3f564062e975286ae1edca1609b77a18 --username Siamant --session 3f564062e975286ae1edca1609b77a18 --tweakClass cpw.mods.fml.common.launcher.FMLTweaker --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --gameDir C:\Users\Шилкин\AppData\Roaming\.exclient\hitech --version 1.7.10 --assetsDir C:\Users\Шилкин\AppData\Roaming\.exclient\hitech\assets --uuid 780e469a6f64172c834a8d2de068918c --userProperties {} --assetIndex 1.7.10
            string javaParams = String.Format("java {0} {1} -cp \"{2}\" {3} {4} --version {5} --gameDir {6} --assetsDir {6}/assets --uuid {7} --userProperties {8} --assetIndex {5}", extraArguments, javaLibPath, classPath, mainClass, arguments, version, folder, uid,"{}");

            StringBuilder toFile = new StringBuilder();
            toFile.AppendLine(String.Format("cd {0}", folder));
            toFile.AppendLine(javaParams);
            toFile.AppendLine("pause");

            File.WriteAllText("start.bat",toFile.ToString());
            Process.Start("start.bat");         
        }
    }
}
