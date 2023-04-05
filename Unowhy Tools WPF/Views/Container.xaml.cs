﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Unowhy_Tools_WPF.ViewModels;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.TaskBar;
using Unowhy_Tools_WPF.Services;

using Unowhy_Tools;
using Wpf.Ui.Mvvm.Interfaces;
using Wpf.Ui.Mvvm.Services;
using Unowhy_Tools_WPF.Views.Pages;
using System.Windows.Controls.Primitives;
using Unowhy_Tools_WPF.Views.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Unowhy_Tools_WPF.Views;

/// <summary>
/// Interaction logic for Container.xaml
/// </summary>
public partial class Container : INavigationWindow
{
    UT.Data UTdata = new UT.Data();
    private bool _initialized = false;

    private readonly IThemeService _themeService;

    private readonly ITaskBarService _taskBarService;

    private readonly ISnackbarService _snackbarService;

    private readonly IDialogService _dialogService;

    public ContainerViewModel ViewModel
    {
        get;
    }

    public void applylang()
    {
        home.Content = UT.GetLang("titlehome");
        hsqm.Content = UT.GetLang("titlehsqm");
        repair.Content = UT.GetLang("titlerepair");
        delete.Content = UT.GetLang("titledelete");
        customize.Content = UT.GetLang("titlecust");
        drivers.Content = UT.GetLang("titledrv");
        pcname.Content = UT.GetLang("titlepcn");
        wre.Content = UT.GetLang("titlewre");
        adduser.Content = UT.GetLang("titleadduser");
        adminset.Content = UT.GetLang("titleadminset");
        about.Content = UT.GetLang("titleabout");
        drvbk.Content = UT.GetLang("titledrvbk");
        drvrt.Content = UT.GetLang("titledrvrt");
        drvconv.Content = UT.GetLang("titleconv");
        settings.Content = UT.GetLang("titlesettings");
        pcinfo.Content = UT.GetLang("titlepci");
    }

    public Container(ContainerViewModel viewModel, INavigationService navigationService, IPageService pageService, IThemeService themeService, ITaskBarService taskBarService, ISnackbarService snackbarService, IDialogService dialogService)
    {
        ViewModel = viewModel;
        DataContext = this;
        _themeService = themeService;
        _taskBarService = taskBarService;
        InitializeComponent();
        SetPageService(pageService);
        navigationService.SetNavigationControl(RootNavigation);
        snackbarService.SetSnackbarControl(RootSnackbar);
        dialogService.SetDialogControl(RootDialog);

        _snackbarService = snackbarService;
        _dialogService = dialogService;

        _themeService.SetTheme(_themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);
        _themeService.SetTheme(_themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);

        applylang();
    }

    public void NavAnimLeft()
    {
        RootNavigation.TransitionType = Wpf.Ui.Animations.TransitionType.SlideLeft;
    }

    public void NavAnimRight()
    {
        RootNavigation.TransitionType = Wpf.Ui.Animations.TransitionType.SlideRight;
    }
    
    public void NavAnimNormal()
    {
        RootNavigation.TransitionType = Wpf.Ui.Animations.TransitionType.SlideBottom;
    }

    public bool ShowDialogQ(string msg, BitmapImage img)
    {
        var result = DialogQRoot.ShowDialog(msg, img);
        if (result)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ShowDialogI(string msg, BitmapImage img)
    {
        DialogIRoot.ShowDialog(msg, img);
    }

    public async Task ShowWait()
    {
        await WaitRoot.Show();
    }

    public async Task HideWait()
    {
        await WaitRoot.Hide();
    }

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        System.Windows.Application.Current.Shutdown();
    }

    #region INavigationWindow methods

    public Frame GetFrame()
        => RootFrame;

    public INavigation GetNavigation()
        => RootNavigation;

    public bool Navigate(Type pageType)
        => RootNavigation.Navigate(pageType);

    public void SetPageService(IPageService pageService)
        => RootNavigation.PageService = pageService;

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();

    #endregion INavigationWindow methods

    private async void Init(object sender, RoutedEventArgs e)
    {
        await Load();
    }
    
