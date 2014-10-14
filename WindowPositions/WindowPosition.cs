﻿using LordJZ.WinAPI;

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
                if (this.Title != dto.Title)
                    return false;
            }

            if (dto.ClassName != this.ClassName)
                return false;

            return true;
        }
    }
}
