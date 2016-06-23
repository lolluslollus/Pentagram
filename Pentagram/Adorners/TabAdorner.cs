using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Pentagram.Adorners
{
	public sealed class TabAdorner : CanvasAdorner
	{
		private static readonly BitmapImage _chiaveDiViolino = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/chiaveDiViolino_160x370.png", UriKind.Absolute) };
		private static readonly BitmapImage _chiaveDiBasso = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/chiaveDiBasso_160x244.png", UriKind.Absolute) };
		private static readonly BitmapImage _ritmoQQ = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/ritmo_qq_186x266.png", UriKind.Absolute) };
		private static readonly BitmapImage _ritmoTQ = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/ritmo_tq_186x266.png", UriKind.Absolute) };
		private static readonly BitmapImage _ritmoDQ = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/Symbols/ritmo_dq_186x266.png", UriKind.Absolute) };

		private readonly Chiavi _chiave;
		private readonly Misura _misura;

		private Tab _tab = null;
		public Tab Tab
		{
			get { return _tab; }
			private set
			{
				bool isChanged = value != _tab;
				if (isChanged)
				{
					if (_tab != null) _tab.PropertyChanged -= OnTab_PropertyChanged;
					_tab = value;
					if (_tab != null) _tab.PropertyChanged += OnTab_PropertyChanged;
				}
			}
		}
		private void OnTab_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Draw();
		}

		#region ctor and dispose
		public TabAdorner(Canvas parentLayoutRoot, Tab tab, Chiavi chiave, Misura misura) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			_misura = misura;
			Tab = tab;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_tab != null) _tab.PropertyChanged -= OnTab_PropertyChanged;
		}
		#endregion ctor and dispose

		protected override void Draw()
		{
			if (_layoutRoot == null) return;
			Task upd = RunInUiThreadAsync(() =>
			{
				if (_layoutRoot == null) return;
				_layoutRoot.Children.Clear();

				if (_tab == null) return;

				switch (_tab.TabSymbol)
				{
					case TabSymbols.Nil:
						return;
					case TabSymbols.Chiave:
						if (_chiave == Chiavi.Violino)
						{
							var symbolImage = new Image() { Source = _chiaveDiViolino, Width = CHIAVE_WIDTH };
							_layoutRoot.Children.Add(symbolImage);
							double lineY = GetLineY(new Tone(3, NoteBianche.sol, Accidenti.Nil));
							Canvas.SetTop(symbolImage, lineY - CHIAVE_WIDTH / 160 * 185); // the numbers here comes from the image size
						}
						else if (_chiave == Chiavi.Basso)
						{
							var symbolImage = new Image() { Source = _chiaveDiBasso, Width = CHIAVE_WIDTH };
							_layoutRoot.Children.Add(symbolImage);
							double lineY = GetLineY(new Tone(4, NoteBianche.re, Accidenti.Nil));
							Canvas.SetTop(symbolImage, lineY - CHIAVE_WIDTH / 160 * 122); // the numbers here comes from the image size
						}
						return;
					case TabSymbols.Armatura:
						return;
					case TabSymbols.Misura:
						var numTB = new TextBlock() { Text = _misura.Num.ToString(), FontSize = 72, FontFamily = new FontFamily("Snap ITC"), Padding = MARGIN_THICKNESS_0, Margin = MARGIN_THICKNESS_0 };
						var denTB = new TextBlock() { Text = _misura.Den.ToString(), FontSize = 72, FontFamily = new FontFamily("Snap ITC"), Padding = MARGIN_THICKNESS_0, Margin = MARGIN_THICKNESS_0 };
						var numVB = new Viewbox() { Child = numTB, Height = 2 * LINE_GAP, Margin = MARGIN_THICKNESS_0 };
						var denVB = new Viewbox() { Child = denTB, Height = 2 * LINE_GAP, Margin = MARGIN_THICKNESS_0 };
						_layoutRoot.Children.Add(numVB);
						_layoutRoot.Children.Add(denVB);
						double numY = GetLineY(new Tone(4, NoteBianche.fa, Accidenti.Nil));
						double denY = GetLineY(new Tone(3, NoteBianche.si, Accidenti.Nil));
						Canvas.SetTop(numVB, numY);
						Canvas.SetTop(denVB, denY);
						return;
					case TabSymbols.TwoVerticalBars:
						return;
					case TabSymbols.Refrain:
						return;
					default:
						return;
				}
			});
		}

		public override double GetHeight()
		{
			var estimator = new TabAdornerEstimator(_tab, _chiave, _misura);
			return estimator.GetHeight();
		}

		public override double GetWidth()
		{
			var estimator = new TabAdornerEstimator(_tab, _chiave, _misura);
			return estimator.GetWidth();
		}
	}

	public sealed class TabAdornerEstimator : CanvasAdornerBase
	{
		private readonly Chiavi _chiave;
		private readonly Misura _misura;
		private Tab _tab = null;

		#region ctor and dispose
		public TabAdornerEstimator(Tab tab, Chiavi chiave, Misura misura)
		{
			_chiave = chiave;
			_misura = misura;
			_tab = tab;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_tab == null) throw new ArgumentNullException("TabAdorner.GetWidth() needs a tab");
			switch (_tab.TabSymbol)
			{
				case TabSymbols.Nil:
					return NOTE_BALL_WIDTH;
				case TabSymbols.Chiave:
					return CHIAVE_WIDTH;
				case TabSymbols.Armatura:
					return ARMATURA_WIDTH;
				case TabSymbols.Misura:
					return MISURA_WIDTH;
				case TabSymbols.TwoVerticalBars:
					return VERTICAL_BAR_WIDTH;
				case TabSymbols.Refrain:
					return REFRAIN_WIDTH;
				default:
					return NOTE_BALL_WIDTH;
			}
		}
	}
}
