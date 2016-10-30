namespace DrawingApp
{
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Xml;

    public class DrawingData
    {

        #region SaveDrawing

        public static void SaveDrawing(Canvas canvas)
        {
            XmlDocument doc = CreateXMLdoc(canvas);
            SaveXmlDocument(doc);  
        }

        private static XmlDocument CreateXMLdoc(Canvas canvas)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement drawingElement = doc.CreateElement("Drawing");

            foreach (Polyline line in canvas.Children)
            {   
                drawingElement.AppendChild(CreateLineElement(doc, line));
            }

            doc.AppendChild(drawingElement);
            return doc;
        }

        private static XmlElement CreateLineElement(XmlDocument doc, Polyline line)
        {
            XmlElement lineElement = doc.CreateElement("Line");
            lineElement.SetAttribute("Stroke", line.Stroke.ToString());
            lineElement.SetAttribute("StrokeThickness", line.StrokeThickness.ToString());

            foreach (Point point in line.Points)
            {
                lineElement.AppendChild(CreatePointElement(doc, point));
            }
            return lineElement;
        }

        private static XmlElement CreatePointElement(XmlDocument doc, Point point)
        {
            XmlElement pointElement = doc.CreateElement("Point");
            pointElement.SetAttribute("X", point.X.ToString());
            pointElement.SetAttribute("Y", point.Y.ToString());
            return pointElement;
        }

        private static void SaveXmlDocument(XmlDocument doc)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "XML File|*.xml";
            saveFile.Title = "Save Drawing";
            saveFile.ShowDialog();

            if (saveFile.FileName != "")
                doc.Save(saveFile.FileName);

        }

        # endregion

        #region PopulateCanvasFromSavedDrawing

        public static void PopulateCanvasFromSavedDrawing(Canvas canvas)
        {
              try
              {
                  LoadDrawing(GetXmlFileName(), canvas);
              }
              catch (XmlException e)
              {
                  ErrorLog.WriteToLog(e.LineNumber.ToString(), e.Message);
                  string filename = "XmlFiles/ErrorMessage.xml";
                  LoadDrawing(filename, canvas);
              }
        }

        private static string GetXmlFileName()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "XML Document |*.xml";
            openFile.ShowDialog();

            return openFile.FileName;
        }

        private static void LoadDrawing(string filename, Canvas canvas)
        {
            if (filename != "")
            {
                XmlElement XmlDoc = LoadXmlDocument(filename);
                foreach (Polyline line in LoadPolylinesFromXml(XmlDoc))
                {
                    canvas.Children.Add(line);
                }
            }
        }

        private static XmlElement LoadXmlDocument(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc.DocumentElement;
        }

        private static List<Polyline> LoadPolylinesFromXml(XmlElement XmlDoc)
        {
            List<Polyline> lineList = new List<Polyline>();
            
            foreach (XmlElement line in XmlDoc.GetElementsByTagName("Line"))
            {
                lineList.Add(CreatePolyLineFromXml(line));
            }
            return lineList;
        }

        private static Polyline CreatePolyLineFromXml(XmlElement XmlPolyline)
        {
            Polyline polyline = new Polyline();
            polyline.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(XmlPolyline.GetAttribute("Stroke"));
            polyline.StrokeThickness = double.Parse(XmlPolyline.GetAttribute("StrokeThickness").ToString());

            foreach (XmlElement pointElement in XmlPolyline.GetElementsByTagName("Point"))
            {
                polyline.Points.Add(GetPoint(pointElement));
            }
            return polyline;
        }

        private static Point GetPoint(XmlElement pointElement)
        {
            Point point = new Point();
            point.X = double.Parse(pointElement.GetAttribute("X").ToString());
            point.Y = double.Parse(pointElement.GetAttribute("Y").ToString());
            return point;
        }

        #endregion

    }
}
