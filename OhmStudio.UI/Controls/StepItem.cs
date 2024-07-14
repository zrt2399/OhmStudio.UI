using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    internal class EllipseItem : ContentControl
    {
        internal EllipseOrientation EllipseOrientation { get; set; }

        internal StepItem ParentStep { get; set; }

        internal Path Path { get; set; }

        internal Point GetEllipsePoint(UIElement parent)
        {
            return TranslatePoint(new Point(ActualWidth / 2, ActualHeight / 2), parent);
        }

        internal void RemoveStep()
        {
            if (EllipseOrientation == EllipseOrientation.Right)
            {
                ParentStep.JumpStep.FromStep = null;
                ParentStep.JumpStep = null;
            }
            if (EllipseOrientation == EllipseOrientation.Bottom)
            {
                ParentStep.NextStep.LastStep = null;
                ParentStep.NextStep = null;
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
            DependencyProperty.Register(nameof(StepType), typeof(StepType), typeof(StepItem));

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

        public DragCanvas DragCanvas { get; private set; }

        internal Dictionary<EllipseOrientation, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DragCanvas = this.FindParentObject<DragCanvas>();

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
                item.ParentStep = this;
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

        internal void UpdatePath()
        {
            if (LastStep != null)
            {
                UpdateBezierCurve(EllipseItems[EllipseOrientation.Top].Path);
            }
            if (NextStep != null)
            {
                UpdateBezierCurve(EllipseItems[EllipseOrientation.Bottom].Path);
            }
            if (FromStep != null)
            {
                UpdateBezierCurve(EllipseItems[EllipseOrientation.Left].Path);
            }
            if (JumpStep != null)
            {
                UpdateBezierCurve(EllipseItems[EllipseOrientation.Right].Path);
            }
        }

        internal void UpdateBezierCurve(Path path)
        {
            PathAttach.UpdateBezierCurve(path, PathAttach.GetStartEllipseItem(path).GetEllipsePoint(DragCanvas), PathAttach.GetEndEllipseItem(path).GetEllipsePoint(DragCanvas));
        }
    }
}