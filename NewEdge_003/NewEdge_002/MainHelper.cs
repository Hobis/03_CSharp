using System;
using System.Windows.Forms;

namespace NewEdge_002
{
    // #
    public enum Win_Message_Types
    {
        Win_Init,
        Win_Set_Title,
        Win_Set_Visible,
        Win_Set_MinSize,
        Win_Set_Location,
        Win_Resize_Max,
        Win_Resize_Min,
        Win_Resize_Normal,
        Win_Resize_FullScreen,
        Win_Resize,
        Win_Open,
        Win_Center_Location
    }

    // #
    public static class Debug
    {
        private const string _frontMsg = "# [hb] ";
        public static void Log(string msg)
        {
            MessageBox.Show(_frontMsg + msg);
        }
    }
}