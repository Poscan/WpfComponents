﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using TestApp.StepBarV2;

namespace TestApp
{
    public class IsFirstStepConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stepBarItem = value as StepBarItem;

            if (stepBarItem == null)
                return false;

            var itemsControl = stepBarItem?.Parent as ItemsControl;
            var index = itemsControl?.ItemContainerGenerator.IndexFromContainer(stepBarItem);

            return index == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}