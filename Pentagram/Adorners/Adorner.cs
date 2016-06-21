using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilz;
using Utilz.Controlz;
using Utilz.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pentagram.Adorners
{
	public abstract class Adorner : ObservableData, IDisposable
	{
		public readonly static int HOW_MANY_WHITE_NOTES;
		public readonly static double PENTAGRAM_HEIGHT;
		public readonly static double LINE_GAP;
		public readonly static double NOTE_BALL_WIDTH;

		protected readonly Canvas _layoutRoot = null;

		#region ctor and dispose
		static Adorner()
		{
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
			PENTAGRAM_HEIGHT = (double)App.Current.Resources["PentagramHeight"];
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			NOTE_BALL_WIDTH = (double)App.Current.Resources["NoteBallWidth"];
		}
		public Adorner(Canvas parentLayoutRoot)
		{
			_layoutRoot = new Canvas() { Name = "LayoutRoot" };
			parentLayoutRoot.Children.Add(_layoutRoot);
		}
		public abstract void Dispose();
		#endregion ctor and dispose

		public Panel GetLayoutRoot()
		{
			return _layoutRoot;
		}
		public abstract double GetHeight();
		public abstract double GetWidth();
	}
}
