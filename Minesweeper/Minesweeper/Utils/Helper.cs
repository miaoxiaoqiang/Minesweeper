using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Controls;

namespace Minesweeper.Utils
{
    internal static class Helper
    {
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        static Helper()
        {
            ExeIcon = GetExeIcon();
        }

        public static BitmapSource ExeIcon
        {
            get;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }

            return null;
        }

        public static List<T> FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> TList = new List<T> { };
                DependencyObject parent = VisualTreeHelper.GetParent(obj);
                if (parent != null && parent is T)
                {
                    TList.Add((T)parent);
                    List<T> parentOfParent = FindVisualParent<T>(parent);
                    if (parentOfParent != null)
                    {
                        TList.AddRange(parentOfParent);
                    }
                }
                else if (parent != null)
                {
                    List<T> parentOfParent = FindVisualParent<T>(parent);
                    if (parentOfParent != null)
                    {
                        TList.AddRange(parentOfParent);
                    }
                }
                return TList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            if (parent == null)
            {
                return null;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);

#pragma warning disable CS0162 // 检测到无法访问的代码
            for (int i = 0; i < count; i++)
            {
                var item = VisualTreeHelper.GetChild(parent, i);
                if (item is ScrollViewer viewer)
                {
                    return viewer;
                }
                else
                {
                    return GetScrollViewer(item);
                }
            }
#pragma warning restore CS0162 // 检测到无法访问的代码

            return null;
        }

        /// <summary>
        /// 去除字节数组尾部的空白区(0x00)
        /// </summary>
        /// <param name="bytes">字节数组对象</param>
        public static byte[] BytesTrimEnd(this byte[] bytes)
        {
            List<byte> _list = bytes.ToList();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == 0x00)
                {
                    _list.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            return _list.ToArray();
        }

        private static BitmapSource GetExeIcon()
        {
            string exefilepath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
            int count = ExtractIconEx(exefilepath, -1, null, null, 0);
            IntPtr[] largeIcons = new IntPtr[count];
            IntPtr[] smallIcons = new IntPtr[count];
            ExtractIconEx(exefilepath, 0, largeIcons, smallIcons, count);

            if (largeIcons != null && largeIcons.Length > 0)
            {
                BitmapSource source = Imaging.CreateBitmapSourceFromHIcon(largeIcons[0], Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            return null;
        }
    }
}
