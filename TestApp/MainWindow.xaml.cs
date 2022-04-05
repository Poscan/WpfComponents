using System.Collections.Generic;
using System.Windows;

namespace TestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> labels = new List<string>() { "Подписать врачом", "Подписать МО", "В обработке", "Ошибка отправки" };
            //StepBarControla.Labels = labels;
        }

        public int Step { get; set; }

        private void PrevButtonOnClick(object sender, RoutedEventArgs e)
        {
            StepBarControla.CurrentStep--;
        }

        private void NextButtonOnClick(object sender, RoutedEventArgs e)
        {
            StepBarControla.CurrentStep++;
        }
    }
}
