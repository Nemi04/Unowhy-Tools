﻿using Wpf.Ui.Common.Interfaces;
using Unowhy_Tools_WPF.ViewModels;
using System.Windows;

using Unowhy_Tools;
using System.Diagnostics;
using System;
using Microsoft.Win32;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.IO.Pipes;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Unowhy_Tools_WPF.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;
using System.Net.Http;
using System.Security.Policy;

namespace Unowhy_Tools_WPF.Views.Pages;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Wifi : INavigableView<DashboardViewModel>
{
    UT.Data UTdata = new UT.Data();

    public DashboardViewModel ViewModel
    {
        get;
    }

    public void GoForw(object sender, RoutedEventArgs e)
    {
        UT.anim.TransitionForw(RootGrid);
    }

    public async void Init(object sender, EventArgs e)
    {
        
    }

    public void applylang()
    {

    }

    public Wifi(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();
    }

    /*public async void Get_Click(object sender, RoutedEventArgs e)
    {
        WebClient wc = new WebClient();
        string sn = serial.Text;
        string g = wc.DownloadString($"https://idf.hisqool.com/conf/devices/{sn}/configuration");
        string jsonString = g;
        List<dynamic> dataList = JsonConvert.DeserializeObject<List<dynamic>>(jsonString);

        List<string> urlList = new List<string>();

        foreach (var data in dataList)
        {
            string url = data.url;
            urlList.Add(url);
        }

        List<string> jsonResponseList = new List<string>();

        JObject mergedJson = new JObject();

        foreach (var url in urlList)
        {
            var client = new WebClient();
            JObject json = JObject.Parse(client.DownloadString(url));
            mergedJson.Merge(json);
        }



        DataTable dt = new DataTable();
        dt.Columns.Add("SSID", typeof(string));
        dt.Columns.Add("Password", typeof(string));
        dt.Columns.Add("Security Type", typeof(string));
        dt.Columns.Add("Hidden", typeof(string));
        dt.Columns.Add("Proxy Type", typeof(string));
        dt.Columns.Add("Proxy URL (Automatic)", typeof(string));
        dt.Columns.Add("Proxy Address (Manual)", typeof(string));
        dt.Columns.Add("Proxy Port (Manual)", typeof(string));

        foreach (JProperty property in mergedJson.Properties())
        {
            if (property.Name.StartsWith("conf/network/all/"))
            {
                DataRow row = dt.NewRow();
                JToken payload = property.Value["payload"];
                JToken options = property.Value["options"];
                JToken proxy = options["proxy"];
                if (payload != null && options != null && proxy != null)
                {
                    Console.WriteLine("SSID: " + options["ssid"]);
                    Console.WriteLine("Password: " + options["password"]);
                    Console.WriteLine("Security: " + options["securityType"]);
                    Console.WriteLine("Hidden: " + options["hidden"]);
                    Console.WriteLine("Proxy: " + proxy["type"]);

                    row["SSID"] = options["ssid"];
                    row["Password"] = options["password"];
                    row["Security Type"] = options["securityType"];
                    row["Hidden"] = options["hidden"];
                    row["Proxy Type"] = proxy["type"];

                    if (proxy["type"].ToString() == "none")
                    {
                        Console.WriteLine("No proxy");
                    }
                    else if (proxy["type"].ToString() == "manual")
                    {
                        Console.WriteLine("Manual Proxy");
                        Console.WriteLine("Port: " + proxy["proxyPort"]);
                        Console.WriteLine("IP: " + proxy["proxyHostName"]);
                        row["Proxy Address (Manual)"] = proxy["proxyHostName"];
                        row["Proxy Port (Manual)"] = proxy["proxyPort"];
                    }
                    else if (proxy["type"].ToString() == "automatic")
                    {
                        Console.WriteLine("Auto Proxy");
                        Console.WriteLine("URL: " + proxy["autoProxyUrl"]);
                        row["Proxy URL (Automatic)"] = proxy["autoProxyUrl"];
                    }

                    dt.Rows.Add(row);
                }
            }
        }

        Board.DataContext = dt;
    }*/

