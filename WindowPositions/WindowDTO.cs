using LordJZ.WinAPI;

namespace WindowPositions
{
    public class WindowDTO
    {
        public WindowDTO(NativeWindow window)
        {
            NativeWindow = window;

            this.ClassName = window.ClassName;
            this.Title = window.Text;
        }

        internal readonly NativeWindow NativeWindow;

        public string ClassName { get; set; }

        public string Title { get; set; }
    }
}
