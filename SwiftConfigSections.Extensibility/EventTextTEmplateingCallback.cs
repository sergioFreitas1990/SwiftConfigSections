using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.Text;

namespace SwiftConfigSections.Extensibility
{
    /// <summary>
    /// An implementation of the ITextTemplatingCallback interface that uses event
    /// handlers instead of methods.
    /// </summary>
    public class EventTextTEmplateingCallback : ITextTemplatingCallback
    {
        public delegate void ErrorCallbackEvent(bool warning, string message, int line, int column);
        public delegate void SetFileExtensionEvent(string extension);
        public delegate void SetOutputEncodingEvent(Encoding encoding, bool fromOutputDirective);

        public event ErrorCallbackEvent OnErrorCallback;
        public event SetFileExtensionEvent OnSetFileExtension;
        public event SetOutputEncodingEvent OnSetOutputEncoding;

        void ITextTemplatingCallback.ErrorCallback(bool warning, string message, int line, int column)
        {
            OnErrorCallback?.Invoke(warning, message, line, column);
        }

        void ITextTemplatingCallback.SetFileExtension(string extension)
        {
            OnSetFileExtension?.Invoke(extension);
        }

        void ITextTemplatingCallback.SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            OnSetOutputEncoding?.Invoke(encoding, fromOutputDirective);
        }
    }
}
