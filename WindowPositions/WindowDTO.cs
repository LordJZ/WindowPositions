using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LordJZ.Linq;
using LordJZ.WinAPI;

namespace WindowPositions
{
    public class WindowDTO
    {
        //HwndWrapper[WindowPositions.exe;;837cb3a7-028d-43a7-bfee-33e6db4b1105]|MainWindow
        static readonly Regex s_hwndWrapperRegex = new Regex(@"HwndWrapper\[(.+);[a-z0-9\-]{36}\]");

        static WindowDTO TryCreate(NativeWindow window)
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

	    static readonly Tuple<string, string>[] s_exceptions =
	    {
            Tuple.Create("CiceroUIWndFrame", "CiceroUIWndFrame"),
            Tuple.Create("GDI+ Hook Window Class", "GDI+ Window"),
		    Tuple.Create("IME", "Default IME"),
		    Tuple.Create("MSCTFIME UI", "MSCTFIME UI"),
		    Tuple.Create("OleDdeWndClass", "DDE Server Window")
	    };

	    static bool CheckExceptions(WindowDTO dto)
	    {
	        if (dto.Title == "SystemResourceNotifyWindow" ||
	            dto.Title == "MediaContextNotificationWindow" ||
                dto.ClassName.StartsWith(".NET-BroadcastEventWindow"))
	        {
                // skip WPF stuff
                return false;
	        }

		    foreach (Tuple<string, string> exception in s_exceptions)
		    {
                if (dto.ClassName == exception.Item1 && dto.Title == exception.Item2)
                    return false;
		    }

            return true;
	    }

	    public static IEnumerable<WindowDTO> Enumerate()
	    {
		    return NativeWindow.Enumerate()
		                       .Select(TryCreate)
		                       .Where(dto => dto != null && !string.IsNullOrEmpty(dto.Title))
                               .Where(CheckExceptions);
	    }

        public WindowDTO(NativeWindow window)
        {
            NativeWindow = window;

            this.PID = window.ProcessId;

            try
            {
                const ProcessAccessRights access =
                    ProcessAccessRights.QueryInformation | ProcessAccessRights.VirtualMemoryRead;

                Process process = Process.Open(this.PID, access);
                this.ProcessName = System.IO.Path.GetFileName(process.ImagePath);
            }
            catch
            {
            }

            this.Title = window.Text;
            this.ClassName = UnwrapClassName(window.ClassName);

            if (!string.IsNullOrEmpty(this.ProcessName))
                this.ClassName = this.ClassName.Replace("DefaultDomain;", this.ProcessName);
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

        public string ProcessName { get; set; }

        public int PID { get; set; }
    }
}
