﻿using System;
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

namespace DigitalSignature
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
        private async void GetSignatureButtonClick(object sender, RoutedEventArgs e)
        {
            Task<string> task = new(() =>
                   DiscreteLogarithmDigitalSignature.GetDigitalSignature(MessageTextBox.Text)
           );
            task.Start(TaskScheduler.FromCurrentSynchronizationContext());
            await task;
            SignatureTextBlock.Text = task.Result;
        }
        private void MessageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageTextBox.Text)
             && !string.IsNullOrWhiteSpace(MessageTextBox.Text))
                GetSignatureButton.IsEnabled = true;
            else
                GetSignatureButton.IsEnabled = false;
        }
    }
}
