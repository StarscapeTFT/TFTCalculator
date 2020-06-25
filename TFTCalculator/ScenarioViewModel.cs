using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TFTCalculator
{
    public static class OxyExtensions
    {
        public static OxyColor ToOxyColor(this Color color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public static class MathExtensions
    {
        public static bool NearlyEqualTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.001;
        }

        public static bool NearlyGreaterThan(this double value1, double value2)
        {
            return value1 - value2 > -0.001;
        }
    }

    public class ScenarioViewModel : ViewModel
    {
        const int RefreshInterval = 100;
        const int MaxShops = 50;
        const int ProbabilityModeIndex = 0;
        const int LevelEquivalentsModeIndex = 1;

        public BindingList<UnitViewModel> Units
        {
            get; set;
        } = new BindingList<UnitViewModel>();

        public BindingList<ObservableValue<int>> OthersTaken
        {
            get; set;
        } = new BindingList<ObservableValue<int>>
        {
            new ObservableValue<int>(0),
            new ObservableValue<int>(0),
            new ObservableValue<int>(0),
            new ObservableValue<int>(0),
            new ObservableValue<int>(0)
        };

        private int playerLevel;
        public int PlayerLevel
        {
            get => playerLevel;
            set
            {
                SetProperty(ref playerLevel, value);
                OnRelatedPropertyChanged(nameof(TierProbabilitiesForLevel));
            }
        }

        private int alternatePlayerLevel;
        public int AlternatePlayerLevel
        {
            get => alternatePlayerLevel;
            set => SetProperty(ref alternatePlayerLevel, value);
        }

        public double[] TierProbabilitiesForLevel
        {
            get => ShopProbability.TierProbabilities[PlayerLevel];
        }

        ShopProbabilityViewModel shopProbability;
        public ShopProbabilityViewModel ShopProbability
        {
            get => shopProbability;
            set
            {
                SetProperty(ref shopProbability, value);

                foreach (UnitViewModel unit in Units)
                {
                    unit.PoolSize = ShopProbability.UnitPool[unit.Tier];
                }

                PreviewedShopProbability = value;
            }
        }

        ShopProbabilityViewModel previewedShopProbability;
        public ShopProbabilityViewModel PreviewedShopProbability
        {
            get => previewedShopProbability;
            set => SetProperty(ref previewedShopProbability, value);
        }

        bool useApproximateMode = false;
        public bool UseApproximateMode
        {
            get => useApproximateMode;
            set => SetProperty(ref useApproximateMode, value);
        }

        List<string> searchOptions = new List<string>();
        public List<string> SearchOptions
        {
            get => searchOptions;
            set => SetProperty(ref searchOptions, value);
        }

        int selectedSearchOption;
        public int SelectedSearchOption
        {
            get => selectedSearchOption;
            set => SetProperty(ref selectedSearchOption, value);
        }

        Visibility plotVisibility = Visibility.Visible;
        public Visibility PlotVisibility
        {
            get => plotVisibility;
            set => SetProperty(ref plotVisibility, value);
        }

        Visibility accuracyWarningVisibility = Visibility.Hidden;
        public Visibility AccuracyWarningVisibility
        {
            get => accuracyWarningVisibility;
            set => SetProperty(ref accuracyWarningVisibility, value);
        }

        Visibility errorVisibility = Visibility.Hidden;
        public Visibility ErrorVisibility
        {
            get => errorVisibility;
            set => SetProperty(ref errorVisibility, value);
        }

        string errorText = "Something bad happened.";
        public string ErrorText
        {
            get => errorText;
            set => SetProperty(ref errorText, value);
        }

        Visibility warningVisibility = Visibility.Hidden;
        public Visibility WarningVisibility
        {
            get => warningVisibility;
            set => SetProperty(ref warningVisibility, value);
        }

        string warningText = "Something bad happened.";
        public string WarningText
        {
            get => warningText;
            set => SetProperty(ref warningText, value);
        }

        Visibility busyIconVisibility = Visibility.Hidden;
        public Visibility BusyIconVisibility
        {
            get => busyIconVisibility;
            set => SetProperty(ref busyIconVisibility, value);
        }

        int selectedModeIndex;
        public int SelectedModeIndex
        {
            get => selectedModeIndex;
            set => SetProperty(ref selectedModeIndex, value);
        }

        public Func<double, string> YAxisFormatter { get; }

        public PlotModel PlotModelCDF
        {
            get;
        }

        public PlotModel PlotModelPDF
        {
            get;
        }

        public PlotModel PlotModelLevelComparison
        {
            get;
        }

        public PlotController PlotControllerCDF
        {
            get;
        } = new PlotController();

        public PlotController PlotControllerPDF
        {
            get;
        } = new PlotController();

        public PlotController PlotControllerLevelComparison
        {
            get;
        } = new PlotController();

        public List<ShopProbabilityViewModel> ShopSets
        {
            get;
        }

        private readonly List<HistogramItem> ProbabilityDistribution = new List<HistogramItem>();
        private readonly List<HistogramItem> CumulativeProbability = new List<HistogramItem>();
        private readonly List<BarItem> LevelEquivalents = new List<BarItem>();

        private readonly HistogramSeries cdfSeries;
        private readonly HistogramSeries pdfSeries;
        private readonly BarSeries levelEquivalentsSeries;

        object calculatorThreadLockToken = new object();
        CancellationTokenSource cancellationTokenSource;
        CalculationModel calculationModel;
        DispatcherTimer updateTimer = new DispatcherTimer();

        public ScenarioViewModel()
        {
            Color plotColor = (Color)Application.Current.Resources["Heading1Color"];
            
            PlotControllerCDF.Bind(new OxyMouseEnterGesture(), PlotCommands.HoverSnapTrack);
            PlotControllerPDF.Bind(new OxyMouseEnterGesture(), PlotCommands.HoverSnapTrack);
            PlotControllerLevelComparison.Bind(new OxyMouseEnterGesture(), PlotCommands.HoverSnapTrack);

            ShopSets = ShopProbabilityViewModel.CreateAllShops();
            ShopProbability = ShopSets[0];

            PlayerLevel = 6;

            YAxisFormatter = (double value) => { return value.ToString("0.00", CultureInfo.CurrentCulture); };
            PlotModelCDF = new PlotModel()
            {
                PlotAreaBorderColor = plotColor.ToOxyColor(),
                DefaultColors = new List<OxyColor>() { OxyColor.FromArgb(196, 66, 165, 255) },
                SelectionColor = OxyColor.FromArgb(196, 122, 105, 255),
            };

            PlotModelPDF = new PlotModel()
            {
                PlotAreaBorderColor = plotColor.ToOxyColor(),
                DefaultColors = new List<OxyColor>() { OxyColor.FromArgb(196, 66, 165, 255) },
                SelectionColor = OxyColor.FromArgb(196, 122, 105, 255),
            };

            PlotModelLevelComparison = new PlotModel()
            {
                PlotAreaBorderColor = plotColor.ToOxyColor(),
                DefaultColors = new List<OxyColor>() { OxyColor.FromArgb(196, 66, 165, 255) },
                SelectionColor = OxyColor.FromArgb(196, 122, 105, 255),
            };

            PlotModelCDF.MouseLeave += (s, e) =>
            {
                cdfSeries.ClearSelection();
                Series_SelectionChanged(PlotModelCDF.Series.First(), new EventArgs());
            };

            PlotModelPDF.MouseLeave += (s, e) =>
            {
                pdfSeries.ClearSelection();
                Series_SelectionChanged(PlotModelPDF.Series.First(), new EventArgs());
            };

            PlotModelLevelComparison.MouseLeave += (s, e) =>
            {
                levelEquivalentsSeries.ClearSelection();
                Series_SelectionChanged(PlotModelLevelComparison.Series.First(), new EventArgs());
            };

            PlotModelCDF.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = -0.5,
                AbsoluteMaximum = 50.5,
                Minimum = -0.5,
                Maximum = 50.5,
                Title = "Number of Shops",
                AxisTitleDistance = 4,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            PlotModelCDF.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1,
                Minimum = 0,
                Maximum = 1,
                Title = "Probability",
                Key = "cdf",
                AxisTitleDistance = 10,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            PlotModelPDF.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = -0.5,
                AbsoluteMaximum = 50.5,
                Minimum = -0.5,
                Maximum = 50.5,
                Title = "Number of Shops",
                AxisTitleDistance = 4,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            PlotModelPDF.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 100,
                Title = "Percent of Games",
                Key = "pdf",
                AxisTitleDistance = 10,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            PlotModelLevelComparison.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Player Level",
                Key = "level",
                AxisTitleDistance = 4,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                GapWidth = 0.4,
                Labels =
                {
                    "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9"
                },
                Angle = -45,
            });

            PlotModelLevelComparison.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                AbsoluteMinimum = 0,
                Title = "Number of Shops for ≥50%",
                Key = "shops",
                AxisTitleDistance = 10,
                AxislineColor = plotColor.ToOxyColor(),
                MajorGridlineColor = plotColor.ToOxyColor(),
                MinorTicklineColor = plotColor.ToOxyColor(),
                TicklineColor = plotColor.ToOxyColor(),
                TextColor = plotColor.ToOxyColor(),
                TitleColor = plotColor.ToOxyColor(),
                TitleFontSize = 16,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            cdfSeries = new HistogramSeries
            {
                ItemsSource = CumulativeProbability,
                YAxisKey = "cdf",
                TrackerFormatString = "Shops: {9}" + Environment.NewLine + "Probability: {7:0.###}",
                SelectionMode = SelectionMode.Single
            };

            pdfSeries = new HistogramSeries
            {
                ItemsSource = ProbabilityDistribution,
                YAxisKey = "pdf",
                TrackerFormatString = "Shops: {9}" + Environment.NewLine + "Games: {7:0.#}%",
                SelectionMode = SelectionMode.Single
            };

            levelEquivalentsSeries = new BarSeries
            {
                ItemsSource = LevelEquivalents,
                XAxisKey = "shops",
                YAxisKey = "level",
                TrackerFormatString = "{1}" + Environment.NewLine + "{2} shops for ≥50%",
                SelectionMode = SelectionMode.Single
            };

            cdfSeries.SelectionChanged += Series_SelectionChanged;
            pdfSeries.SelectionChanged += Series_SelectionChanged;
            levelEquivalentsSeries.SelectionChanged += Series_SelectionChanged;
            
            for (int level = 0; level < 9; ++level)
            {
                LevelEquivalents.Add(new BarItem(0, level));
            }

            PlotModelCDF.Series.Add(cdfSeries);
            PlotModelPDF.Series.Add(pdfSeries);
            PlotModelLevelComparison.Series.Add(levelEquivalentsSeries);

            Units.ListChanged += Units_ListChanged;

            Units.Add(new UnitViewModel { IsEnabled = true, AlreadyTaken = 0, LookingFor = 3, Tier = 2 });
            Units.Add(new UnitViewModel { IsEnabled = false, AlreadyTaken = 0, LookingFor = 3 });
            Units.Add(new UnitViewModel { IsEnabled = false, AlreadyTaken = 0, LookingFor = 3 });
            Units.Add(new UnitViewModel { IsEnabled = false, AlreadyTaken = 0, LookingFor = 3 });

            OthersTaken.ListChanged += (s, e) => RecalculateAll();
            ShopProbability.PropertyChanged += (s, e) => RecalculateAll();
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ShopProbability)
                 || e.PropertyName == nameof(UseApproximateMode)
                 || e.PropertyName == nameof(SelectedSearchOption))
                    RecalculateAll();

                else if (e.PropertyName == nameof(PlayerLevel) || e.PropertyName == nameof(SelectedModeIndex))
                {
                    int level;
                    int atLeast;

                    lock (calculatorThreadLockToken)
                    {
                        cancellationTokenSource?.Cancel();
                        updateTimer.Stop();

                        // We don't need to recalculate everything. We just need to restart the calculation.
                        level = PlayerLevel;
                        atLeast = SelectedSearchOption + 1;

                        if (e.PropertyName == nameof(PlayerLevel))
                        {
                            lock (PlotModelCDF.SyncRoot)
                            {
                                CumulativeProbability.Clear();
                            }

                            lock (PlotModelPDF.SyncRoot)
                            {
                                ProbabilityDistribution.Clear();
                            }
                        }
                    }

                    DoCalculate(level, atLeast);
                }
            };

            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100 milliseconds

            UpdateUnitSearchOptions();
            SelectedSearchOption = 0;
            RecalculateAll();
        }

        private void Series_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is XYAxisSeries series)
            {
                if (series.ItemsSource is List<HistogramItem> items)
                {
                    for (int i = 0; i < items.Count; ++i)
                    {
                        if (series.GetSelectedItems().Contains(i))
                        {
                            items[i].Color = series.PlotModel.SelectionColor;
                        }
                        else
                        {
                            items[i].Color = series.PlotModel.DefaultColors[0];
                        }
                    }

                    series.PlotModel.InvalidatePlot(false);
                }

                // The items don't inherit from a common interface or class so we get the joy of copy-pasting code.
                else if (series.ItemsSource is List<BarItem> barItems)
                {
                    for (int i = 0; i < barItems.Count; ++i)
                    {
                        if (series.GetSelectedItems().Contains(i))
                        {
                            barItems[i].Color = series.PlotModel.SelectionColor;
                        }
                        else
                        {
                            barItems[i].Color = series.PlotModel.DefaultColors[0];
                        }
                    }

                    series.PlotModel.InvalidatePlot(false);
                }
            }
        }

        /// <summary>
        /// Called when we should refresh the plots.
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (calculatorThreadLockToken)
            {
                if (sender != this)
                {
                    BusyIconVisibility = Visibility.Visible;
                }

                // Copy histogram data
                List<double> probabilityData = calculationModel.CumulativeProbability[PlayerLevel];
                for (int numShops = CumulativeProbability.Count + 1; numShops < probabilityData.Count + 1; ++numShops)
                {
                    lock (PlotModelCDF.SyncRoot)
                    {
                        CumulativeProbability.Add(new HistogramItem(numShops - 0.5, numShops + 0.5, probabilityData[numShops - 1], numShops));
                    }

                    lock (PlotModelPDF.SyncRoot)
                    {
                        double difference = numShops > 1 ? probabilityData[numShops - 1] - probabilityData[numShops - 2] : probabilityData[0];

                        ProbabilityDistribution.Add(new HistogramItem(numShops - 0.5, numShops + 0.5, difference * 100, numShops));
                    }
                }

                // Copy level equivalents data
                for (int level = 0; level < 9; ++level)
                {
                    int index = calculationModel.CumulativeProbability[level].FindIndex((double d) => d >= 0.5);
                    if (index != -1)
                    {
                        LevelEquivalents[level].Value = index + 1;
                    }
                }

                PlotModelCDF.InvalidatePlot(true);
                PlotModelPDF.InvalidatePlot(true);
                PlotModelLevelComparison.InvalidatePlot(true);
            }
        }

        private void Units_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Units[e.NewIndex].PoolSize = ShopProbability.UnitPool[Units[e.NewIndex].Tier];
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                if (e.PropertyDescriptor.Name == nameof(UnitViewModel.IsEnabled))
                {
                    // Recalculate if the selected index doesn't change.
                    // This is to work around ComboBox weirdness.
                    int oldIndex = SelectedSearchOption;
                    UpdateUnitSearchOptions();
                    if (SelectedSearchOption == oldIndex)
                        RecalculateAll();
                }
                else
                {
                    if (e.PropertyDescriptor.Name == nameof(UnitViewModel.Tier))
                    {
                        Units[e.NewIndex].PoolSize = ShopProbability.UnitPool[Units[e.NewIndex].Tier];
                    }

                    RecalculateAll();
                }
            }
        }

        void UpdateUnitSearchOptions()
        {
            // Update the search options
            int numEnabled = Units.Where(unit => unit.IsEnabled).Count();
            int searchOption = SelectedSearchOption;
            List<string> newSearchOptions = new List<string>();
            if (numEnabled == 0)
            {
                newSearchOptions.Add("nothing");
            }
            else
            {
                for (int i = 1; i < numEnabled; ++i)
                {
                    newSearchOptions.Add("at least " + i);
                }

                newSearchOptions.Add("all " + numEnabled);
            }

            SearchOptions = newSearchOptions;
            SelectedSearchOption = Math.Min(newSearchOptions.Count - 1, searchOption);
        }

        void RecalculateAll()
        {
            // Stupid hack.
            // The unit search combobox clears itself if you change the items source.
            // Even though we reset it back to where it was immediately, it still fires the selection changed binding.
            // So we'll just ignore that...
            if (SelectedSearchOption == -1)
                return;

            lock (calculatorThreadLockToken)
            {
                cancellationTokenSource?.Cancel();
                updateTimer.Stop();

                lock (PlotModelCDF.SyncRoot)
                {
                    CumulativeProbability.Clear();
                }

                lock (PlotModelPDF.SyncRoot)
                {
                    ProbabilityDistribution.Clear();
                }

                foreach (BarItem item in LevelEquivalents)
                {
                    item.Value = 0;
                }
            }

            Calculate();
        }

        void Calculate()
        {
            ShopProbability shopProbability = ShopProbability.ToShopProbability(OthersTaken.ToList().ConvertAll(v => v.Value));
            List<Unit> units = Units.Where(unit => unit.IsEnabled).ToList().ConvertAll(unitViewModel => unitViewModel.ToUnit());

            if (!shopProbability.IsValid(units))
            {
                AccuracyWarningVisibility = Visibility.Hidden;
                PlotVisibility = Visibility.Hidden;
                BusyIconVisibility = Visibility.Hidden;
                ErrorVisibility = Visibility.Hidden;

                WarningText = "The combination of units you are searching for is invalid.";
                WarningVisibility = Visibility.Visible;

                return;
            }

            // Make sure the calculation won't take A Very Long Time.
            if (!UseApproximateMode)
            {
                int numCombinations = Units.Where(unit => unit.IsEnabled).Select(unit => unit.LookingFor + 1).Aggregate(1, (x, y) => x * y);

                // Arbitrary cutoff based on my computer's processor. Very scientific.
                if (numCombinations > 7 * 7 * 7 * 7)
                {
                    AccuracyWarningVisibility = Visibility.Visible;
                    PlotVisibility = Visibility.Hidden;
                    BusyIconVisibility = Visibility.Hidden;
                    ErrorVisibility = Visibility.Hidden;
                    WarningVisibility = Visibility.Hidden;
                    return;
                }
            }

            ErrorVisibility = Visibility.Hidden;
            AccuracyWarningVisibility = Visibility.Hidden;
            PlotVisibility = Visibility.Visible;
            WarningVisibility = Visibility.Hidden;

            int level;
            int atLeast;
            lock (calculatorThreadLockToken)
            {
                calculationModel = new CalculationModel(calculatorThreadLockToken,
                                                        UseApproximateMode ? CalculationAccuracy.Approximate : CalculationAccuracy.Exact,
                                                        shopProbability,
                                                        units);

                level = PlayerLevel;
                atLeast = SelectedSearchOption + 1;
            }

            DoCalculate(level, atLeast);
        }

        async void DoCalculate(int level, int atLeast)
        {
            CancellationToken cancellationToken;
            lock (calculatorThreadLockToken)
            {
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                updateTimer.Start();
            }

            try
            {
                await calculationModel.Calculate(cancellationToken,
                                                 selectedModeIndex == ProbabilityModeIndex ? CalculationMode.Probability : CalculationMode.LevelComparison,
                                                 level,
                                                 atLeast);
            }
            catch (ArgumentException e)
            {
                ErrorText = e.Message;
                PlotVisibility = Visibility.Hidden;
                ErrorVisibility = Visibility.Visible;
            }
            catch (OutOfMemoryException)
            {
                ErrorText = "Unable to allocate memory for the calculation.\n\nIf this problem persists, you might need to restart the calculator.\n\nSorry :(";
                PlotVisibility = Visibility.Hidden;
                ErrorVisibility = Visibility.Visible;
            }
            catch(Exception e)
            {
                ErrorText = "Unexpected error.\n\n" + e.Message;
                PlotVisibility = Visibility.Hidden;
                ErrorVisibility = Visibility.Visible;
            }

            lock (calculatorThreadLockToken)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    updateTimer.Stop();
                    UpdateTimer_Tick(this, new EventArgs());
                    BusyIconVisibility = Visibility.Hidden;
                }
            }
        }
    }
}
