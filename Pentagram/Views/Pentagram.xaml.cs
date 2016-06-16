using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilz.Controlz;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pentagram
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Pentagram : ObservableControl
    {
		public readonly int MinY0 = 50;
		public readonly int MinY1 = 50 + LineGap;
		public readonly int MinY2 = 50 + LineGap + LineGap;
		public readonly int MinY3 = 50 + LineGap + LineGap + LineGap;
		public readonly int MinY4 = 50 + LineGap + LineGap + LineGap + LineGap;
		public readonly int MinX = 0;
		public readonly int MaxX = 600;
		public const int LineGap = 25;

		public Pentagram()
        {
            this.InitializeComponent();
        }
    }
}
