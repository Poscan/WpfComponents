using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestApp.StepBarV2
{
    public partial class StepBar
    {
        public StepBar()
        {
            InitializeComponent();

            var items = Items as INotifyCollectionChanged;
            items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateCurrentStep();
            (Items[0] as StepBarItem)?.UpdateProgressBars();
        }

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(nameof(CurrentStep), typeof(int), typeof(StepBar), new PropertyMetadata(0, CurrentStepChangedCallback, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            var value = (int)baseValue;

            return value < 0 ? 0 : value;
        }

        private static void CurrentStepChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is StepBar stepBarList))
                return;

            var value = (int)dependencyPropertyChangedEventArgs.NewValue;
            var itemsCount = stepBarList.VisibilityItems.Count;
            if (itemsCount == 0)
                return;

            var currentStep = value > itemsCount ? itemsCount : value;

            stepBarList._prevStep = (int)dependencyPropertyChangedEventArgs.OldValue;
            stepBarList._currentStep = currentStep;

            stepBarList.UpdateCurrentStep();
            if (stepBarList.CurrentStep < stepBarList.Items.Count && stepBarList.CurrentStep >= 0)
            {
                stepBarList.StartAnimation();
            }
        }

        private int _currentStep;
        public int CurrentStep
        {
            get => (int)GetValue(CurrentStepProperty);
            set
            {
                if (_currentStep == value)
                    return;

                var itemsCount = VisibilityItems.Count;
                var currentStep = value > itemsCount ? itemsCount : value;

                _prevStep = _currentStep;
                _currentStep = currentStep;
                SetValue(CurrentStepProperty, currentStep);
            }
        }

        private int _prevStep;

        public static readonly DependencyProperty ActiveColorProperty = DependencyProperty.Register(nameof(ActiveColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.RoyalBlue, ColorChangedCallback));

        public Color ActiveColor
        {
            get => (Color)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public static readonly DependencyProperty WaitingColorProperty = DependencyProperty.Register(nameof(WaitingColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.LightGray, ColorChangedCallback));

        public Color WaitingColor
        {
            get => (Color)GetValue(WaitingColorProperty);
            set => SetValue(WaitingColorProperty, value);
        }

        public static readonly DependencyProperty CompleteColorProperty = DependencyProperty.Register(nameof(CompleteColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.RoyalBlue, ColorChangedCallback));

        public Color CompleteColor
        {
            get => (Color)GetValue(CompleteColorProperty);
            set => SetValue(CompleteColorProperty, value);
        }

        public static readonly DependencyProperty DefaultColorProperty = DependencyProperty.Register(nameof(DefaultColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.Black, ColorChangedCallback));

        public Color DefaultColor
        {
            get => (Color)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        private static void ColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is StepBar stepBarList) || stepBarList.Items.Count == 0)
                return;

            stepBarList.UpdateCurrentStep();
        }

        public IList<StepBarItem> VisibilityItems => Items.OfType<StepBarItem>().Where(x => x.Visibility == Visibility.Visible).ToList();

        public void UpdateCurrentStep(double duration = 0.1)
        {
            var visibilityItems = VisibilityItems;

            for (var currentIndex = 0; currentIndex < visibilityItems.Count; currentIndex++)
            {
                var stepBarItem = visibilityItems[currentIndex];

                stepBarItem.ActiveColor = ActiveColor;
                stepBarItem.CompleteColor = CompleteColor;
                stepBarItem.DefaultColor = DefaultColor;
                stepBarItem.WaitingColor = WaitingColor;

                if (stepBarItem.ActiveContent == null)
                {
                    stepBarItem.ActiveContent = new TextBlock()
                    {
                        Text = (currentIndex + 1).ToString()
                    };
                }

                if (currentIndex != CurrentStep)
                {
                    if (currentIndex < CurrentStep)
                    {
                        stepBarItem.SetComplete(duration);
                    }
                    else
                    {
                        stepBarItem.SetWaiting(duration);
                    }
                }
            }

            StartAnimation();
        }

        private void StartAnimation()
        {
            var visibleItems = VisibilityItems;
            if (visibleItems.Count < CurrentStep + 1 || CurrentStep < 0)
                return;

            var currentStepItem = visibleItems[CurrentStep];

            if (CurrentStep > _prevStep)
            {
                currentStepItem?.SetActiveLeftWithAnimation();
            }
            else
            {
                currentStepItem?.SetActiveRightWithAnimation();
            }

            if (currentStepItem != null)
            {
                currentStepItem.Status = Status.Active;
            }
        }
    }
}