using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LordJZ.WinAPI;

namespace WindowPositions
{
    public class WindowPosition
    {
        public string Title { get; set; }

        public string ClassName { get; set; }

        public WindowPlacement Placement { get; set; }

        public bool Matches(WindowDTO dto)
        {
            if (this.Title != null)
            {
                if (dto.Title != this.Title)
                    return false;
            }

            if (dto.ClassName != this.ClassName)
                return false;

            return true;
        }
    }
}
