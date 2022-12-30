using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LordJZ.Linq;
using LordJZ.WinAPI;

namespace WindowPositions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            NativeWindow active = NativeWindow.Foreground;
            if (active.Handle != Handle.NullHandle)
            {
                ProcessDpiAwareness? dpi = null;
                try
                {
                    using (Process proc = Process.Open(active.ProcessId))
                        dpi = proc.DpiAwareness;
                }
                catch
                {
                }

                this.ActivaWindowTextBox.Text = active.ProcessId + "|" + active.ClassName + "|" + active.Text + " (" + dpi + ")";
            }
            else
                this.ActivaWindowTextBox.Text = string.Empty;

            WindowDTO item = (WindowDTO)this.WindowsListView.SelectedItem;
            if (item != null && item.NativeWindow.IsValid)
            {
                WindowPosition wp = WindowPositionRepository.Positions.FirstOrDefault(_ => _.Matches(item));
                WindowPlacement placement = item.NativeWindow.Placement;

                this.DetailsTextBox.Text = string.Format(
                    "Min={1}{0}" +
                    "Max={2}{0}" +
                    "Normal={3}{0}" +
                    "Show={4}{0}" +
                    "Flags={5}{0}{0}" +
                    "Saved={6} WithTitle={7}",
                    Environment.NewLine,
                    placement.MinPosition,
                    placement.MaxPosition,
                    placement.NormalPosition,
                    placement.Show,
                    placement.Flags,
                    wp != null,
                    wp != null && wp.Title != null);

                this.SavePositionButton.IsEnabled = true;
                this.RestorePositionButton.IsEnabled = wp != null;
            }
            else
            {
                this.SavePositionButton.IsEnabled = false;
                this.RestorePositionButton.IsEnabled = false;
            }
        }

        void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
	        this.WindowsListView.ItemsSource =
		        WindowDTO.Enumerate()
		                 .Select(nw =>
		                 {
			                 nw.Saved = WindowPositionRepository.Positions.Any(_ => _.Matches(nw));
			                 return nw;
		                 })
		                 .OrderByDescending(w => w.Saved)
		                 .ThenBy(w => w.ClassName)
		                 .ThenBy(w => w.Title)
		                 .ToArray();
        }

        private void RestorePositionButton_Click(object sender, RoutedEventArgs e)
        {
            WindowDTO item = (WindowDTO)this.WindowsListView.SelectedItem;
            WindowPosition pos = WindowPositionRepository.Positions.First(_ => _.Matches(item));
            pos.Apply(item.NativeWindow);
        }

        private void SavePositionButton_Click(object sender, RoutedEventArgs e)
        {
            WindowDTO item = (WindowDTO)this.WindowsListView.SelectedItem;
            var positions = WindowPositionRepository.Positions.Where(_ => !_.Matches(item));

            WindowPosition pos = new WindowPosition();
            pos.Placement = item.NativeWindow.Placement;
            pos.ClassName = item.ClassName;
            pos.Title = this.MatchTitleCheckBox.IsChecked != false ? item.Title : null;

            positions = System.Linq.Enumerable.Concat(positions, new[] { pos });

            WindowPositionRepository.Positions = positions.ToArray();
        }

        private void RestoreAllPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowPositionRepository.RestoreAllPositions();
        }
    }
}
