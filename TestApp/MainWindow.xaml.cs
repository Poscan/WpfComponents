using System.Collections.Generic;
using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> labels = new List<string>() { "Подписать врачом", "Подписать МО", "В обработке", "Ошибка отправки" };
            StepBarControla.Labels = labels;
        }

        public int Step { get; set; }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (StepBarControla.CurrentStep != 0)
                StepBarControla.CurrentStep--;
        }

        private void NExt(object sender, RoutedEventArgs e)
        {
            StepBarControla.CurrentStep++;
        }
    }
}
