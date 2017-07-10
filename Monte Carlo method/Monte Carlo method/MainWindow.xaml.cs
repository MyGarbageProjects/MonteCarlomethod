using Microsoft.Win32;
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

namespace Monte_Carlo_method
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[,] x_y;
        int QuantityOfTest = 0;
        int AverageArea = 0;
        List<long> allArea = new List<long>();
        WriteableBitmap wb;
        ImageSource OriginalImage;
        Color COLOR_FIGURE = Colors.Black;
        Color COLOR_BACKGROUND = Colors.White;
        Color COLOR_DOT = Colors.Yellow;
        public MainWindow()
        {
            InitializeComponent();
        }

        Image mainImage
        {
            get { return MainImage; }
            set { MainImage = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Reset All
            QuantityOfTest = 0;
            AverageArea = 0;
            lblaverage.Content = "Average of the area: 0";
            lblQuantityofTest.Content = "Quantity of Test: 0";
            RESET();
            //
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (ofd.ShowDialog() == true)
                MainImage.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));

            OriginalImage = MainImage.Source;
        }
        private void txtBoxCountPoint_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(Convert.ToChar(e.Text)))
                e.Handled = true;
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            RESET();
            int countPoint = int.Parse(txtBoxCountPoint.Text);
            if (mainImage.Source != null && countPoint > 0 & countPoint <= 1000000)
                MonteCarloMethod(countPoint);
            else if (countPoint == 0)
                SET_LOG("Error: Set image");
        }
        private void MonteCarloMethod(int n)
        {
            long amountDotFigure = 0;
            long figuresSquare = 0;
            long imageSquare = countPixel();
            //int colorYerllo = 0;

            wb = new WriteableBitmap(mainImage.Source as BitmapSource);
            WriteableBitmap _tempWB = new WriteableBitmap(mainImage.Source as BitmapSource);

            byte[] colorData = { COLOR_DOT.B, COLOR_DOT.G, COLOR_DOT.R, COLOR_DOT.A };
            x_y = new int[n, 2];
            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                int x = r.Next(0, Convert.ToInt32(mainImage.ActualWidth));
                int y = r.Next(0, Convert.ToInt32(mainImage.ActualHeight));
                x_y[i, 0] = x;
                x_y[i, 1] = y;
            }


            for (int i = 0; i < x_y.GetLength(0); i++)
            {
                Color c34l = GetPixel(x_y[i, 0], x_y[i, 1], _tempWB);
                if (c34l == COLOR_FIGURE)
                    amountDotFigure++;
                //else if (c34l == Colors.Yellow)
                   // colorYerllo++;
                SetPixel(x_y[i, 0], x_y[i, 1], colorData);

                
            }

            mainImage.Source = wb;
            figuresSquare = imageSquare * amountDotFigure / n;
            allArea.Add(figuresSquare);
            //LOG
            lblAmoutDotFigure.Content = "Number of points in the figure: " + amountDotFigure;
            lblAmoutDotOutsideFigure.Content = "Number of points outside the figure: " + (n - amountDotFigure);
            lblFiguresArea.Content = "Figure's square ≈ " + figuresSquare;
            lblaverage.Content = "Average of the area: " + calculateAverageArea();
            lblQuantityofTest.Content = "Quantity of Test: " + ++QuantityOfTest;


            SET_LOG(String.Format("N: {0}\r\nNumber of points in the figure: {1}\r\nNumber of points outside the figure: {2}\r\nFigure's area ≈ {3}",
                n, amountDotFigure, n - amountDotFigure, figuresSquare));
            //END LOG
        }
        private void SET_LOG(string text)
        {
            txtLOG.Text += text+ Environment.NewLine + new string('-', 50) + Environment.NewLine;
        }
        private Color GetPixel(int x, int y, WriteableBitmap wBitmap = null)
        {
            if (wBitmap is null)
                wBitmap = wb;
            int[] pixelData = new int[1];
            int widthInBytes = 4;
            wBitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, widthInBytes, 0);

            return (Color)ColorConverter.ConvertFromString(demicalToHex(pixelData[0]));
            #region GetAllPixel
            //BitmapSource prgbaSource = new FormatConvertedBitmap(originalBmp, PixelFormats.Pbgra32, null, 0);
            //WriteableBitmap bmp = new WriteableBitmap(prgbaSource);
            //int w = bmp.PixelWidth;
            //int h = bmp.PixelHeight;
            //int[] pixelData = new int[w * h];
            //int widthInBytes = 4 * w;
            //bmp.CopyPixels(pixelData, widthInBytes, 0);
            //for (int i = 0; i < pixelData.Length; ++i)
            //{
            //    pixelData[i] ^= 0x00ffffff;
            //}
            //bmp.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInBytes, 0);
            #endregion
        }
        private string demicalToHex(int color)
        {
            return color.ToString("X").Insert(0, "#");
        }
        private void SetPixels(Color cl, int[,] x_y)
        {
            int amountDotFigure = 0;
            int amountDotOutsideFigure = 0;
            int colorYerllo = 0;

            wb = new WriteableBitmap(mainImage.Source as BitmapSource);
            byte[] colorData = { cl.B, cl.G, cl.R, cl.A };

            for (int i = 0; i < x_y.GetLength(0); i++)
            {
                Color c34l = GetPixel(x_y[i, 0], x_y[i, 1]);
                if (c34l == COLOR_FIGURE)
                    amountDotFigure++;
                else if (c34l == Colors.Yellow)
                    colorYerllo++;
                else
                    amountDotOutsideFigure++;
                wb.WritePixels(new Int32Rect(x_y[i, 0], x_y[i, 1], 1, 1), colorData, 4, 0);
            }

            mainImage.Source = wb;

            MessageBox.Show(String.Format("Amount dot in the figure:{0}\r\nAmount dot outside in the figure:{1}\r\nYellow dot:{2}",
                amountDotFigure, amountDotOutsideFigure, colorYerllo));

        }
        private void SetPixel(int x, int y, byte[] colorData, bool setWB=false)
        {
            wb.WritePixels(new Int32Rect(x, y, 1, 1), colorData, 4, 0);
            if(setWB)
                mainImage.Source = wb;
        }
        private int countPixel()
        {
            return Convert.ToInt32(Math.Round(mainImage.ActualHeight, 0) * Math.Round(mainImage.ActualWidth, 0));
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //Reset All
            QuantityOfTest = 0;
            AverageArea = 0;
            lblaverage.Content = "Average of the area: 0";
            lblQuantityofTest.Content = "Quantity of Test: 0";
            RESET();
            SET_LOG("RESET ALL");
        }
        private void RESET()
        {
            mainImage.Source = OriginalImage;
            lblAmoutDotFigure.Content = "Number of points in the figure: 0";
            lblAmoutDotOutsideFigure.Content = "Number of points outside the figure: 0";
            lblFiguresArea.Content = "Figure's square ≈ 0";
        }
        private long calculateAverageArea()
        {
            long _tempAverage = 0;
            for (int i = 0; i < allArea.Count; i++) _tempAverage += allArea[i];

            return _tempAverage/allArea.Count;
        }
    }
}
