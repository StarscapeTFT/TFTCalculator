using Octokit;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace TFTCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScenarioViewModel viewModel;

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        public ScenarioViewModel ViewModel
        {
            get => viewModel;
        }

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ScenarioViewModel();
            DataContext = viewModel;

            SizeChanged += MainWindow_SizeChanged;
            SourceInitialized += MainWindow_SourceInitialized;

            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            try
            {
                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("TFTCalculator"));

                var releases = await gitHubClient.Repository.Release.GetAll("StarscapeTFT", "TFTCalculator");
                Release newestRelease = releases[0];
                Version newestVersion = new Version(newestRelease.TagName);
                
                if (newestVersion > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    UpdateButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {

            }
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // Adds the window shadow back, if it is enabled as a Windows setting.
            // See: https://groups.google.com/forum/#!topic/wpf-disciples/gtQI5Wngtfk
            var helper = new WindowInteropHelper(this);

            int val = 2;
            DwmSetWindowAttribute(helper.Handle, 2, ref val, 4);

            var m = new Margins { Bottom = -1, Left = -1, Right = -1, Top = -1 };
            DwmExtendFrameIntoClientArea(helper.Handle, ref m);
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Thickness t = SystemParametersFix.WindowResizeBorderThickness;
                ChromeBorder.BorderThickness = new Thickness(Math.Round(t.Left), Math.Round(t.Top), Math.Round(t.Right), Math.Round(t.Bottom));
                Chrome.ResizeBorderThickness = new Thickness(0);
            }
            else
            {
                ChromeBorder.BorderThickness = new Thickness(0);
                Chrome.ResizeBorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/StarscapeTFT/TFTCalculator",
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/StarscapeTFT/TFTCalculator",
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        private void PatchSelectorItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ComboBoxItem hoveredItem)
            {
                var comboBoxItem = PatchSelector.Items.OfType<ShopProbabilityViewModel>().FirstOrDefault(i => i.Name == hoveredItem.Content.ToString());

                int index = PatchSelector.Items.IndexOf(comboBoxItem);

                if (index != -1)
                {
                    ViewModel.PreviewedShopProbability = ViewModel.ShopSets[index];
                }
            }
        }

        private void PatchSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            ViewModel.PreviewedShopProbability = ViewModel.ShopProbability;
        }

        private void OxyPlot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is PlotView plotView)
            {
                Series series = plotView.ActualModel.Series.FirstOrDefault();

                if (series is XYAxisSeries xySeries && series != null)
                {
                    HitTestResult hitTestResult = xySeries.HitTest(new HitTestArguments(e.GetPosition(plotView).ToScreenPoint(), 1));

                    if (hitTestResult != null)
                    {
                        // What kind of insanity is this? Index is a floating point value?
                        int index = Convert.ToInt32(hitTestResult.Index);
                        hitTestResult.Element.SelectItem(index);
                    }
                    else
                    {
                        xySeries.PlotModel.Series.First().ClearSelection();
                    }
                }
            }
        }
    }
}
