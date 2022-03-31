using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace TestApp
{
    public partial class StepBarItem : UserControl
    {
        public StepBarItem()
        {
            InitializeComponent();
        }

        private Status _status;
        public Status Status
        {
            get => _status;
            set
            {
                switch (value)
                {
                    case Status.NotActive:
                        SetNotActive();
                        break;
                    case Status.Active:
                        SetActive();
                        break;
                    case Status.Complete:
                        SetComplete();
                        break;
                }

                _status = value;
            }
        }

        public Color DefaultColor { get; set; }
        public Color ActiveColor { get; set; }

        private Color _notActiveColor;
        public Color NotActiveColor 
        {
            get => _notActiveColor;
            set
            {
                _notActiveColor = value;

                Bar.Background = new SolidColorBrush(value);
                Bar.BorderBrush = new SolidColorBrush(value);
            }
        }

        private Color _completeColor;
        public Color CompleteColor
        {
            get => _completeColor;
            set
            {
                _completeColor = value;

                Bar.Foreground = new SolidColorBrush(value);
            }
        }

        public void SetActiveWithAnimation()
        {
            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            Bar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public void SetNotActiveWithAnimation()
        {
            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            Bar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        private void SetActive()
        {
            var activeColorBrush = new SolidColorBrush(ActiveColor);

            Step.Fill = new SolidColorBrush(Colors.White);
            Step.Stroke = activeColorBrush;
            NumberStep.Foreground = activeColorBrush;
            NameStep.Foreground = activeColorBrush;

            Bar.Value = 100;
        }

        private void SetNotActive()
        {
            var notActiveColorBrush = new SolidColorBrush(NotActiveColor);

            Step.Fill = new SolidColorBrush(Colors.White);
            Step.Stroke = notActiveColorBrush;
            NumberStep.Foreground = notActiveColorBrush;
            NameStep.Foreground = notActiveColorBrush;

            Bar.Value = 0;
        }

        private void SetComplete()
        {
            var completeColorBrush = new SolidColorBrush(CompleteColor);

            Step.Fill = completeColorBrush;
            Step.Stroke = completeColorBrush;
            NumberStep.Foreground = new SolidColorBrush(Colors.White);
            NameStep.Foreground = new SolidColorBrush(DefaultColor);

            Bar.Value = 100;
        }
    }

    public enum Status
    {
        NotActive,
        Active,
        Complete
    }
}
