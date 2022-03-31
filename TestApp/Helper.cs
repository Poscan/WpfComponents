using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TestApp
{
    public static class Helper
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T dependencyObject)
                    yield return dependencyObject;

                foreach (var visualChild in child.FindVisualChildren<T>())
                {
                    var childOfChild = visualChild;
                    yield return childOfChild;
                }
            }
        }
    }
}