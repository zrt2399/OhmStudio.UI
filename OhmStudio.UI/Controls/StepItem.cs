using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    internal class EllipseItem
    {
        public EllipseItem(Ellipse ellipse)
        {
            Ellipse = ellipse;
        }

        internal Ellipse Ellipse { get; set; }
        internal Point GetEllipsePoint(UIElement parent)
        {
            return Ellipse.TranslatePoint(new Point(Ellipse.ActualWidth / 2, Ellipse.ActualHeight / 2), parent);
        }
    }

    public enum FlowType
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

        public static readonly DependencyProperty FlowTypeProperty =
            DependencyProperty.Register(nameof(FlowType), typeof(FlowType), typeof(StepItem));

        public FlowType FlowType
        {
            get => (FlowType)GetValue(FlowTypeProperty);
            set => SetValue(FlowTypeProperty, value);
        }

        internal Point MouseDownControlPoint { get; set; }

        private Ellipse EllipseLeft;
        private Ellipse EllipseTop;
        private Ellipse EllipseRight;
        private Ellipse EllipseBottom;

        public DragCanvas DragCanvas { get; private set; }

        internal EllipseItem[] EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DragCanvas = this.FindParentObject<DragCanvas>();

            EllipseLeft = GetTemplateChild("EllipseLeft") as Ellipse;
            EllipseTop = GetTemplateChild("EllipseTop") as Ellipse;
            EllipseRight = GetTemplateChild("EllipseRight") as Ellipse;
            EllipseBottom = GetTemplateChild("EllipseBottom") as Ellipse;
            EllipseItems = new EllipseItem[] { new EllipseItem(EllipseLeft), new EllipseItem(EllipseTop), new EllipseItem(EllipseRight), new EllipseItem(EllipseBottom) };
            IsInit = true;
        }

        internal EllipseItem IsPoint(Point point)
        {
            if (VisualTreeHelper.HitTest(DragCanvas, point)?.VisualHit is Ellipse ellipse)
            {
                return EllipseItems.FirstOrDefault(x => x.Ellipse == ellipse);
            }
            else
            {
                return null;
            }
        }
    }
}