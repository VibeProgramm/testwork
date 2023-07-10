using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Keyer
{
    public partial class MainWindow : Window
    {
        private BitmapImage originalBitmap;
        private WriteableBitmap processedBitmap;
        private Color keyingColor = Colors.Green; 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG files (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                originalBitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                processedBitmap = new WriteableBitmap(originalBitmap);
                displayImage.Source = originalBitmap;
            }
        }

        private void colorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(colorTextBox.Text);
                keyingColor = color;
                if (originalBitmap != null)
                {
                    ProcessImage();
                }
            }
            catch
            {
                
            }
        }


        private void saveImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(processedBitmap));
                    encoder.Save(stream);
                }
            }
        }

        private void ProcessImage()
        {
            int stride = processedBitmap.PixelWidth * 4;
            byte[] pixelData = new byte[stride * processedBitmap.PixelHeight];

            processedBitmap.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Проверяем, совпадает ли цвет пикселя с keyingColor
                if (pixelData[i] == keyingColor.B && pixelData[i + 1] == keyingColor.G && pixelData[i + 2] == keyingColor.R)
                {
                    // Если совпадает, делаем пиксель прозрачным
                    pixelData[i] = 0; // B
                    pixelData[i + 1] = 0; // G
                    pixelData[i + 2] = 0; // R
                    pixelData[i + 3] = 0; // A
                }
            }

            processedBitmap.WritePixels(new Int32Rect(0, 0, processedBitmap.PixelWidth, processedBitmap.PixelHeight), pixelData, stride, 0);

            // Отображаем обработанное изображение
            displayImage.Source = processedBitmap;
        }

    }
}
