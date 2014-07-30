using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Text;
using System.Threading;
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
        Win_Center_Location,
        Win_Copy_Folder,
        Win_Close,
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

    // #
    public static class FIO_Util
    {
        public static void DirectoryCopy(string sourcePath, string destPath, bool isSubPaths, string shortcutName)
        {
            _t = new Thread(new ThreadStart(delegate()
            {
                p_DirectoryCopy_Core(sourcePath, destPath, isSubPaths, shortcutName);
            }));
            _t.Start();
        }

        private static Thread _t = null;

        private static void p_DirectoryCopy_Core(string sourcePath, string destPath, bool isSubPaths, string shortcutName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo t_dir = new DirectoryInfo(sourcePath);
            DirectoryInfo[] t_dirs = t_dir.GetDirectories();

            if (!t_dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourcePath);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            
            //destPath = Path.Combine(destPath, shortcutName);
            //Debug.Log("destPath: " + destPath);
            //return;
            //Directory.CreateDirectory(destPath);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] t_files = t_dir.GetFiles();
            foreach (FileInfo t_file in t_files)
            {
                string t_path = Path.Combine(destPath, t_file.Name);
                t_file.CopyTo(t_path, false);
            }
            
            // If copying subdirectories, copy them and their contents to new location. 
            if (isSubPaths)
            {
                foreach (DirectoryInfo t_subdir in t_dirs)
                {
                    string t_path = Path.Combine(destPath, t_subdir.Name);
                    DirectoryCopy(t_subdir.FullName, t_path, isSubPaths, null);
                }
            }

            if (shortcutName != null)
            {
                string t_name = Path.GetFileName(Application.ExecutablePath);
                string t_targetPath = Path.Combine(destPath, t_name);

                //Debug.Log("t_targetPath: " + t_targetPath);

                try
                {
                    p_CreateShortcut(t_targetPath, shortcutName);
                }
                catch (Exception)
                {
                }

                _t = null;
            }
        }

        private static void p_CreateShortcut(string filePath, string shortchutName)
        {
            object t_dto = (object)"Desktop";
            WshShell t_ws = new WshShell();
            string t_sca = (string)t_ws.SpecialFolders.Item(ref t_dto) + "\\" + shortchutName + ".lnk";
            IWshShortcut t_wsc = (IWshShortcut)t_ws.CreateShortcut(t_sca);
            //t_wsc.Description = shortchutName;
            //t_wsc.Hotkey = "Ctrl+Shift+N";
            t_wsc.TargetPath = filePath;
            t_wsc.WorkingDirectory = Path.GetDirectoryName(filePath);
            t_wsc.Save();
        }

/*
        private static void p_createShortcutToDesktop(string linkName)
        {
            string t_deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string t_filePath = t_deskPath + "\\" + linkName + ".url";
            string t_exePath = @"C:\Users\Hobis\Desktop\__test\main1.exe";
            using (StreamWriter writer = new StreamWriter(t_filePath, true, Encoding.ASCII))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + t_exePath);
                writer.WriteLine("IconIndex=0");
                string icon = t_exePath.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
        }*/
    }
}