using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestApp.StepBarV2
{
    public partial class StepBar : ItemsControl
    {
        public StepBar()
        {
            InitializeComponent();

            var items = Items as INotifyPropertyChanged;
            items.PropertyChanged += ItemsCollectionChanged;
        }

        ~StepBar()
        {
            var items = Items as INotifyPropertyChanged;
            items.PropertyChanged -= ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, PropertyChangedEventArgs e)
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

        private async Task UpdateCurrentStep(int oldStep = 0)
        {
            var stepBarItems = Items;

            for (var i = 0; i < stepBarItems.Count; i++)
            {
                var stepBarItem = stepBarItems[i] as StepBarItem;

                if(stepBarItem == null || i == CurrentStep)
                    continue;
                //stepBarItem.ActiveColor = ActiveColor;
                //stepBarItem.CompleteColor = CompleteColor;
                //stepBarItem.DefaultColor = DefaultColor;
                //stepBarItem.NotActiveColor = NotActiveColor;
                //stepBarItem.ActiveContent = ActiveContent;

                stepBarItem.Status = i < CurrentStep ? Status.Complete : Status.Waiting;
            }

            if (CurrentStep >= Items.Count || CurrentStep < 0)
                return;

            var currentStepItem = Items[CurrentStep] as StepBarItem;

            if (CurrentStep > oldStep)
            {
                if (CurrentStep > 0)
                {
                    var prevStep = Items[CurrentStep - 1] as StepBarItem;
                    prevStep?.SetActiveNext();

                    await Task.Delay(90);
                }

                currentStepItem?.SetActiveLeftWithAnimation();
            }
            else
            {
                if (CurrentStep + 1 < Items.Count)
                {
                    var nextStep = Items[CurrentStep + 1] as StepBarItem;
                    nextStep?.SetActivePrev();
                }

                await Task.Delay(90);
                currentStepItem?.SetActiveRightWithAnimation();
            }
        }

        public int CurrentStep
        {
            get => (int) GetValue(CurrentStepProperty);
            set
            {
                if (CurrentStep == value)
                    return;

                var currentStep = value < 0 ? 0 : value;
                currentStep = currentStep > Items.Count ? Items.Count : currentStep;
                SetValue(CurrentStepProperty, currentStep);
            }
        }
    }
}