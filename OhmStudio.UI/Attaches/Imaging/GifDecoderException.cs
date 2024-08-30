using System;
using System.Runtime.Serialization;

namespace OhmStudio.UI.Attaches.Imaging
{
    [Serializable]
    internal class GifDecoderException : Exception
    {
        internal GifDecoderException() { }
        internal GifDecoderException(string message) : base(message) { }
        internal GifDecoderException(string message, Exception inner) : base(message, inner) { }
        protected GifDecoderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}