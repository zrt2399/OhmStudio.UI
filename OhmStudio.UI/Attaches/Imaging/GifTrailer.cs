namespace OhmStudio.UI.Attaches.Imaging
{
    internal class GifTrailer : GifBlock
    {
        private GifTrailer()
        {
        }

        internal const int TrailerByte = 0x3B;

        internal override GifBlockKind Kind => GifBlockKind.Other;

        internal static GifTrailer ReadTrailer()
        {
            return new GifTrailer();
        }
    }
}