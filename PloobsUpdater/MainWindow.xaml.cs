﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FTPLib;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.InteropServices;
using NUnrar.Archive;

namespace PloobsUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool isConnected = false;
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private extern static bool InternetGetConnectedState(ref InternetConnectionState_e lpdwFlags, int dwReserved);
        [Flags]
        enum InternetConnectionState_e : int { INTERNET_CONNECTION_MODEM = 0x1, INTERNET_CONNECTION_LAN = 0x2, INTERNET_CONNECTION_PROXY = 0x4, INTERNET_RAS_INSTALLED = 0x10, INTERNET_CONNECTION_OFFLINE = 0x20, INTERNET_CONNECTION_CONFIGURED = 0x40 }   

        public MainWindow()
        {
            MySqlCommonConnection.WriteClientToDB("tes");

            InitializeComponent();
            this.Hide();

            HandleXnaVersion();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(3, 0,0 );
            dispatcherTimer.Start();

            InternetConnectionState_e flags = 0;
            isConnected = InternetGetConnectedState(ref flags, 0);

            GetVersionOnRegistry();

            if (isConnected == false)
                return;

            try
            {
                AvaliableVersions.Clear();
                ftp = new FTP("ftp.ploobs.com.br", "ploobs", "5ruTrus6");
                ftp.Connect();
                ftp.ChangeDir("/ploobs/Web/Updater");

                foreach (String item in ftp.List())
                {
                    String[] files = item.Split(' ');
                    String file = files[files.Count() - 1];
                    AvaliableVersions.Add(file);
                    listBox1.Items.Add(file);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (packageName != null)
            {
                label2.Content = "CurrentVersion: " + packageName;
            }

            if (AvaliableVersions.Count == 0)
            {
                button1.IsEnabled = false;
                button2.IsEnabled = false;
            }
   
        }


        /// <summary>
        /// TICK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            InternetConnectionState_e flags = 0;
            isConnected = InternetGetConnectedState(ref flags, 0);

            if (isConnected == false)
                return;

            String oldLast = AvaliableVersions[AvaliableVersions.Count - 1];

            try
            {
                
                AvaliableVersions.Clear();
                listBox1.Items.Clear();

                ftp = new FTP("ftp.ploobs.com.br", "ploobs", "5ruTrus6");
                ftp.Connect();
                ftp.ChangeDir("/ploobs/Web/Updater");

                foreach (String item in ftp.List())
                {
                    String[] files = item.Split(' ');
                    String file = files[files.Count() - 1];
                    AvaliableVersions.Add(file);
                    listBox1.Items.Add(file);
                }

            }
            catch (Exception ex)
            {
                ///do not show message box here ....
            }

            if (oldLast != AvaliableVersions[AvaliableVersions.Count - 1])
            {
                string title = "PloobsUpdates";
                string text = "There is a new Version of PloobsEngine Avaliable";

                //show balloon with built-in icon
                this.MyNotifyIcon.ShowBalloonTip(title, text, BalloonIcon.Info);
                this.MyNotifyIcon.TrayBalloonTipClicked += new RoutedEventHandler(MyNotifyIcon_TrayBalloonTipClicked);
            }

            if (AvaliableVersions.Count == 0)
            {
                button1.IsEnabled = false;
                button2.IsEnabled = false;
            }

        }

        void MyNotifyIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            this.Show();            
        }

        String packageName = null;
        List<String> AvaliableVersions = new List<string>();
        FTP ftp = null;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            HandleXnaVersion();

            if (isConnected == false)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }

            try
            {
                String sitem = listBox1.SelectedItem as String;
                if (sitem != packageName)
                {
                    DeleteCurrentEngineVersion();
                    Random rd = new Random();
                    int rdnum = rd.Next();
                    
                    ftp.ChangeDir("/ploobs/Web/Updater/" + sitem);
                    String temppath = System.IO.Path.GetTempPath() + "PloobsEngine" + rdnum + ".rar";
                    ftp.OpenDownload("PloobsEngine.rar", temppath, true);

                    while (ftp.DoDownload() != 0)
                    {
                        Thread.Sleep(500);
                    }

                    try
                    {
                        RarArchive.WriteToDirectory(temppath, System.IO.Path.GetTempPath(),NUnrar.Common.ExtractOptions.ExtractFullPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }

                    temppath = System.IO.Path.GetTempPath() + "PloobsEngine//setup.exe";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = temppath;
                    process.Start();
                    process.WaitForExit();

                    packageName = sitem;
                    if (packageName != null)
                    {
                        label2.Content = "CurrentVersion: " + packageName;
                    }

                    MySqlCommonConnection.WriteClientToDB(sitem);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DeleteCurrentEngineVersion()
        {            
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "msiexec.exe /x {2AF6A123-A362-4E2B-B23A-D1E952C15A9C}";
            process.Start();
            process.WaitForExit();
        }
        
        
        private void TaskbarIcon_TrayRightMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();            
        }


        private void HandleXnaVersion()
        {
            if (GetXNAVersion() == false)
            {
                MessageBox.Show("You dont have the XNA 4.0 version installed, pls install it from " + @"http://www.microsoft.com/download/en/details.aspx?id=23714");
                Application.Current.Shutdown();
            }
        }

        private bool GetXNAVersion()
        {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\XNA\\Framework\\v4.0", false);
                if (myKey != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }        






        private void GetVersionOnRegistry()
        {
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\v4.0.30319\\AssemblyFoldersEx\\PloobsEngine", false);
            if (myKey != null)
            {
                String o = myKey.GetValue(null) as String;
                if (o != null)
                {

                    foreach (var item in Directory.EnumerateFiles(o))
                    {
                        if (item.EndsWith(".dll") && item.Contains("PloobsEngineDebug"))
                        {
                            //PloobsEngineDebug, Version=0.0.0.1, Culture=neutral, PublicKeyToken=0c21691816f8c6d0
                            Assembly Assembly = Assembly.LoadFile(item);
                            String name = Assembly.FullName.Split(',')[0];
                            String version = Assembly.FullName.Split(',')[1].Split('=')[1];
                            packageName = version;
                            break;
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected == false)
            {
                MessageBox.Show("No Internet Connection");
                return;
            }

            try
            {
                Random rd = new Random();
                int rdnum = rd.Next();
                String sitem = listBox1.SelectedItem as String;
                ftp.ChangeDir("/ploobs/Web/Updater/" + sitem);
                String temppath = System.IO.Path.GetTempPath() + "Change" + rdnum + ".pdf";
                ftp.OpenDownload("Changes.pdf", temppath, true);

                while (ftp.DoDownload() != 0)
                {
                }

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = temppath;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
