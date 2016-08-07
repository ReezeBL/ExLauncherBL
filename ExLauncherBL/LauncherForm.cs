using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using System.Security.Cryptography;

namespace ExLauncherBL
{
    public partial class LauncherForm : Form
    {
        const string pathSettings = @"Settings.xml";
        const string pathLoginServer = @"http://ex-server.ru/exAuthLogin.php";
        const string pathConfigInfo = @"http://ex-server.ru/configlauncher.xml";
        public LauncherForm()
        {
            InitializeComponent();
        }

        private void LoadSettings()
        {
            try {
                XDocument doc = XDocument.Load(pathSettings);
                XElement loginDetails = doc.Element("LoginDetails");
                if (loginDetails == null)
                    return;
                loginTextBox.Text = loginDetails.Element("Login").Value;
                passwordTextBox.Text = loginDetails.Element("Password").Value;
            }
            catch(Exception) { }
        }
        private void SaveSettings()
        {
            XDocument doc = new XDocument();
            XElement loginDetails = new XElement("LoginDetails");
            XElement login = new XElement("Login");
            XElement password = new XElement("Password");
            login.Value = loginTextBox.Text;
            password.Value = passwordTextBox.Text;
            loginDetails.Add(login, password);
            doc.Add(loginDetails);
            doc.Save(pathSettings);
        }
        private void LauncherForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
        private void LauncherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private string GeneratePrameters()
        {
            string Username = loginTextBox.Text;
            string Password = passwordTextBox.Text.Replace("+", "%2B").Replace("=", "%3D");
            string GuID = Encoding.UTF8.GetString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Username)));
            return "user=" + Username + "&pass=" + Password + "&guid=" + GuID + "&key=";
        }

        private static string GeneratePrameters(string username, string password)
        {
            password = password.Replace("+", "%2B").Replace("=", "%3D");
            var guId = Encoding.UTF8.GetString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(username)));
            return $"user={username}&pass={password}&guid={guId}&key=";
        }


        private void connectButton_Click(object sender, EventArgs e)
        {
            var response = Connector.POSTurl(pathLoginServer, GeneratePrameters());
            XElement authToken = response.Element("ex-auth");
            if (authToken.Element("error").Value == "true")
                MessageBox.Show("Invalid login or password!");
            else
            {
                string session = authToken.Element("session").Value;
                string uuid = authToken.Element("uuid").Value;
                XDocument config = Connector.GETurl(pathConfigInfo);
                new JavaLoader(config, "hitech").Load(loginTextBox.Text, session, uuid);
            }
        }

        public static XDocument GetServerResponse(string username, string password)
        {
            XDocument response = Connector.POSTurl(pathLoginServer, GeneratePrameters(username, password));
            return response;
        }
    }
}
