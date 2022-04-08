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
            var p = this.Parent as StepBar;
            if(p != null)
                p.UpdateCurrentStep();

            //RenderProgressBar();
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

                RenderProgressBar();
            }
        }

        private void RenderProgressBar()
        {
            var p = Parent as ItemsControl;
            var res = p?.ItemContainerGenerator.IndexFromContainer(this) ?? -1;

            var stepBarItemCollection = p?.Items.OfType<StepBarItem>().ToList();
            var firstVisibilityItem = stepBarItemCollection?.First(x => x.Visibility != Visibility.Collapsed);
            var lastVisibilityItem = stepBarItemCollection?.Last(x => x.Visibility != Visibility.Collapsed);

            var firstIndex = p?.ItemContainerGenerator.IndexFromContainer(firstVisibilityItem);
            var lastIndex = p?.ItemContainerGenerator.IndexFromContainer(lastVisibilityItem);

            LeftBar.Visibility = res == firstIndex ? Visibility.Collapsed : Visibility.Visible;
            RightBar.Visibility = res == lastIndex ? Visibility.Collapsed : Visibility.Visible;
        }

        public Color ActiveColor { get; set; }
        public Color DefaultColor { get; set; }

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
