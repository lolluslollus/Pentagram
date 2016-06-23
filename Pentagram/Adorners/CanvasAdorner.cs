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

namespace Pentagram.Adorners
{
	public abstract class CanvasAdorner : CanvasAdornerBase, IDisposable
	{
		protected readonly Canvas _layoutRoot = null;

		#region ctor and dispose
		public CanvasAdorner(Canvas parentLayoutRoot)
		{
			if (parentLayoutRoot == null) return;
			_layoutRoot = new Canvas() { Name = GetType().Name + "LayoutRoot" };
			parentLayoutRoot.Children.Add(_layoutRoot);
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{ }
		#endregion ctor and dispose

		public Panel GetLayoutRoot()
		{
			return _layoutRoot;
		}

		protected abstract void Draw();
	}

	public abstract class CanvasAdornerBase : ObservableData
	{
		public readonly static int HOW_MANY_WHITE_NOTES;
		public readonly static double PENTAGRAM_HEIGHT;
		public readonly static double LINE_GAP;
		public readonly static double PENTAGRAM_HEIGHT_HALF;
		public readonly static double LINE_GAP_HALF;
		public readonly static double NOTE_BALL_WIDTH;
		public readonly static double ARMATURA_WIDTH;
		public readonly static double CHIAVE_WIDTH;
		public readonly static double REFRAIN_WIDTH;
		public readonly static double MISURA_WIDTH;
		public readonly static double VERTICAL_BAR_WIDTH;
		public readonly static Thickness MARGIN_THICKNESS_0;

		#region ctor and dispose
		static CanvasAdornerBase()
		{
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
			PENTAGRAM_HEIGHT = (double)App.Current.Resources["PentagramHeight"];
			PENTAGRAM_HEIGHT_HALF = PENTAGRAM_HEIGHT * .5;
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			LINE_GAP_HALF = LINE_GAP * .5;
			NOTE_BALL_WIDTH = LINE_GAP;
			ARMATURA_WIDTH = (double)App.Current.Resources["ArmaturaWidth"];
			CHIAVE_WIDTH = (double)App.Current.Resources["ChiaveWidth"];
			REFRAIN_WIDTH = (double)App.Current.Resources["RefrainWidth"];
			MISURA_WIDTH = (double)App.Current.Resources["RitmoWidth"];
			VERTICAL_BAR_WIDTH = (double)App.Current.Resources["VerticalBarWidth"];
			MARGIN_THICKNESS_0 = new Thickness(0.0);
		}
		public CanvasAdornerBase() { }
		#endregion ctor and dispose

		public abstract double GetHeight();
		public abstract double GetWidth();

		protected static double GetLineY(Tone tone)
		{
			return ((3.0 - Convert.ToDouble(tone.Ottava)) * HOW_MANY_WHITE_NOTES + ((int)NoteBianche.si - (int)tone.NotaBianca)) * LINE_GAP_HALF + PENTAGRAM_HEIGHT_HALF;
		}
		protected static double GetLineY(Chiavi chiave, Tone tone)
		{
			double lineY = GetLineY(tone);
			if (chiave == Chiavi.Violino) lineY -= LINE_GAP_HALF;
			else if (chiave == Chiavi.Basso) lineY -= LINE_GAP * 6.5;
			return lineY;
		}
	}
}
