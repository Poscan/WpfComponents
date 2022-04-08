using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
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
        }

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(nameof(CurrentStep), typeof(int), typeof(StepBar), new PropertyMetadata(1, CurrentStepChangedCallback));

        private static void CurrentStepChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is StepBar stepBarList))
                return;

            stepBarList.CurrentStep = (int) dependencyPropertyChangedEventArgs.NewValue;
            stepBarList.UpdateCurrentStep((int) dependencyPropertyChangedEventArgs.OldValue);
        }

        public int CurrentStep
        {
            get => (int)GetValue(CurrentStepProperty);
            set
            {
                if (CurrentStep == value)
                    return;

                var currentStep = value < 0 ? 0 : value;
                var itemsCount = VisibilityItems.Count;
                currentStep = currentStep > itemsCount ? itemsCount : currentStep;

                SetValue(CurrentStepProperty, currentStep);
            }
        }

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

        public void UpdateCurrentStep(int oldStep = 0)
        {
            var visibilityItems = VisibilityItems;
            for (int currentIndex = 0; currentIndex < visibilityItems.Count; currentIndex++)
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
                        Text = (currentIndex + 1).ToString(),
                    };
                }

                if(currentIndex != CurrentStep)
                {
                    stepBarItem.Status = currentIndex < CurrentStep ? Status.Complete : Status.Waiting;
                }
            }

            if(CurrentStep < Items.Count && CurrentStep >= 0)
            {
                StartAnimation(oldStep);
            }
        }

        private async Task StartAnimation(int oldStep)
        {
            var visibleItems = VisibilityItems;
            if (visibleItems.Count < CurrentStep + 1)
                return;

            var currentStepItem = visibleItems[CurrentStep];

            if (CurrentStep > oldStep)
            {
                if (CurrentStep > 0)
                {
                    var prevStep = visibleItems[CurrentStep - 1];
                    prevStep?.SetActiveNext();

                    await Task.Delay(90);
                }

                currentStepItem?.SetActiveLeftWithAnimation();
            }
            else
            {
                if (CurrentStep + 1 < visibleItems.Count)
                {
                    var nextStep = visibleItems[CurrentStep + 1];
                    nextStep?.SetActivePrev();

                    await Task.Delay(90);
                }

                currentStepItem?.SetActiveRightWithAnimation();
            }

            currentStepItem.Status = Status.Active;
        }
    }
}