    private async void NavClick(object sender, RoutedEventArgs e)
    {
        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(0.1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation translateAnimation = new DoubleAnimation
        {
            From = 150,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        TranslateTransform transform = new TranslateTransform();
        RootFrame.RenderTransform = transform;
        RootFrame.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        transform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);
    }

    private async Task Load()
    {
        if (UT.version.isdeb())
        {
            await Task.Delay(150);
            await _snackbarService.ShowAsync("Important info", "Take note, you are using a debug version of Unowhy Tools, this debug version might be bugged", SymbolRegular.Edit32, ControlAppearance.Danger);
            await Task.Delay(150);
            RootMainGrid.Visibility = Visibility.Collapsed;
            RootTitleBar.Visibility = Visibility.Collapsed;
        }
        
        if (!UT.version.isdeb())
        {
            RootMainGrid.Visibility = Visibility.Collapsed;
            RootTitleBar.Visibility = Visibility.Collapsed;
            await Task.Delay(1500);
        }
        RootWelcomeGrid.Visibility = Visibility.Visible;

        DoubleAnimation anim = new DoubleAnimation();
        anim.From = 1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(500);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        DoubleAnimation animopac = new DoubleAnimation();
        animopac.From = 0;
        animopac.To = 1;
        animopac.Duration = TimeSpan.FromMilliseconds(500);
        animopac.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        TranslateTransform trans = new TranslateTransform();
        RootWelcomeGrid.RenderTransform = trans;
        RootWelcomeGrid.BeginAnimation(UIElement.OpacityProperty, animopac);
        trans.BeginAnimation(TranslateTransform.XProperty, anim);

        anim = new DoubleAnimation();
        anim.From = -1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(1000);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        trans = new TranslateTransform();
        SplashImg.RenderTransform = trans;
        trans.BeginAnimation(TranslateTransform.YProperty, anim);

        anim = new DoubleAnimation();
        anim.From = -1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(1000);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        trans = new TranslateTransform();
        SplashDesc.RenderTransform = trans;
        trans.BeginAnimation(TranslateTransform.XProperty, anim);

        anim = new DoubleAnimation();
        anim.From = 1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(1000);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        trans = new TranslateTransform();
        SplashCredit.RenderTransform = trans;
        trans.BeginAnimation(TranslateTransform.XProperty, anim);

        anim = new DoubleAnimation();
        anim.From = 1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(1000);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        trans = new TranslateTransform();
        SplashBar.RenderTransform = trans;
        SplashText.RenderTransform = trans;
        trans.BeginAnimation(TranslateTransform.YProperty, anim);

        string ver = "Version " + UT.version.getverfull().ToString().Insert(2, ".") + " (Build " + UT.version.getverbuild().ToString() + ") ";
        if (UT.version.isdeb()) ver = ver + "(Debug/Beta)";
        else ver = ver + "(Release)";
        SplashVer.Text = ver;

        anim = new DoubleAnimation();
        anim.From = 1000;
        anim.To = 0;
        anim.Duration = TimeSpan.FromMilliseconds(500);
        anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
        trans = new TranslateTransform();
        SplashVer.RenderTransform = trans;
        trans.BeginAnimation(TranslateTransform.XProperty, anim);
        trans.BeginAnimation(TranslateTransform.YProperty, anim);

        if (!UT.CheckAdmin())
        {
            SplashText.Text = "Hi !";
            await Task.Delay(1000);
            SplashText.Text = "Restarting as admin...";
            SplashBar.Value = 100;
            await Task.Delay(500);

            anim.From = 0;
            anim.To = -10000;
            anim.Duration = TimeSpan.FromMilliseconds(500);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            RootWelcomeGrid.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, anim);
            
            UT.RunAdmin($"-u {UTdata.UserID}");
        }
        else
        {
            SplashText.Text = "Hi !";
            await Task.Delay(1500);
            SplashText.Text = "Loading... (Cleaning)";
            SplashBar.Value = 0;
            await Task.Delay(500);
            await UT.Cleanup();
            SplashText.Text = "Loading... (Checking Files)";
            SplashBar.Value = 65;
            await Task.Delay(500);
            bool fs = await UT.FirstStart();
            SplashText.Text = "Loading... (Checking System)";
            SplashBar.Value = 70;
            await Task.Delay(500);
            await UT.Check();
            SplashText.Text = "Loading... (Checking Unowhy Tools Service)";
            SplashBar.Value = 80;
            await Task.Delay(500);
            await UT.UTS.UTScheck();
            SplashText.Text = "Loading... (Preloading pages)";
            Navigate(typeof(Dashboard));
            Navigate(typeof(HisqoolManager));
            Navigate(typeof(Repair));
            Navigate(typeof(Delete));
            Navigate(typeof(Customize));
            Navigate(typeof(Drivers));
            Navigate(typeof(Settings));
            Navigate(typeof(About));
            Navigate(typeof(Updater));
            SplashBar.Value = 90;
            await Task.Delay(500);

            SplashText.Text = "Ready !";
            SplashBar.Value = 100;

            string utsserial = await UT.UTS.UTSmsg("UTSW", "GetSN");

            RootNavigation.TransitionType = Wpf.Ui.Animations.TransitionType.SlideRight;
            foreach (UIElement elements in RootNavigation.Items)
            {
                elements.Visibility = Visibility.Hidden;
            }
            foreach (UIElement elements in RootNavigation.Footer)
            {
                elements.Visibility = Visibility.Hidden;
            }

            await Task.Delay(500);

            anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = -1000;
            anim.Duration = TimeSpan.FromMilliseconds(1000);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            SplashImg.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, anim);

            anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = -1000;
            anim.Duration = TimeSpan.FromMilliseconds(1000);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            SplashDesc.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, anim);

            anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1000;
            anim.Duration = TimeSpan.FromMilliseconds(1000);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            SplashCredit.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, anim);

            anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1000;
            anim.Duration = TimeSpan.FromMilliseconds(1000);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            SplashBar.RenderTransform = trans;
            SplashText.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, anim);

            anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 1000;
            anim.Duration = TimeSpan.FromMilliseconds(500);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            SplashVer.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, anim);
            trans.BeginAnimation(TranslateTransform.YProperty, anim);

            await Task.Delay(500);

            UT.anim.TransitionForw(RootWelcomeGrid);

            await Task.Delay(150);

            RootWelcomeGrid.Visibility = Visibility.Hidden;
            RootTitleBar.Visibility = Visibility.Visible;
            RootMainGrid.Visibility = Visibility.Visible;

            anim = new DoubleAnimation();
            anim.From = -50;
            anim.To = 0;
            anim.Duration = TimeSpan.FromMilliseconds(1000);
            anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 5 };
            trans = new TranslateTransform();
            RootTitleBar.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.YProperty, anim);

            if (fs || utsserial == "Null")
            {
                RootNavigation.Visibility = Visibility.Collapsed;
                Navigate(typeof(FirstConfig));
            }
            else
            {
                Navigate(typeof(Dashboard));
                foreach (UIElement elements in RootNavigation.Items)
                {
                    elements.Visibility = Visibility.Visible;
                    DoubleAnimation opacityAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    DoubleAnimation translateAnimation = new DoubleAnimation
                    {
                        From = -50,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    TranslateTransform transform = new TranslateTransform();
                    elements.RenderTransform = transform;

                    elements.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
                    transform.BeginAnimation(TranslateTransform.XProperty, translateAnimation);

                    await Task.Delay(50);
                }
                foreach (UIElement elements in RootNavigation.Footer)
                {
                    elements.Visibility = Visibility.Visible;
                    DoubleAnimation opacityAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    DoubleAnimation translateAnimation = new DoubleAnimation
                    {
                        From = 50,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    TranslateTransform transform = new TranslateTransform();
                    elements.RenderTransform = transform;

                    elements.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
                    transform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);

                    await Task.Delay(150);
                }
                await Task.Delay(1000);
                RootNavigation.TransitionType = Wpf.Ui.Animations.TransitionType.SlideBottom;

                pcname.Visibility = Visibility.Collapsed;
                adduser.Visibility = Visibility.Collapsed;
                adminset.Visibility = Visibility.Collapsed;
                drvbk.Visibility = Visibility.Collapsed;
                drvrt.Visibility = Visibility.Collapsed;
                drvconv.Visibility = Visibility.Collapsed;
                wre.Visibility = Visibility.Collapsed;
                pcinfo.Visibility = Visibility.Collapsed;
                updater.Visibility = Visibility.Collapsed;
                wifi.Visibility = Visibility.Collapsed;
                fc.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void NavigationButtonTheme_OnClick(object sender, RoutedEventArgs e)
    {
        _themeService.SetTheme(_themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);
    }

    private void TrayMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;

        System.Diagnostics.Debug.WriteLine($"DEBUG | WPF UI Tray clicked: {menuItem.Tag}", "Unowhy_Tools_WPF");
    }

    private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"DEBUG | WPF UI Navigated to: {sender?.Current ?? null}", "Unowhy_Tools_WPF");

        // This funky solution allows us to impose a negative
        // margin for Frame only for the Dashboard page, thanks
        // to which the banner will cover the entire page nicely.
        RootFrame.Margin = new Thickness(
            left: 0,
            top: sender?.Current?.PageTag == "" ? -69 : 0,
            right: 0,
            bottom: 0);
    }
}

