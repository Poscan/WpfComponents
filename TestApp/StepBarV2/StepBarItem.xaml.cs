using System;
using System.ComponentModel;
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
            if (Parent is StepBar stepBar)
            {
                SetWaiting(0);
                _status = Status.Waiting;

                stepBar.UpdateCurrentStep(0);
                UpdateProgressBars();
            }
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
                if (_status == value)
                    return;

                _status = value;

                switch (_status)
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
            }
        }

        public void UpdateProgressBars()
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

        private Color _activeColor;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color ActiveColor
        {
            get => _activeColor;
            set
            {
                if (_activeColor == value)
                    return;

                _activeColor = value;

                if (Status == Status.Active)
                {
                    SetActive();
                }
            }
        }

        private Color _defaultColor;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                if (_defaultColor == value)
                    return;

                _defaultColor = value;

                if (Status == Status.Complete)
                {
                    SetComplete(0);
                }
            }
        }

        private Color _waitingColor;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color WaitingColor
        {
            get => _waitingColor;
            set
            {
                if (_waitingColor == value)
                    return;

                _waitingColor = value;

                var waitingColorBrush = new SolidColorBrush(value);

                LeftBar.Background = waitingColorBrush;
                LeftBar.BorderBrush = waitingColorBrush;

                RightBar.Background = waitingColorBrush;
                RightBar.BorderBrush = waitingColorBrush;

                if (Status == Status.Waiting)
                {
                    SetWaiting(0);
                }
            }
        }

        private Color _completeColor;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color CompleteColor
        {
            get => _completeColor;
            set
            {
                if (_completeColor == value)
                    return;

                _completeColor = value;

                var completeColorBrush = new SolidColorBrush(value);

                LeftBar.Foreground = completeColorBrush;
                RightBar.Foreground = completeColorBrush;

                if (Status == Status.Complete)
                {
                    SetComplete(0);
                }
            }
        }

        private void ProgressBarBeginAnimation(ProgressBar bar, int toValue)
        {
            var animation = new DoubleAnimation(toValue, TimeSpan.FromSeconds(0.1), FillBehavior.HoldEnd);
            animation.BeginTime = TimeSpan.FromSeconds(0.1);

            SetActive();
            bar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetActiveLeftWithAnimation()
        {
            ProgressBarBeginAnimation(LeftBar, 100);

            var animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0), FillBehavior.HoldEnd);
            RightBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetActiveRightWithAnimation()
        {
            ProgressBarBeginAnimation(RightBar, 0);
            var animation = new DoubleAnimation(100, TimeSpan.FromSeconds(0), FillBehavior.HoldEnd);
            LeftBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        private void SetActive()
        {
            var activeColorBrush = new SolidColorBrush(ActiveColor);

            Step.Stroke = activeColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = activeColorBrush;
            StepContent.Foreground = activeColorBrush;
        }

        public void SetWaiting(double duration = 0.1)
        {
            var waitingColorBrush = new SolidColorBrush(WaitingColor);

            Step.Stroke = waitingColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = waitingColorBrush;
            StepContent.Foreground = waitingColorBrush;

            var leftAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(duration), FillBehavior.HoldEnd);
            LeftBar.BeginAnimation(RangeBase.ValueProperty, leftAnimation);
            var rightAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0), FillBehavior.HoldEnd);
            RightBar.BeginAnimation(RangeBase.ValueProperty, rightAnimation);

            _status = Status.Waiting;
        }

        public void SetComplete(double duration = 0.1)
        {
            var completeColorBrush = new SolidColorBrush(CompleteColor);

            Step.Stroke = completeColorBrush;
            Step.Fill = completeColorBrush;

            NameStep.Foreground = new SolidColorBrush(DefaultColor);
            StepContent.Foreground = new SolidColorBrush(Colors.White);

            var rightAnimation = new DoubleAnimation(100, TimeSpan.FromSeconds(duration), FillBehavior.HoldEnd);
            RightBar.BeginAnimation(RangeBase.ValueProperty, rightAnimation);
            var leftAnimation = new DoubleAnimation(100, TimeSpan.FromSeconds(0), FillBehavior.HoldEnd);
            LeftBar.BeginAnimation(RangeBase.ValueProperty, leftAnimation);

            _status = Status.Complete;
        }
    }
}
