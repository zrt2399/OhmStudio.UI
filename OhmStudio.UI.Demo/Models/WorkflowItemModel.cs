using System.Diagnostics;
using System.Windows.Input;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Demo.ViewModels;
using OhmStudio.UI.Mvvm;

namespace OhmStudio.UI.Demo.Models
{
    public class WorkflowItemModel : ObservableObject
    {
        public WorkflowItemModel()
        {
            DeleteCommand = new RelayCommand(Delete);
        }

        public bool IsSelected { get; set; }

        public void OnIsSelectedChanged()
        {
            Debug.WriteLine(Name);
        }

        public string Name { get; set; }

        public StepType StepType { get; set; } = StepType.Nomal;

        public double Width { get; set; } = 200;
        public double Height { get; set; } = 200;

        public double Left { get; set; }
        public double Top { get; set; }

        public bool IsLock { get; set; }  

        public string LineContent { get; set; } = "下一节点";

        public WorkflowItemModel PreviousStep { get; set; }
        public WorkflowItemModel NextStep { get; set; }
        public WorkflowItemModel FromStep { get; set; }
        public WorkflowItemModel JumpToStep { get; set; }

        public ICommand DeleteCommand { get; }

        public void Delete()
        {
            ViewModelLocator.MainViewModel.WorkflowItemModels.Remove(this);
        }
    }
}