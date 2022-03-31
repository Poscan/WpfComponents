using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TestApp
{
    public partial class StepBarControl : UserControl
    {
        public StepBarControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty CountStepProperty = DependencyProperty.Register(nameof(CountStep), typeof(int), typeof(StepBarControl),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(CountStepChangedCallback)));

        private static void CountStepChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stepBarControl = dependencyObject as StepBarControl;

            var countStep = stepBarControl?.CountStep ?? 1;
            stepBarControl?.RenderStepBar(countStep < 0 ? 1 : countStep);
        }

        public int CountStep
        {
            get => (int)GetValue(CountStepProperty);
            set
            {
                var countStep = value < 0 ? 1 : value;
                
                SetValue(CountStepProperty, countStep);
            }
        }

        public static DependencyProperty CurrentStepProperty = DependencyProperty.Register(nameof(CurrentStep), typeof(int),
            typeof(StepBarControl),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(CurrentStepChangedCallback)));

        private static void CurrentStepChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stepBarControl = dependencyObject as StepBarControl;

            var currentStep = stepBarControl?.CurrentStep ?? 0;
            currentStep = currentStep < 0 ? 0 : currentStep;
            currentStep = currentStep > stepBarControl?.CountStep ? (int)stepBarControl?.CountStep : currentStep;

            if (currentStep > (int)dependencyPropertyChangedEventArgs.OldValue)
            {
                stepBarControl?.UpdateCurrentStep(currentStep);
            }
            else
            {
                stepBarControl?.PrevCurrentStep(currentStep);
            }
        }

        public int CurrentStep
        {
            get => (int)GetValue(CurrentStepProperty);
            set
            {
                var currentStep = value < 0 ? 0 : value;
                currentStep = currentStep > CountStep ? CountStep : currentStep;
                SetValue(CurrentStepProperty, currentStep);
            }
        }

        private void RenderStepBar(int countStep)
        {
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();

            var columnEllipse = new ColumnDefinition { Width = new GridLength(30) };

            var ellipse = new Ellipse();
            var ellipseStyle = Resources["EllipseBaseStyle"] as Style;
            ellipse.Style = ellipseStyle;

            var text = new TextBlock();
            var textStyle = Resources["TextBaseStyle"] as Style;
            text.Style = textStyle;
            text.Text = "1";

            MainGrid.ColumnDefinitions.Add(columnEllipse);

            MainGrid.Children.Add(ellipse);
            MainGrid.Children.Add(text);

            Grid.SetColumn(ellipse, 0);
            Grid.SetColumn(text, 0);

            for (var i = 0; i < countStep - 1; i++)
            {
                var columnProgressNew = new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) };
                var columnEllipseNew = new ColumnDefinition { Width = new GridLength(30) };

                var progressBar = new ProgressBar();
                progressBar.Style = Resources["ProgressBarBaseStyle"] as Style;

                var ellipseNew = new Ellipse();
                ellipseNew.Style = ellipseStyle;

                var textNew = new TextBlock();
                textNew.Style = textStyle;
                textNew.Text = (i + 2).ToString();

                MainGrid.ColumnDefinitions.Add(columnProgressNew);
                MainGrid.ColumnDefinitions.Add(columnEllipseNew);

                MainGrid.Children.Add(progressBar);
                MainGrid.Children.Add(ellipseNew);
                MainGrid.Children.Add(textNew);

                Grid.SetColumn(progressBar, i * 2 + 1);
                Grid.SetColumn(ellipseNew, (i + 1) * 2);
                Grid.SetColumn(textNew, (i + 1) * 2);
            }

            var text2 = new TextBlock();
            var text3 = new TextBlock();
            text2.Style = Resources["TextHeaderBaseStyle"] as Style;
            text3.Style = Resources["TextHeaderBaseStyle"] as Style;
            text2.Text = "Подписать врачом";
            text3.Text = "Подписать МО";

            UpdateCurrentStep(CurrentStep);
            MainGrid.Children.Add(text2);
            MainGrid.Children.Add(text3);
            Grid.SetColumn(text2, 0);
            Grid.SetColumn(text3, 2);
        }

        private void UpdateCurrentStep(int currentStep)
        {
            for (var i = 0; i < currentStep; i++)
            {
                SetCompleteStep(i);
            }

            for (var i = currentStep + 1; i < CountStep; i++)
            {
                SetNotActiveStep(i);
            }

            SetActiveStep(currentStep);
        }

        private void SetActiveStep(int step)
        {
            if (step >= CountStep)
                return;

            if (step != 0)
            {
                ProgressFill(step);
            }

            GetEllipseByStep(step).Style = Resources["EllipseActiveStyle"] as Style;
            GetTextBlockByStep(step).Style = Resources["TextNotCompleteStyle"] as Style;
        }

        private void PrevCurrentStep(int currentStep)
        {
            for (var i = 0; i <= currentStep; i++)
            {
                SetCompleteStep(i);
            }

            for (var i = currentStep + 1; i < CountStep; i++)
            {
                SetNotActiveStep(i);
            }

            SetPrevActiveStep(currentStep);
        }

        private void SetPrevActiveStep(int step)
        {
            if (step >= CountStep)
                return;

            if (step >= 0)
            {
                ProgressNotFill(step + 1);
            }

            GetEllipseByStep(step).Style = Resources["EllipseActiveStyle"] as Style;
            GetTextBlockByStep(step).Style = Resources["TextNotCompleteStyle"] as Style;
        }


        private void SetNotActiveStep(int step)
        {
            if (step != 0 && step < CountStep)
            {
                GetProgressBarByStep(step).Value = 0;
            }

            GetEllipseByStep(step).Style = Resources["EllipseNotActiveStyle"] as Style;
            GetTextBlockByStep(step).Style = Resources["TextNotCompleteStyle"] as Style;
        }

        private void SetCompleteStep(int step)
        {
            if (step >= CountStep)
                return;

            if (step != 0 && step < CountStep)
            {
                GetProgressBarByStep(step).Value = 100;
            }

            GetEllipseByStep(step).Style = Resources["EllipseCompleteStyle"] as Style;
            GetTextBlockByStep(step).Style = Resources["TextCompleteStyle"] as Style;
        }

        private ProgressBar GetProgressBarByStep(int step)
        {
            if (step == 0 || step >= CountStep)
                return null;

            var c = MainGrid.Children;
            return MainGrid.Children[step * 3 - 1] as ProgressBar;
        }

        private Ellipse GetEllipseByStep(int step)
        {
            if (step >= CountStep)
                return null;

            return MainGrid.Children[step * 3] as Ellipse;
        }

        private TextBlock GetTextBlockByStep(int step)
        {
            if (step >= CountStep)
                return null;

            return MainGrid.Children[step * 3 + 1] as TextBlock;
        }

        private void ProgressFill(int step)
        {
            var animation = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(0.35), FillBehavior.Stop);
            var progressBar = GetProgressBarByStep(step);
            progressBar.BeginAnimation(RangeBase.ValueProperty, animation);
            progressBar.Value = 100;
        }

        private void ProgressNotFill(int step)
        {
            var animation = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(0.35), FillBehavior.Stop);
            var progressBar = GetProgressBarByStep(step);
            if (progressBar == null)
                return;
            progressBar.BeginAnimation(RangeBase.ValueProperty, animation);
            progressBar.Value = 0;
        }
    }
}