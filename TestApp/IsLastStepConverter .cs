using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using TestApp.StepBarV2;

namespace TestApp
{
    public class IsLastStepConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stepBarItem = value as StepBarItem;

            if (stepBarItem == null || stepBarItem.Visibility == Visibility.Collapsed)
                return false;

            var stepBar = stepBarItem.Parent as StepBar;

            var lastStepBarItem = stepBar.Items.OfType<StepBarItem>().Last(x => x.Visibility != Visibility.Collapsed);
            var lastVisibleIndex = stepBar?.ItemContainerGenerator.IndexFromContainer(lastStepBarItem);
            var index = stepBar?.ItemContainerGenerator.IndexFromContainer(stepBarItem);

            return index == lastVisibleIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}