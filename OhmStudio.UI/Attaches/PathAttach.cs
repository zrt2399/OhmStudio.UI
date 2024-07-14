using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Attaches
{
    internal class PathAttach
    {
        internal static readonly DependencyProperty StartEllipseItemProperty =
            DependencyProperty.RegisterAttached("StartEllipseItem", typeof(EllipseItem), typeof(PathAttach));

        internal static void SetStartEllipseItem(DependencyObject element, EllipseItem value)
        {
            element.SetValue(StartEllipseItemProperty, value);
        }

        internal static EllipseItem GetStartEllipseItem(DependencyObject element)
        {
            return (EllipseItem)element.GetValue(StartEllipseItemProperty);
        }

        internal static readonly DependencyProperty EndEllipseItemProperty =
            DependencyProperty.RegisterAttached("EndEllipseItem", typeof(EllipseItem), typeof(PathAttach));

        internal static void SetEndEllipseItem(DependencyObject element, EllipseItem value)
        {
            element.SetValue(EndEllipseItemProperty, value);
        }

        internal static EllipseItem GetEndEllipseItem(DependencyObject element)
        {
            return (EllipseItem)element.GetValue(EndEllipseItemProperty);
        }

        internal static void UpdateBezierCurve(Path path, Point startPoint, Point endPoint)
        {
            if (path.Data is PathGeometry pathGeometry && pathGeometry.Figures.Count > 0)
            {
                var pathFigure = pathGeometry.Figures.First();

                if (pathFigure.Segments.Count > 0 && pathFigure.Segments[0] is BezierSegment bezierSegment)
                {
                    // 更新贝塞尔曲线的起点、控制点和终点
                    pathFigure.StartPoint = startPoint;
                    bezierSegment.Point1 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y); // 控制点1
                    bezierSegment.Point2 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);   // 控制点2
                    bezierSegment.Point3 = endPoint; // 终点

                    //Debug.WriteLine(startPoint.X + ":" + startPoint.Y);
                    // 如果需要强制刷新UI
                    //path.InvalidateVisual();
                }
            }
        }
    }
}