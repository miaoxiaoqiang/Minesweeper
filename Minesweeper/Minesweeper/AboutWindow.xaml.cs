using System;
using System.ComponentModel;
using System.Reflection;

namespace Minesweeper
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class AboutWindow : XPWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyFileVersionAttribute asmdis = (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute));
            VersionText.Text = "版本 " + asmdis.Version;
            AssemblyCopyrightAttribute asmcpr = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            CopyrightText.Text = asmcpr.Copyright;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void InlineUIContainer_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/miaoxiaoqiang/Minesweeper.git")).Dispose();
        }
    }
}
