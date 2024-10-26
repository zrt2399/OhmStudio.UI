using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace OhmStudio.UI.Helpers
{
    public static class PathHelper
    {
        public static readonly BitmapSource FolderIcon = GetFolderIcon(string.Empty);

        public static bool IsAbsolutePath(this string path)
        {
            return Path.IsPathRooted(path) && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public static string GetParentDirectory(this string path)
        {
            return Directory.GetParent(path)?.FullName ?? string.Empty;
        }

        public static bool Exists(string fullPath, bool isFolder)
        {
            if (isFolder)
            {
                return Directory.Exists(fullPath);
            }
            else
            {
                return File.Exists(fullPath);
            }
        }

        public static bool Exists(string fullPath, out bool isFolder)
        {
            isFolder = false;
            if (File.Exists(fullPath))
            {
                return true;
            }
            else
            {
                if (Directory.Exists(fullPath))
                {
                    isFolder = true;
                    return true;
                }
                return false;
            }
        }

        public static bool Exists(string fullPath)
        {
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        public static Process OpenFileLocation(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            return Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
        }

        public static Process OpenFlie(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName);
            //processStartInfo.WorkingDirectory = Directory.GetParent(fileName)?.FullName;
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.ErrorDialog = true;
            process.Start();
            return process;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>  
        /// 返回系统设置的图标。
        /// </summary>  
        /// <param name="pszPath">文件路径 如果为""  返回文件夹的</param>  
        /// <param name="dwFileAttributes">0</param>  
        /// <param name="psfi">结构体</param>  
        /// <param name="cbSizeFileInfo">结构体大小</param>  
        /// <param name="uFlags">枚举类型</param>  
        /// <returns>-1失败</returns>  
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public enum SHGFI
        {
            SHGFI_LARGEICON = 0x0,// 获取大图标
            SHGFI_SMALLICON = 0x1,// 获取小图标
            SHGFI_OPENICON = 0x2,// Specify open folder icon (if applicable)
            SHGFI_USEFILEATTRIBUTES = 0x10,//使用传递的文件属性，而不是实际文件系统中的属性。
            SHGFI_ICON = 0x100, //检索表示文件图标的句柄，以及系统映像列表中图标的索引。 句柄将复制到 psfi 指定的结构的 hIcon 成员，并将索引复制到 iIcon 成员。
            SHGFI_DISPLAYNAME = 0x200,//检索文件的显示名称，即 Windows 资源管理器中显示的名称。包含图标的文件的名称将复制到 psfi 指定的结构的 szDisplayName 成员。
            SHGFI_TYPENAME = 0x400//检索描述文件类型的字符串。字符串将复制到 psfi 中指定的结构的 szTypeName 成员。
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// 获取文件/文件夹图标。
        /// </summary>
        /// <param name="fullPath">文件/文件夹完全路径。</param>
        /// <param name="isFolder">指示路径是文件还是文件夹。</param>
        /// <param name="smallIcon">是否获取小图标。</param>
        /// <returns></returns>
        public static BitmapSource GetIcon(string fullPath, bool isFolder, bool smallIcon = false)
        {
            IntPtr hIcon = IntPtr.Zero;
            try
            {
                var uFlags = SHGFI.SHGFI_ICON | (smallIcon ? SHGFI.SHGFI_SMALLICON : SHGFI.SHGFI_LARGEICON);
                if (!string.IsNullOrEmpty(fullPath) && !isFolder)
                {
                    uFlags |= SHGFI.SHGFI_USEFILEATTRIBUTES;
                }
                SHFILEINFO sHFILEINFO = new SHFILEINFO();
                IntPtr iconIntPtr = SHGetFileInfo(fullPath, 0, ref sHFILEINFO, (uint)Marshal.SizeOf(sHFILEINFO), (uint)uFlags);
                if (iconIntPtr == IntPtr.Zero || sHFILEINFO.hIcon == IntPtr.Zero)
                {
                    return null;
                }
                hIcon = sHFILEINFO.hIcon;
                using Icon icon = Icon.FromHandle(hIcon);
                var iconBitmapSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                return iconBitmapSource;
            }
            finally
            {
                if (hIcon != IntPtr.Zero)
                {
                    DestroyIcon(hIcon);
                }
            }
        }

        public static BitmapSource GetFileIcon(string fullPath, bool smallIcon = false)
        {
            return GetIcon(fullPath, false, smallIcon);
        }

        public static BitmapSource GetFolderIcon(string fullPath, bool smallIcon = false)
        {
            return GetIcon(fullPath, true, smallIcon);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        //导入Windows API函数和一些常量
        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        [DllImport("shell32.dll")]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        //写调用查看文件属性的对话框
        public static void ShowPathProperties(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = fileName;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);
        }

        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public WFunc wFunc;
            public string pFrom;
            public string pTo;
            public FILEOP_FLAGS fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        private enum WFunc
        {
            FO_MOVE = 0x0001,  //移动文件
            FO_COPY = 0x0002,  //复制文件
            FO_DELETE = 0x0003, //删除文件，只是用pFrom
            FO_RENAME = 0x0004 //文件重命名
        }

        private enum FILEOP_FLAGS
        {
            FOF_MULTIDESTFILES = 0x0001,
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0044,
            FOF_RENAMEONCOLLISION = 0x0008,
            FOF_NOCONFIRMATION = 0x10,
            FOF_WANTMAPPINGHANDLE = 0x0020,
            FOF_ALLOWUNDO = 0x40,
            FOF_FILESONLY = 0x0080,
            FOF_SIMPLEPROGRESS = 0x0100,
            FOF_NOCONFIRMMKDIR = 0x0200,
            FOF_NOERRORUI = 0x0400,
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,
            FOF_NORECURSION = 0x1000
        }

        [DllImport("shell32.dll")]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        /// <summary>
        /// 删除单个文件。
        /// </summary>
        /// <param name="fileName">删除的文件名</param>
        /// <param name="toRecycle">指示是将文件放入回收站还是永久删除，true-放入回收站，false-永久删除</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认删除对话框，false-不显示确认删除对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框，true-显示，false-不显示。该参数当指定永久删除文件时有效</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <returns>操作执行结果标识，删除文件成功返回0，否则，返回错误代码。</returns>
        public static int DeletePath(string fileName, bool toRecycle, bool showDialog, bool showProgress, out string errorMsg)
        {
            try
            {
                return ToDelete(fileName, toRecycle, showDialog, showProgress, out errorMsg);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }

        private static int ToDelete(string fileName, bool toRecycle, bool showDialog, bool showProgress, out string errorMsg)
        {
            errorMsg = string.Empty;
            SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT();
            lpFileOp.wFunc = WFunc.FO_DELETE;
            lpFileOp.pFrom = fileName + "\0";    //将文件名以结尾字符"\0"结束
            lpFileOp.fFlags = FILEOP_FLAGS.FOF_NOERRORUI;
            if (toRecycle)
            {
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_ALLOWUNDO; //设定删除到回收站
            }

            if (!showDialog)
            {
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_NOCONFIRMATION;   //设定不显示提示对话框
            }

            if (!showProgress)
            {
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_SILENT;   //设定不显示进度对话框
            }

            lpFileOp.fAnyOperationsAborted = true;
            int num = SHFileOperation(ref lpFileOp);
            if (num == 0)
            {
                return 0;
            }

            string tmp = GetErrorString(num);
            //.av 文件正常删除了但也提示 402 错误，不知道为什么。屏蔽之。
            if (fileName.EndsWith(".av", StringComparison.OrdinalIgnoreCase) && num == 0x402)
            {
                return 0;
            }

            errorMsg = string.Format("{0}({1})", tmp, fileName);
            return num;
        }

        /// <summary>
        /// 解释错误代码。
        /// </summary>
        /// <param name="number">代码号。</param>
        /// <returns>返回关于错误代码的文字描述。</returns>
        private static string GetErrorString(int number)
        {
            if (number == 0)
            {
                return "Delete successful";
            }

            return number switch
            {
                2 => "系统找不到指定的文件。",
                7 => "存储控制块被销毁。您是否选择的\"取消\"操作？",
                113 => "文件已存在！",
                115 => "重命名文件操作,原始文件和目标文件必须具有相同的路径名。不能使用相对路径。",
                117 => "I/O控制错误",
                123 => "指定了重复的文件名",
                116 => "源是根目录，不能移动或重命名。",
                118 => "安全设置拒绝访问源。",
                124 => "源或目标或两者中的路径无效。",
                65536 => "目标上发生未指定的错误。",
                1026 => "在试图移动或拷贝一个不存在的文件。",
                1223 => "操作被取消！",
                _ => "未识别的错误代码：" + number,
            };
        }

        public static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            //await Task.Run(async () =>
            //{
            if (Directory.Exists(destinationFolder))
            {
                Directory.Delete(destinationFolder, true);
            }
            // 创建目标文件夹
            Directory.CreateDirectory(destinationFolder);

            // 获取源文件夹中的所有文件
            string[] files = Directory.GetFiles(sourceFolder);

            // 复制文件
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(destinationFolder, fileName);
                File.Copy(file, destinationPath);
            }

            // 获取源文件夹中的所有子文件夹
            string[] subFolders = Directory.GetDirectories(sourceFolder);

            // 递归复制子文件夹及其内容
            foreach (string subFolder in subFolders)
            {
                string folderName = Path.GetFileName(subFolder);
                string destinationPath = Path.Combine(destinationFolder, folderName);
                /*await*/
                CopyFolder(subFolder, destinationPath);
            }
            //});
        }

        public static bool IsSubdirectory(string candidateParent, string candidateChild)
        {
            var parent = new DirectoryInfo(candidateParent);
            var child = new DirectoryInfo(candidateChild);

            while (child.Parent != null)
            {
                if (child.Parent.FullName.Equals(parent.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                child = child.Parent;
            }

            return false;
        }

        public static List<string> GetAllFilesAndFolders(string folderPath)
        {
            List<string> fileList = new List<string>();

            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            // 检查参数是否是有效的文件夹路径
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException("Invalid folder path.");
            }

            // 获取当前文件夹内的文件和文件夹
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                fileList.Add(file.FullName);
            }

            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                fileList.Add(dir.FullName);

                // 递归调用，获取子文件夹内的所有文件和文件夹
                List<string> subFolderList = GetAllFilesAndFolders(dir.FullName);
                fileList.AddRange(subFolderList);
            }

            return fileList;
        }
    }
}