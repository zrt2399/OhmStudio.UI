﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    internal class PathItem : Control
    {
        internal static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(PathItem), new PropertyMetadata((sender, e) =>
            {
                if (sender is PathItem pathItem && e.NewValue is Point point)
                {
                    Canvas.SetLeft(pathItem, point.X);
                    Canvas.SetTop(pathItem, point.Y);
                }
            }));

        internal static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point0Property =
            DependencyProperty.Register(nameof(Point0), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(nameof(Point1), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(nameof(Point2), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point3Property =
            DependencyProperty.Register(nameof(Point3), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(PathItem), new PropertyMetadata("下一节点"));

        internal static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(PathItem));

        internal Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        internal Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        internal Point Point0
        {
            get => (Point)GetValue(Point0Property);
            set => SetValue(Point0Property, value);
        }

        internal Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        internal Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        internal Point Point3
        {
            get => (Point)GetValue(Point3Property);
            set => SetValue(Point3Property, value);
        }

        internal string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the polygon.
        /// </summary>
        internal PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        internal EllipseItem StartEllipseItem { get; set; }

        internal EllipseItem EndEllipseItem { get; set; }

        internal DragCanvas DragCanvas { get; set; }

        internal void UpdateBezierCurve(Point startPoint, Point endPoint)
        {
            Point startPointTemp = new Point();
            Point endPointTemp = new Point();
            Point endEllipsePoint = EndEllipseItem == null ? endPoint : EndEllipseItem.GetPoint(DragCanvas);
 
            startPointTemp.X = Math.Min(startPoint.X, endEllipsePoint.X); 
            startPointTemp.Y = Math.Min(startPoint.Y, endEllipsePoint.Y);
            endPointTemp.X = Math.Min(startPoint.X, endPoint.X);
            endPointTemp.Y = Math.Min(startPoint.Y, endPoint.Y);
              
            StartPoint = startPointTemp;
            EndPoint = endPointTemp;

            startPoint.X -= StartPoint.X;
            startPoint.Y -= StartPoint.Y;
            endPoint.X -= StartPoint.X;
            endPoint.Y -= StartPoint.Y;

            Point0 = startPoint;
            Point1 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);
            Point2 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
            Point3 = endPoint;
            UpdateArrow(Point2, endPoint);
        }

        private void UpdateArrow(Point point2, Point endPoint)
        {
            double arrowLength = 10;
            double arrowWidth = 10;

            // 计算箭头的方向
            Vector direction = endPoint - point2;
            direction.Normalize();

            // 算出箭头的两个角点
            Point arrowPoint1 = endPoint - direction * arrowLength + new Vector(-direction.Y, direction.X) * arrowWidth / 2;
            Point arrowPoint2 = endPoint - direction * arrowLength - new Vector(-direction.Y, direction.X) * arrowWidth / 2;

            // 更新箭头形状的点
            Points = new PointCollection(new[] { endPoint, arrowPoint1, arrowPoint2 });
        }

        internal void Delete()
        {
            StartEllipseItem.RemoveStep();
            StartEllipseItem.PathItem = null;
            StartEllipseItem = null;
            EndEllipseItem.PathItem = null;
            EndEllipseItem = null;
            ContextMenu = null;
            DragCanvas.Children.Remove(this);
        }
    }

    internal class EllipseItem : Control
    {
        internal static readonly DependencyProperty EllipseOrientationProperty =
            DependencyProperty.Register(nameof(EllipseOrientation), typeof(EllipseOrientation), typeof(EllipseItem));

        internal EllipseOrientation EllipseOrientation
        {
            get => (EllipseOrientation)GetValue(EllipseOrientationProperty);
            set => SetValue(EllipseOrientationProperty, value);
        }

        internal StepItem StepParent { get; set; }

        internal PathItem PathItem { get; set; }

        internal Point GetPoint(UIElement parent)
        {
            return TranslatePoint(new Point(ActualWidth / 2, ActualHeight / 2), parent);
        }

        internal void RemoveStep()
        {
            if (EllipseOrientation == EllipseOrientation.Right)
            {
                StepParent.JumpStep.FromStep = null;
                StepParent.JumpStep = null;
            }
            if (EllipseOrientation == EllipseOrientation.Bottom)
            {
                StepParent.NextStep.LastStep = null;
                StepParent.NextStep = null;
            }
        }
    }

    public enum EllipseOrientation
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public enum StepType
    {
        Begin,
        Nomal,
        Condition,
        End
    }

    public class StepItem : ContentControl
    {
        public StepItem()
        {

        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(StepItem));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty LastStepProperty =
            DependencyProperty.Register(nameof(LastStep), typeof(StepItem), typeof(StepItem));

        public StepItem LastStep
        {
            get => (StepItem)GetValue(LastStepProperty);
            set => SetValue(LastStepProperty, value);
        }

        public static readonly DependencyProperty FromStepProperty =
            DependencyProperty.Register(nameof(FromStep), typeof(StepItem), typeof(StepItem));

        public StepItem FromStep
        {
            get => (StepItem)GetValue(FromStepProperty);
            set => SetValue(FromStepProperty, value);
        }

        public static readonly DependencyProperty JumpStepProperty =
            DependencyProperty.Register(nameof(JumpStep), typeof(StepItem), typeof(StepItem));

        public StepItem JumpStep
        {
            get => (StepItem)GetValue(JumpStepProperty);
            set => SetValue(JumpStepProperty, value);
        }

        public static readonly DependencyProperty NextStepProperty =
            DependencyProperty.Register(nameof(NextStep), typeof(StepItem), typeof(StepItem));

        public StepItem NextStep
        {
            get => (StepItem)GetValue(NextStepProperty);
            set => SetValue(NextStepProperty, value);
        }

        public static readonly DependencyProperty StepTypeProperty =
            DependencyProperty.Register(nameof(StepType), typeof(StepType), typeof(StepItem), new PropertyMetadata(StepType.Nomal));

        public StepType StepType
        {
            get => (StepType)GetValue(StepTypeProperty);
            set => SetValue(StepTypeProperty, value);
        }

        internal Point MouseDownControlPoint { get; set; }

        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;

        public DragCanvas DragCanvas { get; internal set; }

        internal Dictionary<EllipseOrientation, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //DragCanvas = this.FindParentObject<DragCanvas>();

            EllipseLeft = GetTemplateChild("EllipseLeft") as EllipseItem;
            EllipseTop = GetTemplateChild("EllipseTop") as EllipseItem;
            EllipseRight = GetTemplateChild("EllipseRight") as EllipseItem;
            EllipseBottom = GetTemplateChild("EllipseBottom") as EllipseItem;
            EllipseItems = new Dictionary<EllipseOrientation, EllipseItem>();
            EllipseItems.Add(EllipseOrientation.Left, EllipseLeft);
            EllipseItems.Add(EllipseOrientation.Top, EllipseTop);
            EllipseItems.Add(EllipseOrientation.Right, EllipseRight);
            EllipseItems.Add(EllipseOrientation.Bottom, EllipseBottom);
            foreach (var item in EllipseItems.Values)
            {
                item.StepParent = this;
            }
            IsInit = true;
        }

        internal EllipseItem GetEllipseItem(Point point)
        {
            var hitResult = VisualTreeHelper.HitTest(DragCanvas, point)?.VisualHit as FrameworkElement;
            if (hitResult?.TemplatedParent is EllipseItem ellipseItem)
            {
                return EllipseItems.Values.FirstOrDefault(x => x == ellipseItem);
            }
            else
            {
                return null;
            }
        }

        internal bool SetStep(StepItem fromStep, EllipseItem fromEllipse, EllipseItem toEllipse)
        {
            bool flag = false;
            if (fromEllipse == toEllipse)
            {
                return false;
            }
            if (this == fromStep)
            {
                UIMessageTip.ShowWarning("无法设置节点为自己");
            }
            else if (fromEllipse.PathItem != null || toEllipse.PathItem != null)
            {
                UIMessageTip.ShowWarning("该节点已经存在连接关系，无法创建连接曲线，请删除后再试");
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Left || toEllipse.EllipseOrientation == EllipseOrientation.Right)
            {
                UIMessageTip.ShowWarning("被跳转节点无法直接设置");
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Top || toEllipse.EllipseOrientation == EllipseOrientation.Bottom)
            {
                UIMessageTip.ShowWarning("上一步下一步节点设置错误");
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Right && toEllipse.EllipseOrientation == EllipseOrientation.Top)
            {
                UIMessageTip.ShowWarning("跳转节点只能设置为下一节点的被跳转节点");
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Bottom && toEllipse.EllipseOrientation == EllipseOrientation.Left)
            {
                UIMessageTip.ShowWarning("下一步节点只能设置为下一节点的上一步节点");
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Right)
            {
                fromStep.JumpStep = this;
                FromStep = fromStep;
                flag = true;
            }
            else if (fromEllipse.EllipseOrientation == EllipseOrientation.Bottom)
            {
                fromStep.NextStep = this;
                LastStep = fromStep;
                flag = true;
            }

            return flag;
        }

        internal void Delete()
        {
            foreach (var item in EllipseItems.Values)
            {
                item.PathItem.Delete();
            }
            DragCanvas.Children.Remove(this);
        }

        public void UpdateCurve()
        {
            if (LastStep != null)
            {
                UpdateCurve(EllipseItems[EllipseOrientation.Top].PathItem);
            }
            if (NextStep != null)
            {
                UpdateCurve(EllipseItems[EllipseOrientation.Bottom].PathItem);
            }
            if (FromStep != null)
            {
                UpdateCurve(EllipseItems[EllipseOrientation.Left].PathItem);
            }
            if (JumpStep != null)
            {
                UpdateCurve(EllipseItems[EllipseOrientation.Right].PathItem);
            }
        }

        internal void UpdateCurve(PathItem pathItem)
        {
            pathItem.UpdateBezierCurve(pathItem.StartEllipseItem.GetPoint(DragCanvas), pathItem.EndEllipseItem.GetPoint(DragCanvas));
        }
    }
}