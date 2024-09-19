using System.Diagnostics;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Mvvm;

namespace OhmStudio.UI.Demo.Models
{
    public class WorkflowItemModel : ObservableObject
    {
        public bool IsSelected { get; set; }

        public void OnIsSelectedChanged()
        {
            Debug.WriteLine(Name);
        }

        public string Name { get; set; }

        public StepType StepType { get; set; } = StepType.Begin;

        public double Width { get; set; } = 200;
        public double Height { get; set; } = 200;

        public double Left { get; set; }
        public double Top { get; set; }

        public bool IsDraggable { get; set; } = true;

        public string PathContent { get; set; } = "下一节点";

        public WorkflowItemModel LastStep { get; set; }
        public WorkflowItemModel NextStep { get; set; }
        public WorkflowItemModel FromStep { get; set; }
        public WorkflowItemModel JumpStep { get; set; }
    }
}