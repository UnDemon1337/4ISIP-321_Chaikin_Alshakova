using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using QRCoder;
using System.IO;

namespace TechnoServise.Pages
{
    /// <summary>
    /// Логика взаимодействия для QRViewPage.xaml
    /// </summary>
    public partial class QRViewPage : Page
    {
        public QRViewPage()
        {
            InitializeComponent();

            GenerateQR();
        }

        private void GenerateQR()
        {
            string inputText = "https://forms.gle/EvKo5TpFNVowjHGa9";

            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("Введите текст для генерации QR-кода", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Генерация QR-кода
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (var qrCodeBitmap = qrCode.GetGraphic(10))
                using (var memory = new MemoryStream())
                {
                    qrCodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    // Отображение QR-кода
                    QRCodeImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации QR-кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
