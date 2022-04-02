using System;
using System.Windows;
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

            WaitingColor = Colors.LightGray;
            ActiveColor = Colors.RoyalBlue;
            CompleteColor = Colors.RoyalBlue;
            DefaultColor = Colors.Black;

            Position = Position.Center;
            Status = Status.Waiting;
        }

        private static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(Status), typeof(StepBarItem), new PropertyMetadata(StepBarV2.Status.Waiting));

        public Status Status
        {
            get => (Status)GetValue(StatusProperty);
            set
            {
                SetValue(StatusProperty, value);

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
            }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Position), typeof(StepBarItem), new PropertyMetadata(Position.Center, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is StepBarItem stepBarItem))
                return;

            stepBarItem.Position = (Position) e.NewValue;
        }

        public Position Position
        {
            get => (Position) GetValue(PositionProperty);
            set
            {
                if (value == Position)
                    return;

                SetValue(PositionProperty, value);

                switch (value)
                {
                    case Position.Head:
                        LeftBar.Visibility = Visibility.Collapsed;
                        RightBar.Visibility = Visibility.Visible;
                        break;
                    case Position.Center:
                        LeftBar.Visibility = Visibility.Visible;
                        RightBar.Visibility = Visibility.Visible;
                        break;
                    case Position.Tail:
                        LeftBar.Visibility = Visibility.Visible;
                        RightBar.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

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

        public Color ActiveColor { get; set; }

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

        public Color DefaultColor { get; set; }

        public void SetActiveNext()
        {
            SetComplete();

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            RightBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetActivePrev()
        {
            SetWaiting();

            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            LeftBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetActiveLeftWithAnimation()
        {
            SetActive();

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            LeftBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetActiveRightWithAnimation()
        {
            SetActive();

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            RightBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        private void SetWaiting()
        {
            var waitingColorBrush = new SolidColorBrush(WaitingColor);

            Step.Stroke = waitingColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = waitingColorBrush;
            NumberStep.Foreground = waitingColorBrush;

            LeftBar.Value = 0;
            RightBar.Value = 0;
        }

        private void SetActive()
        {
            var activeColorBrush = new SolidColorBrush(ActiveColor);

            Step.Stroke = activeColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = activeColorBrush;
            NumberStep.Foreground = activeColorBrush;

            LeftBar.Value = 100;
            RightBar.Value = 0;
        }

        private void SetComplete()
        {
            var completeColorBrush = new SolidColorBrush(CompleteColor);

            Step.Stroke = completeColorBrush;
            Step.Fill = completeColorBrush;

            NameStep.Foreground = new SolidColorBrush(DefaultColor);
            NumberStep.Foreground = new SolidColorBrush(Colors.White);

            LeftBar.Value = 100;
            RightBar.Value = 100;
        }
    }

    public enum Status
    {
        Waiting,
        Active,
        Complete
    }

    public enum Position
    {
        Head,
        Center,
        Tail
    }
}
