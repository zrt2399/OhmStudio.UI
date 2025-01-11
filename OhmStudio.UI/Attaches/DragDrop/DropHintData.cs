using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// Data presented in drop hint adorner.
    /// </summary>
    public class DropHintData : INotifyPropertyChanged
    {
        private DropHintState hintState;
        private string hintText;

        public DropHintData(DropHintState hintState, string hintText)
        {
            HintState = hintState;
            HintText = hintText;
        }

        /// <summary>
        /// The hint text to display to the user. See <see cref="IDropInfo.DropHintText"/>
        /// and <see cref="IDropHintInfo.DropHintText"/>.
        /// </summary>
        public string HintText
        {
            get => hintText;
            set
            {
                if (value == hintText) return;
                hintText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The hint state to display different colors for hints. See <see cref="IDropInfo.DropTargetHintState"/>
        /// and <see cref="IDropHintInfo.DropTargetHintState"/>.
        /// </summary>
        public DropHintState HintState
        {
            get => hintState;
            set
            {
                if (value == hintState) return;
                hintState = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
 
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}