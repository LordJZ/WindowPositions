using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LordJZ.Collections;
using LordJZ.WinAPI;

namespace WindowPositions
{
    class WindowPositionRepository
    {
        static readonly XmlSerializer m_ser = new XmlSerializer(typeof(WindowPosition[]));

        static readonly string m_filename =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "WindowPositionRepository.xml");

        static WindowPosition[] m_positions;

        public static WindowPosition[] Positions
        {
            get
            {
                if (m_positions == null)
                    LoadPositions();

                return m_positions;
            }
            set
            {
                m_positions = value;
                SavePositions();
            }
        }

        static void LoadPositions()
        {
            try
            {
                using (StreamReader tr = new StreamReader(m_filename))
                    m_positions = (WindowPosition[])m_ser.Deserialize(tr) ?? EmptyArray<WindowPosition>.Instance;
            }
            catch
            {
                m_positions = EmptyArray<WindowPosition>.Instance;
            }
        }

        static void SavePositions()
        {
            using (var wr = new StreamWriter(m_filename))
                m_ser.Serialize(wr, m_positions);
        }

        public static void RestoreAllPositions()
        {
            IEnumerable<WindowDTO> windows = NativeWindow.Enumerate().Select(WindowDTO.TryCreate).Where(dto => dto != null).ToArray();

            var pairs = from position in Positions
                        from window in windows
                        where position.Matches(window)
                        select new { Position = position, Window = window };

            foreach (var pair in pairs)
            {
                NativeWindow window = pair.Window.NativeWindow;
                window.Placement = pair.Position.Placement;
            }
        }
    }
}
