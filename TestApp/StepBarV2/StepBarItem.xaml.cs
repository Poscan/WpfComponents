using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApp.StepBarV2
{
    public partial class StepBarItem
    {
        public StepBarItem()
        {
            InitializeComponent();

            IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Parent is StepBar stepBar)
                stepBar.UpdateCurrentStep();
        }

        public static readonly DependencyProperty ActiveContentProperty = DependencyProperty.RegisterAttached(nameof(ActiveContent), typeof(FrameworkElement), typeof(StepBarItem), new PropertyMetadata(null));
        public FrameworkElement ActiveContent
        {
            get => (FrameworkElement)GetValue(ActiveContentProperty);
            set => SetValue(ActiveContentProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached(nameof(Description), typeof(string), typeof(StepBarItem), new PropertyMetadata(string.Empty));
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        private Status _status;
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;

                switch (value)
                {
                    case Status.Waiting:
                        SetWaiting();
                        break;
                    case Status.Active:
                        SetActive();
                        break;
                    case Status.Complete:
                        SetComplete();
                        break;
                }

                UpdateProgressBars();
            }
        }

        private void UpdateProgressBars()
        {
            var stepBar = Parent as StepBar;

            var stepBarItemCollection = stepBar?.VisibilityItems;
            var firstVisibilityItem = stepBarItemCollection?.First();
            var lastVisibilityItem = stepBarItemCollection?.Last();

            var firstIndex = firstVisibilityItem?.GetIndex();
            var lastIndex = lastVisibilityItem?.GetIndex();

            var newIndex = GetIndex();

            LeftBar.Visibility = newIndex == firstIndex ? Visibility.Collapsed : Visibility.Visible;
            RightBar.Visibility = newIndex == lastIndex ? Visibility.Collapsed : Visibility.Visible;
        }

        private int GetIndex()
        {
            var stepBar = Parent as StepBar;
            var hashCode = GetHashCode();
            var newIndex = -1;

            for (var i = 0; i < stepBar?.Items.Count; i++)
            {
                if (stepBar?.Items[i].GetHashCode() == hashCode)
                {
                    newIndex = i;
                    break;
                }
            }

            return newIndex;
        }

        public Color ActiveColor;
        public Color DefaultColor;

        private Color _waitingColor;
        public Color WaitingColor
        {
            get => _waitingColor;
            set
            {
                _waitingColor = value;

                var waitingColorBrush = new SolidColorBrush(value);

                LeftBar.Background = waitingColorBrush;
                LeftBar.BorderBrush = waitingColorBrush;
                
                RightBar.Background = waitingColorBrush;
                RightBar.BorderBrush = waitingColorBrush;
            }
        }

        private Color _completeColor;
        public Color CompleteColor
        {
            get => _completeColor;
            set
            {
                _completeColor = value;

                var completeColorBrush = new SolidColorBrush(value);

                LeftBar.Foreground = completeColorBrush;
                RightBar.Foreground = completeColorBrush;
            }
        }

        private void ProgressBarBeginAnimation(ProgressBar bar, int fromValue, int toValue, Action brushAction)
        {
            var animation = new DoubleAnimation(fromValue, toValue, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);

            bar.BeginAnimation(RangeBase.ValueProperty, animation);

            brushAction.Invoke();
        }

        public void SetActiveNext() => ProgressBarBeginAnimation(RightBar, 0, 100, SetComplete);

        public void SetActivePrev() => ProgressBarBeginAnimation(LeftBar, 100, 0, SetWaiting);

        public void SetActiveLeftWithAnimation() => ProgressBarBeginAnimation(LeftBar, 0, 100, SetActive);

        public void SetActiveRightWithAnimation() => ProgressBarBeginAnimation(RightBar, 100, 0, SetActive);

        private void SetWaiting()
        {
            var waitingColorBrush = new SolidColorBrush(WaitingColor);

            Step.Stroke = waitingColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = waitingColorBrush;
            StepContent.Foreground = waitingColorBrush;

            LeftBar.Value = 0;
            RightBar.Value = 0;
        }

        private void SetActive()
        {
            var activeColorBrush = new SolidColorBrush(ActiveColor);

            Step.Stroke = activeColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = activeColorBrush;
            StepContent.Foreground = activeColorBrush;

            LeftBar.Value = 100;
            RightBar.Value = 0;
        }

        private void SetComplete()
        {
            var completeColorBrush = new SolidColorBrush(CompleteColor);

            Step.Stroke = completeColorBrush;
            Step.Fill = completeColorBrush;

            NameStep.Foreground = new SolidColorBrush(DefaultColor);
            StepContent.Foreground = new SolidColorBrush(Colors.White);

            LeftBar.Value = 100;
            RightBar.Value = 100;
        }
    }
}
