using System.Text.RegularExpressions;
using LordJZ.WinAPI;

namespace WindowPositions
{
    public class WindowDTO
    {
        //HwndWrapper[WindowPositions.exe;;837cb3a7-028d-43a7-bfee-33e6db4b1105]|MainWindow
        static readonly Regex s_hwndWrapperRegex = new Regex(@"HwndWrapper\[(.+);[a-z0-9\-]{36}\]");

        public static WindowDTO TryCreate(NativeWindow window)
        {
            try
            {
                return new WindowDTO(window);
            }
            catch
            {
                return null;
            }
        }

        public WindowDTO(NativeWindow window)
        {
            NativeWindow = window;

            this.ClassName = UnwrapClassName(window.ClassName);
            this.Title = window.Text;
        }

        static string UnwrapClassName(string className)
        {
            Match match = s_hwndWrapperRegex.Match(className);
            if (match.Success)
                return match.Groups[1].Value;

            return className;
        }

        internal readonly NativeWindow NativeWindow;

        public bool Saved { get; set; }

        public string ClassName { get; set; }

        public string Title { get; set; }
    }
}
