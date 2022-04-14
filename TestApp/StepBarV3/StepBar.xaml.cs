using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApp.StepBarV3
{
    [TemplatePart(Name = ElementProgressBarBack, Type = typeof(ProgressBar))]
    public partial class StepBar
    {
        private const string ElementProgressBarBack = "PART_ProgressBarBack";
        private ProgressBar _backProgressBar;

        public StepBar()
        {
            InitializeComponent();

            var items = Items as INotifyCollectionChanged;
            items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResizeProgressBar();
        }

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(nameof(CurrentStep), typeof(int), typeof(StepBar), new PropertyMetadata(0, CurrentStepChanged, CoerceCurrentStep));

        private static object CoerceCurrentStep(DependencyObject dependencyObject, object basevalue)
        {
            var value = (int)basevalue;
            if (value < 0)
                return 0;

            return value;
        }

        private static void CurrentStepChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stepBar = dependencyObject as StepBar;

            if (stepBar == null || stepBar._backProgressBar == null)
                return;

            var animation = new DoubleAnimation((int)dependencyPropertyChangedEventArgs.NewValue, TimeSpan.FromSeconds(0.2));
            stepBar._backProgressBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }

        public int CurrentStep
        {
            get => (int)GetValue(CurrentStepProperty);
            set => SetValue(CurrentStepProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _backProgressBar = GetTemplateChild(ElementProgressBarBack) as ProgressBar;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ResizeProgressBar();
        }

        private void ResizeProgressBar()
        {
            var colCount = Items.Count;

            if (_backProgressBar == null || colCount <= 0) return;
            _backProgressBar.Maximum = colCount - 1;
            _backProgressBar.Value = CurrentStep;

            _backProgressBar.Width = (colCount - 1) * (ActualWidth / colCount);
        }
    }
}
