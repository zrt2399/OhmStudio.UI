using System.Windows;

namespace OhmStudio.UI.Attaches.DragDrop
{
    internal class DragDropEffectPreview : DragDropPreview
    {
        public DragDropEffectPreview(UIElement rootElement, UIElement previewElement, Point translation, DragDropEffects effects, string effectText, string destinationText)
            : base(rootElement, previewElement, translation, default)
        {
            Effects = effects;
            EffectText = effectText;
            DestinationText = destinationText;
        }

        public DragDropEffects Effects { get; set; }

        public string EffectText { get; set; }

        public string DestinationText { get; set; }
    }
}