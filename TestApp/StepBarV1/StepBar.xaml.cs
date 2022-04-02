using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestApp.StepBarV1
{
    public partial class StepBar
    {
        public StepBar()
        {
            InitializeComponent();
        }

        public static DependencyProperty ActiveColorProperty = DependencyProperty.Register(nameof(ActiveColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.RoyalBlue, ColorChangedCallback));

        public Color ActiveColor
        {
            get => (Color)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public static DependencyProperty NotActiveColorProperty = DependencyProperty.Register(nameof(NotActiveColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.LightGray, ColorChangedCallback));

        public Color NotActiveColor
        {
            get => (Color)GetValue(NotActiveColorProperty);
            set => SetValue(NotActiveColorProperty, value);
        }

        public static DependencyProperty CompleteColorProperty = DependencyProperty.Register(nameof(CompleteColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.RoyalBlue, ColorChangedCallback));

        public Color CompleteColor
        {
            get => (Color)GetValue(CompleteColorProperty);
            set => SetValue(CompleteColorProperty, value);
        }

        public static DependencyProperty DefaultColorProperty = DependencyProperty.Register(nameof(DefaultColor), typeof(Color), typeof(StepBar), new PropertyMetadata(Colors.Black, ColorChangedCallback));

        public Color DefaultColor
        {
            get => (Color)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        private static void ColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is StepBar stepBarList) || stepBarList.MainGrid.Children.Count == 0)
                return;

            stepBarList.UpdateCurrentStep();
        }

        public static DependencyProperty CountStepProperty = DependencyProperty.Register(nameof(CountStep), typeof(int), typeof(StepBar), new PropertyMetadata(1, CountStepChangedCallback));

        private static void CountStepChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stepBarList = dependencyObject as StepBar;

            var countStep = stepBarList?.CountStep ?? 1;
            var labelsCount = stepBarList?.Labels.Count ?? 0;

            stepBarList?.RenderStepBar(Math.Max(countStep, labelsCount));
        }

        public int CountStep
        {
            get => (int)GetValue(CountStepProperty);
            set => SetValue(CountStepProperty, value);
        }

        public static DependencyProperty LabelsProperty = DependencyProperty.Register(nameof(Labels), typeof(List<string>), typeof(StepBar), new PropertyMetadata(new List<string>() ,LabelsChangedCallback));

        private static void LabelsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stepBarList = dependencyObject as StepBar;

            if(stepBarList?.MainGrid?.Children.Count > 0)
            {
                stepBarList?.RenderStepBar(stepBarList.CountStep);
            }
        }

        public List<string> Labels
        {
            get => (List<string>)GetValue(LabelsProperty);
            set
            {
                if(value == Labels)
                    return;

                if (value.Count > CountStep)
                {
                    SetValue(CountStepProperty, value.Count);
                }

                SetValue(LabelsProperty, value);
            }
        }

        public static DependencyProperty CurrentStepProperty = DependencyProperty.Register(nameof(CurrentStep), typeof(int), typeof(StepBar), new PropertyMetadata(1, CurrentStepChangedCallback));

        private static void CurrentStepChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(!(dependencyObject is StepBar stepBarList) || stepBarList.MainGrid.Children.Count == 0)
                return;

            stepBarList.CurrentStep = (int)dependencyPropertyChangedEventArgs.NewValue;
            stepBarList.UpdateCurrentStep((int)dependencyPropertyChangedEventArgs.OldValue);
        }

        public int CurrentStep
        {
            get => (int)GetValue(CurrentStepProperty);
            set
            {
                if (CurrentStep == value)
                    return;

                var currentStep = value < 0 ? 0 : value;
                currentStep = currentStep > CountStep ? CountStep : currentStep;
                SetValue(CurrentStepProperty, currentStep);
            }
        }

        private void UpdateCurrentStep(int oldStep = 0)
        {
            if (CurrentStep <= 0)
            {
                SetActiveFirstStep();
            }
            else
            {
                SetCompleteFirstStep();
            }

            var stepBarItems = MainGrid.FindVisualChildren<StepBarItem>().ToList();

            for (var i = 0; i < stepBarItems.Count; i++)
            {
                var stepBarItem = stepBarItems[i];

                stepBarItem.ActiveColor = ActiveColor;
                stepBarItem.CompleteColor = CompleteColor;
                stepBarItem.DefaultColor = DefaultColor;
                stepBarItem.NotActiveColor = NotActiveColor;

                stepBarItem.Status = i < CurrentStep ? Status.Complete : Status.NotActive;
            }

            if(CurrentStep < CountStep && CurrentStep - 1 >= 0)
            {
                stepBarItems[CurrentStep - 1].Status = Status.Active;
            }

            if (CurrentStep > oldStep)
            {
                NextCurrentStep(CurrentStep);
            }
            else
            {
                PrevCurrentStep(CurrentStep);
            }

            SetLastStep();
        }

        private void NextCurrentStep(int currentStep)
        {
            var stepBarItems = MainGrid.FindVisualChildren<StepBarItem>().ToList();

            if (currentStep < CountStep && currentStep - 1 >= 0)
            {
                stepBarItems[currentStep - 1].SetActiveWithAnimation();
            }
        }

        private void PrevCurrentStep(int currentStep)
        {
            var stepBarItems = MainGrid.FindVisualChildren<StepBarItem>().ToList();

            if (currentStep < CountStep - 1 && currentStep >= 0)
            {
                stepBarItems[currentStep].SetNotActiveWithAnimation();
            }
        }

        private void SetLastStep()
        {
            var stepBarItems = MainGrid.FindVisualChildren<StepBarItem>().ToList();
            var lastStepBarItem = stepBarItems.LastOrDefault();

            if (lastStepBarItem == null)
                return;

            lastStepBarItem.NameStep.Style = Resources["LastStepBarItemStyle"] as Style;

            switch (lastStepBarItem.Status)
            {
                case Status.Active:
                    lastStepBarItem.NameStep.Foreground = new SolidColorBrush(ActiveColor);
                    break;
                case Status.Complete:
                    lastStepBarItem.NameStep.Foreground = new SolidColorBrush(DefaultColor);
                    break;
                case Status.NotActive:
                    lastStepBarItem.NameStep.Foreground = new SolidColorBrush(NotActiveColor);
                    break;
            }
        }

        private void SetCompleteFirstStep()
        {
            var solidColorBrush = new SolidColorBrush(CompleteColor);

            var firstEllipse = GetFirstEllipse();
            firstEllipse.Stroke = solidColorBrush;
            firstEllipse.Fill = solidColorBrush;

            GetFirstNumberText().Foreground = new SolidColorBrush(Colors.White);
            GetFirstNameText().Foreground = new SolidColorBrush(DefaultColor);
        }

        private void SetActiveFirstStep()
        {
            var solidColorBrush = new SolidColorBrush(ActiveColor);

            var firstEllipse = GetFirstEllipse();
            firstEllipse.Stroke = solidColorBrush;
            firstEllipse.Fill = new SolidColorBrush(Colors.White);
            
            GetFirstNameText().Foreground = solidColorBrush;
            GetFirstNumberText().Foreground = solidColorBrush;
        }

        private Ellipse GetFirstEllipse()
        {
            return MainGrid.Children[0] as Ellipse;
        }

        private TextBlock GetFirstNumberText()
        {
            return MainGrid.Children[1] as TextBlock;
        }

        private TextBlock GetFirstNameText()
        {
            return MainGrid.Children[2] as TextBlock;
        }

        private void RenderStepBar(int countStep)
        {
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.Children.Clear();

            CreateFirstStep();

            for (var i = 1; i < countStep; i++)
            {
                var columnEllipse = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                MainGrid.ColumnDefinitions.Add(columnEllipse);

                var stepBarItem = new StepBarItem();
                stepBarItem.NumberStep.Text = (i + 1).ToString();
                stepBarItem.NameStep.Text = Labels?.Count > i ? Labels[i] : string.Empty;
                stepBarItem.Status = Status.NotActive;

                MainGrid.Children.Add(stepBarItem);

                Grid.SetColumn(stepBarItem, i);
                Grid.SetRowSpan(stepBarItem, 2);
            }

            UpdateCurrentStep();
        }

        private void CreateFirstStep()
        {
            var gridLength = new GridLength(30);
            var firstColumn = new ColumnDefinition { Width = gridLength };
            var firstRow = new RowDefinition { Height = gridLength };
            var secondRow = new RowDefinition { Height = gridLength };

            MainGrid.ColumnDefinitions.Add(firstColumn);
            MainGrid.RowDefinitions.Add(firstRow);
            MainGrid.RowDefinitions.Add(secondRow);

            var ellipse = new Ellipse();
            ellipse.Height = 30;
            ellipse.Width = 30;

            var numberStep = new TextBlock();
            numberStep.Text = "1";
            numberStep.HorizontalAlignment = HorizontalAlignment.Center;
            numberStep.VerticalAlignment = VerticalAlignment.Center;

            var nameStep = new TextBlock();
            nameStep.Text = Labels.Count > 0 ? Labels[0] : string.Empty;
            nameStep.HorizontalAlignment = HorizontalAlignment.Left;
            nameStep.Margin = new Thickness(-20, 6, -100, 0);

            MainGrid.Children.Add(ellipse);
            MainGrid.Children.Add(numberStep);
            MainGrid.Children.Add(nameStep);

            Grid.SetRow(nameStep, 1);
        }
    }
}
