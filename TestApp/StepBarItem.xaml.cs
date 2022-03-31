using System;
using System.Windows;
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
                if(_status == value)
                    return;

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

        public Color ActiveColor
        {
            get
            {
                var stepBarList = Parent as StepBarList;
                return stepBarList?.ActiveColor ?? Colors.RoyalBlue;
            }
        }

        public Color NotActiveColor 
        {
            get
            {
                var stepBarList = Parent as StepBarList;
                return stepBarList?.NotActiveColor ?? Colors.LightGray;
            }
        }

        public Color CompleteColor
        {
            get
            {
                var stepBarList = Parent as StepBarList;
                return stepBarList?.CompleteColor ?? Colors.RoyalBlue;
            }
        }

        public Color DefaultColor
        {
            get
            {
                var stepBarList = Parent as StepBarList;
                return stepBarList?.DefaultColor ?? Colors.Black;
            }
        }

        private void SetActive()
        {
            Step.Style = Resources["EllipseActiveStyle"] as Style;
            NumberStep.Style = Resources["TextActiveStyle"] as Style;
            NameStep.Style = Resources["HeaderActiveStyle"] as Style;

            Bar.Value = 100;
        }

        public void SetActiveWithAnimation()
        {
            Step.Style = Resources["EllipseActiveStyle"] as Style;
            NumberStep.Style = Resources["TextActiveStyle"] as Style;
            NameStep.Style = Resources["HeaderActiveStyle"] as Style;

            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            Bar.BeginAnimation(RangeBase.ValueProperty, animation);
            Bar.Value = 100;
        }

        private void SetNotActive()
        {
            Step.Style = Resources["EllipseNotActiveStyle"] as Style;
            NumberStep.Style = Resources["TextNotActiveStyle"] as Style;
            NameStep.Style = Resources["HeaderNotActiveStyle"] as Style;

            Bar.Value = 0;
        }

        public void SetNotActiveWithAnimation()
        {
            Step.Style = Resources["EllipseNotActiveStyle"] as Style;
            NumberStep.Style = Resources["TextNotActiveStyle"] as Style;
            NameStep.Style = Resources["HeaderNotActiveStyle"] as Style;

            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.2), FillBehavior.Stop);

            Bar.BeginAnimation(RangeBase.ValueProperty, animation);
            Bar.Value = 0;
        }

        private void SetComplete()
        {
            Step.Style = Resources["EllipseCompleteStyle"] as Style;
            NumberStep.Style = Resources["TextCompleteStyle"] as Style;
            NameStep.Style = Resources["HeaderCompleteStyle"] as Style;

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
