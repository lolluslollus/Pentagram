using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Pentagram.Adorners
{
	public sealed class BattutaVStackAdorner : CanvasAdorner
	{
		private static readonly SolidColorBrush _lineStroke = new SolidColorBrush(Colors.Black);
		private static readonly SolidColorBrush _lineStrokeRed = new SolidColorBrush(Colors.Red);
		private static readonly SolidColorBrush _lineStrokeBlue = new SolidColorBrush(Colors.Blue);
		private static readonly double _lineStrokeThickness = 1.0;
		// 1 battuta each voice
		//private SwitchableObservableCollection<Battuta> _battute = new SwitchableObservableCollection<Battuta>();
		//public SwitchableObservableCollection<Battuta> Battute
		private List<Battuta> _battute = new List<Battuta>();
		public List<Battuta> Battute

		{
			get { return _battute; }
			private set
			{
				bool isChanged = value != _battute;
				if (isChanged)
				{
					//if (_battute != null)
					//{
					//	_battute.CollectionChanged -= OnBattute_CollectionChanged;
					//}
					_battute = value;
					//if (_battute != null)
					//{
					//	_battute.CollectionChanged += OnBattute_CollectionChanged;
					//}
				}
			}
		}
		private void OnBattute_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}

		private bool _isFirstInRow = false;
		public bool IsFirstInRow
		{
			get { return _isFirstInRow; }
			set
			{
				if (_isFirstInRow == value) return;
				_isFirstInRow = value;
				Draw();
			}
		}

		private readonly List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		//public BattutaVStackAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Battuta> battute) : base(parentLayoutRoot)
		public BattutaVStackAdorner(Canvas parentLayoutRoot, List<Battuta> battute, bool isFirstInRow) : base(parentLayoutRoot)
		{
			Battute = battute;
			_isFirstInRow = isFirstInRow;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			//if (_battute != null)
			//{
			//	_battute.CollectionChanged -= OnBattute_CollectionChanged;
			//}
			foreach (var adorner in _adorners)
			{
				adorner?.Dispose();
			}
		}
		#endregion ctor and dispose

		protected override void Draw()
		{
			if (_layoutRoot == null) return;
			Task upd = RunInUiThreadAsync(() =>
			{
				if (_layoutRoot == null) return;
				foreach (var adorner in _adorners)
				{
					adorner?.Dispose();
				}
				_adorners.Clear();
				_layoutRoot.Children.Clear();

				if (_battute == null) return;

				double height = 0.0;
				double width = 0.0;
				var battutaYs = new List<double>();
				foreach (var battuta in _battute)
				{
					var adorner = new BattutaAdorner(_layoutRoot, battuta, _isFirstInRow);
					_adorners.Add(adorner);
					Canvas.SetTop(adorner.GetLayoutRoot(), height);
					battutaYs.Add(height);
					height += adorner.GetHeight();
					width = Math.Max(adorner.GetWidth(), width);
				}

				foreach (var battutaY in battutaYs)
				{
					var lineYs = GetLineYs();
					foreach (var lineY in lineYs)
					{
						var line = new Line() { X1 = 0.0, X2 = width, Y1 = lineY + battutaY, Y2 = lineY + battutaY, Stroke = _lineStroke, StrokeThickness = _lineStrokeThickness };
						_layoutRoot.Children.Add(line);
					}
					var vBarLeft = new Line() { X1 = 0.0, X2 = 0.0, Y1 = lineYs.First() + battutaY, Y2 = lineYs.Last() + battutaY, Stroke = _lineStroke, StrokeThickness = _lineStrokeThickness };
					_layoutRoot.Children.Add(vBarLeft);
					var vBarRight = new Line() { X1 = width, X2 = width, Y1 = lineYs.First() + battutaY, Y2 = lineYs.Last() + battutaY, Stroke = _lineStroke, StrokeThickness = _lineStrokeThickness };
					_layoutRoot.Children.Add(vBarRight);
				}
			});
		}

		public override double GetHeight()
		{
			var estimator = new BattutaVStackAdornerEstimator(_battute, _isFirstInRow);
			return estimator.GetHeight();
		}

		public override double GetWidth()
		{
			var estimator = new BattutaVStackAdornerEstimator(_battute, _isFirstInRow);
			return estimator.GetWidth();
		}

		private static List<double> GetLineYs()
		{
			var result = new List<double>();
			result.Add(GetLineY(new Tone(4, NoteBianche.fa, Accidenti.Nil)));
			result.Add(GetLineY(new Tone(4, NoteBianche.re, Accidenti.Nil)));
			result.Add(GetLineY(new Tone(3, NoteBianche.si, Accidenti.Nil)));
			result.Add(GetLineY(new Tone(3, NoteBianche.sol, Accidenti.Nil)));
			result.Add(GetLineY(new Tone(3, NoteBianche.mi, Accidenti.Nil)));
			return result;
		}
	}

	public sealed class BattutaVStackAdornerEstimator : CanvasAdornerBase
	{
		// 1 battuta each voice
		//private SwitchableObservableCollection<Battuta> _battute = new SwitchableObservableCollection<Battuta>();
		//public SwitchableObservableCollection<Battuta> Battute
		private List<Battuta> _battute = new List<Battuta>();
		private bool _isFirstInRow = false;
		private List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		//public BattutaVStackAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Battuta> battute) : base(parentLayoutRoot)
		public BattutaVStackAdornerEstimator(List<Battuta> battute, bool isFirstInRow)
		{
			_battute = battute;
			_isFirstInRow = isFirstInRow;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			if (_battute == null) throw new ArgumentNullException("BattutaVStackAdorner.GetHeight() needs battute");

			double result = 0.0;

			foreach (var battuta in _battute)
			{
				var adorner = new BattutaAdornerEstimator(battuta, _isFirstInRow);
				result += adorner.GetHeight();
			}

			return result;
		}

		public override double GetWidth()
		{
			if (_battute == null) throw new ArgumentNullException("BattutaVStackAdorner.GetWidth() needs battute");

			double result = 0.0;

			foreach (var battuta in _battute)
			{
				var adorner = new BattutaAdornerEstimator(battuta, _isFirstInRow);
				result = Math.Max(adorner.GetWidth(), result);
			}

			return result;
		}
	}
}
