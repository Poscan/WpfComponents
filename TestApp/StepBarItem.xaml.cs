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
            Step.Fill = new SolidColorBrush(Colors.White);
            Step.Stroke = new SolidColorBrush(ActiveColor);
            NumberStep.Foreground = new SolidColorBrush(ActiveColor);
            NameStep.Foreground = new SolidColorBrush(ActiveColor);

            Bar.Value = 100;
        }

        private void SetNotActive()
        {
            Step.Fill = new SolidColorBrush(Colors.White);
            Step.Stroke = new SolidColorBrush(NotActiveColor);
            NumberStep.Foreground = new SolidColorBrush(NotActiveColor);
            NameStep.Foreground = new SolidColorBrush(NotActiveColor);

            Bar.Value = 0;
        }

        private void SetComplete()
        {
            Step.Fill = new SolidColorBrush(CompleteColor);
            Step.Stroke = new SolidColorBrush(CompleteColor);
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
