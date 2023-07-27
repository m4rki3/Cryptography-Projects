using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace CipherBlockChaining
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Security.Cryptography.Aes aes;
        public MainWindow()
        {
            InitializeComponent();
            aes = System.Security.Cryptography.Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.GenerateIV();
            aes.GenerateKey();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        }
        ~MainWindow()
        {
            aes.Dispose();
        }
        private void MessageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageTextBox.Text) 
             && !string.IsNullOrWhiteSpace(MessageTextBox.Text))
                EncryptButton.IsEnabled = true;
            else
                EncryptButton.IsEnabled = false;
        }

        private void EncryptButtonClick(object sender, RoutedEventArgs e)
        {
            EncryptedTextBlock.Text = AesCBC.Encrypt(aes, MessageTextBox.Text, aes.IV, aes.Key);
            DecryptButton.IsEnabled = true;
        }

        private void DecryptButtonClick(object sender, RoutedEventArgs e)
        {
            DecryptedTextBlock.Text = AesCBC.Decrypt(aes, EncryptedTextBlock.Text, aes.IV, aes.Key);
        }
    }
}
