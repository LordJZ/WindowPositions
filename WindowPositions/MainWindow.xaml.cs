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
    public partial class MainWindow : Window
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
            if (active.Handle == Handle.NullHandle)
                this.ActivaWindowTextBox.Text = string.Empty;
            else
                this.ActivaWindowTextBox.Text = active.ClassName + "|" + active.Text;

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
                NativeWindow.Enumerate()
                            .Select(nw => new WindowDTO(nw))
                            .OrderBy(w => w.ClassName)
                            .ThenBy(w => w.Title)
                            .ToArray();
        }

        private void RestorePositionButton_Click(object sender, RoutedEventArgs e)
        {
            WindowDTO item = (WindowDTO)this.WindowsListView.SelectedItem;
            WindowPosition pos = WindowPositionRepository.Positions.First(_ => _.Matches(item));
            NativeWindow window = item.NativeWindow;
            window.Placement = pos.Placement;
        }

        private void SavePositionButton_Click(object sender, RoutedEventArgs e)
        {
            WindowDTO item = (WindowDTO)this.WindowsListView.SelectedItem;
            var positions = WindowPositionRepository.Positions.Where(_ => !_.Matches(item));

            WindowPosition pos = new WindowPosition();
            pos.Placement = item.NativeWindow.Placement;
            pos.ClassName = item.NativeWindow.ClassName;
            pos.Title = this.MatchTitleCheckBox.IsChecked != false ? item.NativeWindow.Text : null;

            positions = System.Linq.Enumerable.Concat(positions, new[] { pos });

            WindowPositionRepository.Positions = positions.ToArray();
        }

        private void RestoreAllPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowPositionRepository.RestoreAllPositions();
        }
    }
}