    public async void Get_Click(object sender, RoutedEventArgs e)
    {
        if (UT.CheckInternet())
        {
            await UT.waitstatus.open();
            await Task.Delay(1000);

            var web = new HttpClient();
            string sn = serial.Text;
            string configurl = $"https://idf.hisqool.com/conf/devices/{sn}/configuration";

            HttpResponseMessage response = await web.GetAsync(configurl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string g = await web.GetStringAsync(configurl);
                string jsonString = g;
                List<dynamic> dataList = JsonConvert.DeserializeObject<List<dynamic>>(jsonString);

                List<string> urlList = new List<string>();

                foreach (var data in dataList)
                {
                    string url = data.url;
                    urlList.Add(url);
                }

                List<string> jsonResponseList = new List<string>();

                JObject mergedJson = new JObject();

                foreach (var url in urlList)
                {
                    JObject json = JObject.Parse(await web.GetStringAsync(url));
                    mergedJson.Merge(json);
                }

                ObservableCollection<DataRow> dataRows = new ObservableCollection<DataRow>();

                /*DataRow prerow = new DataRow();
                prerow.SSID = "SSID";
                prerow.Password = "Password";
                prerow.SecurityType = "Security Type";
                prerow.Hidden = "Hidden ?";
                prerow.ProxyType = "Proxy Type";
                prerow.ProxyAddressManual = "Proxy IP (Manual)";
                prerow.ProxyPortManual = "Proxy Port (Manual)";
                prerow.ProxyUrlAutomatic = "Proxy URL (Automatic)";
                dataRows.Add(prerow);*/

                foreach (JProperty property in mergedJson.Properties())
                {
                    if (property.Name.StartsWith("conf/network/all/"))
                    {
                        JToken payload = property.Value["payload"];
                        JToken options = property.Value["options"];
                        JToken proxy = options["proxy"];
                        if (payload != null && options != null && proxy != null)
                        {
                            DataRow row = new DataRow();

                            row.SSID = options["ssid"].ToString() + "   ";
                            row.Password = options["password"].ToString() + "   ";
                            row.SecurityType = options["securityType"].ToString() + "   ";
                            if (options["hidden"] != null)
                            {
                                row.Hidden = options["hidden"].ToString() + "   ";
                            }
                            else
                            {
                                row.Hidden = "Null" + "   ";
                            }

                            row.ProxyType = proxy["type"].ToString() + "   ";

                            if (proxy["type"].ToString() == "none")
                            {

                            }
                            else if (proxy["type"].ToString() == "manual")
                            {
                                row.ProxyAddressManual = proxy["proxyHostName"].ToString() + "   ";
                                row.ProxyPortManual = proxy["proxyPort"].ToString() + "   ";
                            }
                            else if (proxy["type"].ToString() == "automatic")
                            {
                                row.ProxyUrlAutomatic = proxy["autoProxyUrl"].ToString() + "   ";
                            }

                            dataRows.Add(row);
                        }
                    }
                }

                Board.ItemsSource = dataRows;
                await UT.waitstatus.close();
            }
            else
            {
                await UT.waitstatus.close();
                UT.DialogIShow(UT.GetLang("noid"), "no.png");
            }            
        }
        else
        {
            UT.DialogIShow(UT.GetLang("nonet"), "nowifi.png");
        }
    }

    public class DataRow
    {
        public string SSID { get; set; }
        public string Password { get; set; }
        public string SecurityType { get; set; }
        public string Hidden { get; set; }
        public string ProxyType { get; set; }
        public string ProxyAddressManual { get; set; }
        public string ProxyPortManual { get; set; }
        public string ProxyUrlAutomatic { get; set; }
    }
}