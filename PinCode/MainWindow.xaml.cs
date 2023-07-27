using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PinCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            RandomByteDetector detector = new(250);
            string code = CodeTextBox.Text;
            Task<string> task = new(() => CodeGenerator.GenerateCode(code, detector,
                new Calculator(detector, new SubstitutionFactory(detector))
            ));
            task.Start();
            await task;
            GeneratedCodeLabel.Content = task.Result;
        }
        private void CodeTextBoxChangedCallback(object sender, TextChangedEventArgs e)
        {
            if (CodeTextBox.Text.Length == 16) GenerateButton.IsEnabled = true;
            else if (GenerateButton.IsEnabled && CodeTextBox.Text.Length != 16) GenerateButton.IsEnabled = false;
        }
    }
}
