namespace DrawingApp
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDrawing;
        private Brush _stroke = Brushes.Black;
        private double _strokeThickness = 5;

        private Polyline _line;

        private int _grownUpAnswer;
        private int _first;
        private int _second;

        private bool _allowOpenSave;

        public MainWindow()
        {
            InitializeComponent();
            _blackBrush.StrokeThickness = 3;
            _stroke1.Stroke = Brushes.Blue;
            RemoveRestrictions(false);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopups();
            _isDrawing = true;
            BuildLine(e.GetPosition(_mainCanvas));
            _mainCanvas.Children.Add(_line);
            _mainCanvas.CaptureMouse();
        }

        private void BuildLine(Point startPoint)
        {
            _line = new Polyline();
            _line.Stroke = _stroke;
            _line.StrokeThickness = _strokeThickness;
            _line.Points.Add(startPoint);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                _line.Points.Add(e.GetPosition(_mainCanvas));
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            _line = null;
            _mainCanvas.ReleaseMouseCapture();   
        }

        private void OnToolSelect(object sender, MouseButtonEventArgs e)
        {
            string[] typeArray = e.Source.GetType().ToString().Split('.');

            string type = typeArray[typeArray.Length - 1];
            ClosePopups();

            switch(type)
            {
                case "Rectangle":
                    ResetBrushes();
                    ChangeBrushStroke(e.Source);
                    break;

                case "Line":
                    ChangeBrushStrokeThickness(e.Source);
                    break;

                case "Image":
                    ActivateDrawingControls(e.Source as FrameworkElement);
                    break;  
            }
        }

        private void ChangeBrushStroke(object element)
        {
            Rectangle rect = element as Rectangle;
            rect.StrokeThickness = 3;
            _stroke = rect.Fill;
        }

        private void ChangeBrushStrokeThickness(object element)
        {
            Line line = element as Line;
            if (_strokeThickness != line.StrokeThickness / 2)
            {
                ResetStrokes();
                line.Stroke = Brushes.Blue;
                _strokeThickness = line.StrokeThickness / 2;
            }
        }

        private void TurnOnEraser()
        {
            Eraser.Height = 25;
            Eraser.Width = 25;
            _stroke = _mainCanvas.Background;
        }

        private void ActivateDrawingControls(FrameworkElement element)
        {
            if (element != null)
            {
                switch(element.Name)
                {
                    case "Save":
                        if (_allowOpenSave)
                        {
                            DrawingData.SaveDrawing(_mainCanvas);
                        }
                        break;

                    case "Open":
                        if (_allowOpenSave)
                        {
                            _mainCanvas.Children.Clear();
                            DrawingData.PopulateCanvasFromSavedDrawing(_mainCanvas);
                        }
                        break; 

                    case "Unlock":
                        if (!GrownUpTest.IsOpen)
                        {
                            GenerateMathProblem();
                            GrownUpTest.IsOpen = true;
                        }
                        break;

                    case "Eraser":
                        TurnOnEraser();
                        break;

                    case "Clear":
                        _mainCanvas.Children.Clear();
                        break;
                }
            }
        }

        private void OnSubmitAnswer(object sender, RoutedEventArgs e)
        {
            GrownUpTest.IsOpen = false;

            try
            {
                if (int.Parse(Answer.Text) == (_first * _second))
                {
                    GrownUpControl.IsOpen = true;
                }
                _grownUpAnswer = 0;
            }
            catch (ArgumentNullException nullExp)
            {
                Console.WriteLine(nullExp.Message);
                ErrorLog.WriteToLog("161", nullExp.Message);
                ClosePopups();
                _grownUpAnswer = 0;
            }
            catch (FormatException exp)
            {
                Console.WriteLine(exp.Message);
                ErrorLog.WriteToLog("161", exp.Message);
                ClosePopups();
                _grownUpAnswer = 0;
            }
        }

        private void OnChangePermission(object sender, RoutedEventArgs e)
        {
            ClosePopups();

            if (_allowOpenSave)
            {
                RemoveRestrictions(false);
            }
            else
            {
                RemoveRestrictions(true);
            }
        }

        private void OnCloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnShowCredits(object sender, RoutedEventArgs e)
        {
            Credits.IsOpen = true;
        }

        private void RemoveRestrictions(bool allow)
        {
            // Permissions allow the user to open and save files
            // If the user is allowed to open and save files
            if (allow)
            {
                _allowOpenSave = true;
                Open.Visibility = System.Windows.Visibility.Visible;
                Save.Visibility = System.Windows.Visibility.Visible;
                AllowSaveBtn.Content = "Disallow Save";
            }
            else
            {
                _allowOpenSave = false;
                Open.Visibility = System.Windows.Visibility.Hidden;
                Save.Visibility = System.Windows.Visibility.Hidden;
                AllowSaveBtn.Content = "Allow Save";
            }
        }

        private void ResetBrushes()
        {
            _blackBrush.StrokeThickness = 1;
            _redBrush.StrokeThickness = 1;
            _greenBrush.StrokeThickness = 1;
            _yellowBrush.StrokeThickness = 1;
            _blueBrush.StrokeThickness = 1;
            Eraser.Height = 30;
            Eraser.Width = 30;
        }

        private void ResetStrokes()
        {
            _stroke1.Stroke = Brushes.Black;
            _stroke2.Stroke = Brushes.Black;
            _stroke3.Stroke = Brushes.Black;
            _stroke4.Stroke = Brushes.Black;
        }

        private void GenerateMathProblem()
        {
            Random rnd = new Random();
            _first = rnd.Next(1, 5);
            _second = rnd.Next(2, 7);
            _grownUpAnswer = _first * _second;

            First.Content = _first;
            Second.Content = _second;
            Answer.Text = "";
        }

        private void ClosePopups()
        {
            GrownUpControl.IsOpen = false;
            GrownUpTest.IsOpen = false;
            Credits.IsOpen = false;
        }
    }
}
