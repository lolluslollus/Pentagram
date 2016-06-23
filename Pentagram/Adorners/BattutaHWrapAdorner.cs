using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Utilz;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Pentagram.Adorners
{
	public sealed class BattutaHWrapAdorner : CanvasAdorner
	{
		private int _firstBattutaIndex = 0;
		public int FirstBattutaIndex { get { return _firstBattutaIndex; } set { if (_firstBattutaIndex == value) return; _firstBattutaIndex = value; Draw(); } }

		private Size _maxSize = Size.Empty;
		public Size MaxSize { get { return _maxSize; } set { if (_maxSize == value) return; _maxSize = value; Draw(); } }

		private SwitchableObservableCollection<Voice> _voices = new SwitchableObservableCollection<Voice>();
		public SwitchableObservableCollection<Voice> Voices
		{
			get { return _voices; }
			private set
			{
				bool isChanged = value != _voices;
				if (isChanged)
				{
					if (_voices != null)
					{
						_voices.CollectionChanged -= OnVoices_CollectionChanged;
					}
					_voices = value;
					if (_voices != null)
					{
						_voices.CollectionChanged += OnVoices_CollectionChanged;
					}
				}
			}
		}
		private void OnVoices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}

		private readonly List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		public BattutaHWrapAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Voice> voices, Size maxSize, int startBattutaIndex) : base(parentLayoutRoot)
		{
			Voices = voices;
			_maxSize = maxSize;
			_firstBattutaIndex = startBattutaIndex;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_voices != null)
			{
				_voices.CollectionChanged -= OnVoices_CollectionChanged;
			}
			foreach (var item in _adorners)
			{
				item?.Dispose();
			}
		}
		#endregion ctor and dispose

		#region draw
		protected override void Draw()
		{
			if (_layoutRoot == null) return;
			Task upd = RunInUiThreadAsync(() =>
			{
				if (_layoutRoot == null) return;
				foreach (var child in _adorners)
				{
					child?.Dispose();
				}
				_adorners.Clear();
				_layoutRoot.Children.Clear();

				if (_voices == null) return;


				int maxBattute = 0;
				foreach (var voice in _voices)
				{
					maxBattute = Math.Max(voice.Battute.Count, maxBattute);
				}

				double lastX1 = 0.0;
				double lastY0 = 0.0;
				double lastY1 = 0.0;
				for (int i = _firstBattutaIndex; i < maxBattute; i++)
				{
					// var battute = new SwitchableObservableCollection<Battuta>();
					var battute = new List<Battuta>();
					foreach (var voice in _voices)
					{
						if (voice.Battute.Count > i) battute.Add(voice.Battute[i]);
						else battute.Add(new Battuta(Chiavi.Violino, new Misura()));
					}

					var nextObjProps = Helper.GetNextObjectProperties(battute, i == _firstBattutaIndex, lastX1, lastY1, _maxSize);
					if (!nextObjProps.IsFits) break;

					if (!nextObjProps.IsStartsNewRow)
					{ // append to current line
						var adorner = new BattutaVStackAdorner(_layoutRoot, battute, nextObjProps.IsFirstInRow);
						Canvas.SetLeft(adorner.GetLayoutRoot(), lastX1);
						Canvas.SetTop(adorner.GetLayoutRoot(), lastY0);
						lastX1 += nextObjProps.Width;
						lastY1 = Math.Max(lastY1, nextObjProps.Height);
						_adorners.Add(adorner);
					}
					else
					{ // wrap into next line
						var adorner = new BattutaVStackAdorner(_layoutRoot, battute, true);
						Canvas.SetLeft(adorner.GetLayoutRoot(), 0.0);
						Canvas.SetTop(adorner.GetLayoutRoot(), lastY1);
						lastX1 = nextObjProps.Width;
						lastY0 += lastY1;
						lastY1 += nextObjProps.Height;
						_adorners.Add(adorner);
					}
					//_lastBattutaIndex = i;
				}
			});
		}
		#endregion draw

		public override double GetHeight()
		{
			// return GetBattuteInPage().Size.Height;
			throw new NotImplementedException("BattutaHWrapAdorner.GetHeight() should not be called");
		}

		public override double GetWidth()
		{
			// return GetBattuteInPage().Size.Width;
			throw new NotImplementedException("BattutaHWrapAdorner.GetWidth() should not be called");
		}
	}

	public sealed class BattutaHWrapAdornerEstimator : CanvasAdornerBase
	{
		private readonly int _firstBattutaIndex = 0;
		private readonly Size _maxSize = Size.Empty;
		private readonly SwitchableObservableCollection<Voice> _voices = new SwitchableObservableCollection<Voice>();

		#region ctor and dispose
		public BattutaHWrapAdornerEstimator(SwitchableObservableCollection<Voice> voices, Size maxSize, int firstBattutaIndex)
		{
			_voices = voices;
			_maxSize = maxSize;
			_firstBattutaIndex = firstBattutaIndex;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			// return GetBattuteInPage().Size.Height;
			throw new NotImplementedException("BattutaHWrapAdorner.GetHeight() should not be called");
		}

		public override double GetWidth()
		{
			// return GetBattuteInPage().Size.Width;
			throw new NotImplementedException("BattutaHWrapAdorner.GetWidth() should not be called");
		}

		public Helper.BattuteInPage GetBattuteInPage()
		{
			if (_voices == null) throw new ArgumentNullException("BattutaHWrapAdorner.GetBattuteInPage() needs voices");

			var result = new Helper.BattuteInPage() { FirstBattutaIndex = _firstBattutaIndex };

			int maxBattute = 0;
			foreach (var voice in _voices)
			{
				maxBattute = Math.Max(voice.Battute.Count, maxBattute);
			}
			result.LastTotalBattutaIndex = maxBattute - 1;

			double lastX1 = 0.0;
			double lastY0 = 0.0;
			double lastY1 = 0.0;
			for (int i = _firstBattutaIndex; i < maxBattute; i++)
			{
				// var battute = new SwitchableObservableCollection<Battuta>();
				var battute = new List<Battuta>();
				foreach (var voice in _voices)
				{
					if (voice.Battute.Count > i) battute.Add(voice.Battute[i]);
					else battute.Add(new Battuta(Chiavi.Violino, new Misura()));
				}

				var nextObjProps = Helper.GetNextObjectProperties(battute, i == _firstBattutaIndex, lastX1, lastY1, _maxSize);
				if (!nextObjProps.IsFits) break;

				if (!nextObjProps.IsStartsNewRow)
				{ // append to current line
					lastX1 += nextObjProps.Width;
					lastY1 = Math.Max(lastY1, nextObjProps.Height);
				}
				else
				{ // wrap into next line
					lastX1 = nextObjProps.Width;
					lastY0 += lastY1;
					lastY1 += nextObjProps.Height;
				}
				result.LastDrawnBattutaIndex = i;
				result.Size.Width = Math.Max(result.Size.Width, lastX1);
				result.Size.Height = lastY1;
			}

			return result;
		}
	}

	public class Helper
	{
		public static NextObjectProperties GetNextObjectProperties(List<Battuta> battute, bool isFirstBattuta, double lastX1, double lastY1, Size maxSize)
		{
			var result = new NextObjectProperties { Width = 0.0, Height = 0.0, IsFits = false, IsFirstInRow = false, IsStartsNewRow = false };

			var tryAdorner = new BattutaVStackAdornerEstimator(battute, isFirstBattuta);
			result.Height = tryAdorner.GetHeight();
			result.Width = tryAdorner.GetWidth();
			// LOLLO TODO when you break here, try reducing LINE_GAP temporarily.
			// Alternatively, display a symbol telling there is a piece missing, right or down.
			// Scrolling is probably not a good idea.
			//Debug.WriteLine("next object height=" + nextObjHeight);
			//Debug.WriteLine("_maxSize.Height=" + _maxSize.Height);
			if (result.Width > maxSize.Width || result.Height > maxSize.Height) return result;

			// no point wrapping the first: OK
			if (isFirstBattuta)
			{
				result.IsFits = true;
				result.IsFirstInRow = true;
				return result;
			}
			// fits in current row: OK
			if (lastX1 + result.Width <= maxSize.Width)
			{
				result.IsFits = true;
				return result;
			}
			// try again with isFirstBattuta = true, ie wrap into next line
			tryAdorner = new BattutaVStackAdornerEstimator(battute, true);
			result.Height = tryAdorner.GetHeight();
			result.Width = tryAdorner.GetWidth();
			if (result.Width > maxSize.Width || result.Height > maxSize.Height) return result;
			// no space for a new line
			if (result.Height + lastY1 > maxSize.Height) return result;

			result.IsFits = true;
			result.IsFirstInRow = true;
			result.IsStartsNewRow = true;
			return result;
		}
		public struct NextObjectProperties
		{
			public double Width;
			public double Height;
			public bool IsFits;
			public bool IsFirstInRow;
			public bool IsStartsNewRow;
		}
		public struct BattuteInPage
		{
			public Size Size;
			public int FirstBattutaIndex;
			public int LastDrawnBattutaIndex;
			public int LastTotalBattutaIndex;
		}
	}
}
