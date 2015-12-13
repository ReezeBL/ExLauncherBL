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
        String folder;
        public JavaLoader(XDocument config, string folder)
        {
            XElement root = config.Element("ex-servers");
            this.config = root.Elements("server").FirstOrDefault(e => e.Element("dir").Value == folder);
            this.folder = "%appdata%/.exclient/" + folder;
        }

        public void Load(String username, String session, String uid)
        {
            string javaLibPath = "-Djava.library.path =\"" + folder + "/bin/natives\"";
            string extraArguments = config.Element("extra-arguments").Value.Replace("@RAM@", "512");
            string arguments = config.Element("arguments").Value.Replace("@SESSION@", session).Replace("@USER@", username);
            StringBuilder classPath = new StringBuilder();
            Array.ForEach(config.Elements("path").ToArray(), e => classPath.AppendFormat("\"{0}/{1}\";", folder, e.Value));
            File.WriteAllText("test.bat", String.Format("{0} {1} -cp {2} {3}", extraArguments, javaLibPath, classPath, arguments));
            Process.Start("java.exe", String.Format("{0} {1} -cp {2} {3}", extraArguments, javaLibPath, classPath, arguments));
        }
    }
}
