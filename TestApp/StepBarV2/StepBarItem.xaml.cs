using System;
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

            WaitingColor = Colors.LightGray;
            ActiveColor = Colors.RoyalBlue;
            CompleteColor = Colors.RoyalBlue;
            DefaultColor = Colors.Black;

            Status = Status.Waiting;
        }

        public static DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(nameof(ActiveContent), typeof(FrameworkElement), typeof(StepBarV1.StepBarItem), new PropertyMetadata(null, null));
        public FrameworkElement ActiveContent
        {
            get
            {
                var content = (FrameworkElement) GetValue(ContentProperty);
                if (content == null)
                    return new TextBlock()
                    {
                        Style = Resources["TextBlockBaseStyle"] as Style,
                        Text = ((Parent as ItemsControl)?.ItemContainerGenerator.IndexFromContainer(this) + 1).ToString()
                    };

                return content;
            }
            set
            {
                SetValue(ContentProperty, value);
                StepContent.Content = ActiveContent;
            }
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
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NameStep.Text = _description;
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
            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);

            RightBar.BeginAnimation(RangeBase.ValueProperty, animation);

            SetComplete();
        }

        public void SetActivePrev()
        {
            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);

            LeftBar.BeginAnimation(RangeBase.ValueProperty, animation);

            SetWaiting();
        }

        public void SetActiveLeftWithAnimation()
        {
            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);

            LeftBar.BeginAnimation(RangeBase.ValueProperty, animation);
            
            SetActive();
        }

        public void SetActiveRightWithAnimation()
        {
            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);

            RightBar.BeginAnimation(RangeBase.ValueProperty, animation);
            
            SetActive();
        }

        private void SetWaiting()
        {
            var waitingColorBrush = new SolidColorBrush(WaitingColor);

            Step.Stroke = waitingColorBrush;
            Step.Fill = new SolidColorBrush(Colors.White);

            NameStep.Foreground = waitingColorBrush;
            //NumberStep.Foreground = waitingColorBrush;
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
            //NumberStep.Foreground = activeColorBrush;
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
            //NumberStep.Foreground = new SolidColorBrush(Colors.White);
            StepContent.Foreground = new SolidColorBrush(Colors.White);

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
}
