﻿using Wpf.Ui.Common.Interfaces;
using Unowhy_Tools_WPF.ViewModels;
using System.Diagnostics;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;

using Unowhy_Tools;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Pipes;
using Wpf.Ui.Interop.WinDef;
using Unowhy_Tools_WPF.Services.Contracts;

namespace Unowhy_Tools_WPF.Views.Pages;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class DebugPage : INavigableView<DashboardViewModel>
{
    NamedPipeClientStream pipeClient;
    UT.Data UTdata = new UT.Data();

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    private readonly ITestWindowService _testWindowService;
    private readonly ISnackbarService _snackbarService;

    public DashboardViewModel ViewModel
    {
        get;
    }

    public DebugPage(DashboardViewModel viewModel, ISnackbarService snackbarService, ITestWindowService testWindowService)
    {
        ViewModel = viewModel;
        InitializeComponent();

        verlab.Text = "Debug Page";

        _testWindowService = testWindowService;
        _snackbarService = snackbarService;
    }

    public void Github_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/STY1001/Unowhy-Tools",
                        UseShellExecute = true
        });
    }

    public void STY1001_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        UT.NavigateTo(typeof(Wifi));
    }

    public async void Discord_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        await UT.waitstatus.open();
        await Task.Delay(1000);
        await UT.waitstatus.close();
    }

    public async void Update_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        debus.Text = "DL...";
        var web = new HttpClient();
        var filebyte = await web.GetByteArrayAsync("https://bit.ly/UTdebupdateZIP");
        string utemp = Path.GetTempPath() + "Unowhy Tools\\Temps";
        File.WriteAllBytes(utemp + "\\update.zip", filebyte);
        debus.Text = "EX...";
        ZipFile.ExtractToDirectory(utemp + "\\update.zip", utemp + "\\Update");
        string pre = utemp + "\\update";
        string post = Directory.GetCurrentDirectory();

        Process.Start("cmd.exe", $"/c echo Updating Unowhy Tools... & taskkill /f /im \"Unowhy Tools.exe\" & net stop UTS & timeout -t 3 & del /s /q \"{post}\\*\" & xcopy \"{pre}\" \"{post}\" /e /h /c /i /y & echo Done ! & powershell -windows hidden -command \"\" & \"Unowhy Tools.exe\" -user {UTdata.UserID}");

    }   

    public void al_click(object sender, System.Windows.RoutedEventArgs e)
    {
        UT.applylang_global();
        instprog_txt.Text = "Hello";
    }

    public void DialoQ_Test(object sender, System.Windows.RoutedEventArgs e)
    {
        if (UT.DialogQShow(dialogtxt.Text, "question.png") == true)
        {
            dqtest.Content = "YES";
        }
        else
        {
            dqtest.Content = "NO";
        }
    }

    public void DialogI_Test(object sender, System.Windows.RoutedEventArgs e)
    {
        UT.DialogIShow(dialogtxt.Text, "about.png");
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        UT.anim.BackBtnAnim(backbtn);
        await Task.Delay(150);
        UT.anim.TransitionBack(Grid1);
        await Task.Delay(200);
        UT.NavigateTo(typeof(Dashboard));
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
        string rep = await UT.UTS.UTSmsg(pipe.Text, msg.Text);
        UT.DialogIShow(rep, "about.png");
    }

    private async void LaunchDebugTray()
    {
        try
        {
            _testWindowService.Show<Views.TrayWindow>();
        }
        catch (Exception ex)
        {
            UT.DialogIShow(ex.ToString(), "no.png");
        }
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        try
        {
            LaunchDebugTray();
        }
        catch (Exception ex)
        {
            UT.DialogIShow(ex.ToString(), "no.png");
        }
    }

    private async void UiPage_Unloaded(object sender, RoutedEventArgs e)
    {
        UT.anim.TransitionBack(Grid1);
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        var mainWindow = System.Windows.Application.Current.MainWindow as Unowhy_Tools_WPF.Views.MainWindow;
        mainWindow.ChangeTheme();
    }

    private void Button_Click_4(object sender, RoutedEventArgs e)
    {
        AllocConsole();
    }

    private async void UiPage_Loaded(object sender, RoutedEventArgs e)
    {
        await UT.DeployBack(typeof(Dashboard), Grid1);
    }
}
