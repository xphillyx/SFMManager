﻿using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SourceFilmMakerManager {

    public partial class Splash_scn : System.Windows.Forms.Form {
        //private string AUTOCHECKUPDATE = ConfigurationManager.AppSettings["Auto_Check_For_Updates"];
        private IniFile configIni = new IniFile("config.ini");
        private float CurrentVer;
        public bool CanCheckUpdate;
        public bool onceUpdate = true;

        public Splash_scn() {
            InitializeComponent();
            configCreate();
            //Create SFM\PuCE Folder.
            FolderCreate();
            Migrate();
            
            onceUpdate = true;
        }

        private void Splash_Load(object sender, System.EventArgs e) {
        }

        private void Migrate() {
            if (Directory.Exists(@"SFM\PuCE\")) {
                Console.WriteLine(@"Found SFM\PuCE, Migrating");
                DirectoryInfo d = new DirectoryInfo("SFM\\PuCE\\");
                FileInfo[] infos = d.GetFiles();
                foreach (FileInfo f in infos) {
                    if (f.Name.EndsWith(".PuCE")) {
                        var fileName = f.Name.Replace(".PuCE", "");
                        Console.WriteLine("Migrating "+ fileName);
                        Directory.CreateDirectory(@"SFM\SFMM\" + fileName);
                        Directory.Move(f.FullName, @"SFM\SFMM\" + fileName + @"\" + fileName + ".SFMM");
                        var infoIni = new IniFile(@"SFM\SFMM\" + fileName + @"\" + "info.ini");
                        infoIni.Write("Date", f.LastWriteTime.ToString());
                        infoIni.Write("Category", "?");
                        infoIni.Write("Source", "?");
                        infoIni.Write("Author", "?");
                        infoIni.Write("URL", "?");
                    }
                }
                Directory.Delete(@"SFM\PuCE");
            }
        }

        private void configCreate() {
            if (!File.Exists("config.ini")) { 
            var infoIni = new IniFile("config.ini");
            infoIni.Write("DownloadServer", "1");
            infoIni.Write("SFMPATH", @"C:\Program Files (x86)\Steam\steamapps\common\SourceFilmmaker\game\usermod");
            infoIni.Write("AutoUpdate", "true");
            }
        }

        public void FolderCreate() {
            var dirPuCE = @"SFM\SFMM\";  // folder location
            var dirManager = @"SFM\Manager\";  // folder location
            var dirDownload = @"SFM\Download\";  // folder location

            if (!Directory.Exists(dirPuCE)) { // if it doesn't exist, create
                Directory.CreateDirectory(dirPuCE);
            }
            else {
                Console.WriteLine(random.LG);
            }
            if (!Directory.Exists(dirManager)) { // if it doesn't exist, create
                Directory.CreateDirectory(dirManager);
            }
            if (!Directory.Exists(dirDownload)) { // if it doesn't exist, create
                Directory.CreateDirectory(dirDownload);
            }
        }

        public void VersionCheck() {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true) {
                WebClient wc = new WebClient();
                try {
                    string Ver = wc.DownloadString("http://sfmm.merubokkusu.com/version.txt");

                    //
                    //CurrentVer is set to the current version....
                    //
                    if (CurrentVer < float.Parse(Ver)) {
                        DialogResult result1 = MessageBox.Show("Go To The Download Page?", "New Version Available", MessageBoxButtons.YesNo);
                        if (result1 == DialogResult.Yes) {
                            Console.Write("Going To SFMMLab");
                            System.Diagnostics.Process.Start("https://sfmlab.com/item/1297/");
                            //Run
                            WindowsFormsApplication1.Form1.SplashOver = true;
                            this.Close();
                        }
                        else {
                            //Run
                            WindowsFormsApplication1.Form1.SplashOver = true;
                            this.Close();
                        }
                    }
                    else {
                        Console.WriteLine(random.VR);
                        //Run
                        System.Threading.Thread.Sleep(2500);
                        WindowsFormsApplication1.Form1.SplashOver = true;
                        this.Close();
                    }
                }
                catch (System.Net.WebException) {
                    MessageBox.Show("Couldn't check for verison, Update Server Is Down.\n You can disable auto check in the config file");
                }
            }//End Internet Check
            else {
                MessageBox.Show("Couldn't check for verison, Your not connected to the internet.\n You can disable auto check in the config file");
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (CanCheckUpdate == false) {
                Close();
            }
        }

        private void checkTrue() {
            if (onceUpdate == true) {
                onceUpdate = false;
                CurrentVer = WindowsFormsApplication1.Form1.CurrentVer;
                var updateBool = configIni.Read("AutoUpdate");
                bool CheckForUpdate = Convert.ToBoolean(updateBool);

                if (CheckForUpdate == true) {
                    WindowsFormsApplication1.Form1.SplashOver = true;
                    CheckFalse_time.Enabled = false;
                    VersionCheck();
                }
            }
        }

        private void CheckTrue_time_Tick(object sender, EventArgs e) {
            checkTrue();
        }
    }
}