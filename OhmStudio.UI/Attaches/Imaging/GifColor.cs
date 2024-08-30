namespace OhmStudio.UI.Attaches.Imaging
{
    internal struct GifColor
    {
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;

        internal GifColor(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public byte R => _r;
        public byte G => _g;
        public byte B => _b;

        public override string ToString()
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", _r, _g, _b);
        }
    }
}