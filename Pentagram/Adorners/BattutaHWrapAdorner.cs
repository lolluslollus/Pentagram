using Pentagram.PersistentData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public sealed class BattutaHWrapAdorner : Adorner
	{
		//private double _maxHeight = 0.0;
		//public double MaxHeight { get { return _maxHeight; } set { if (_maxHeight == value) return; _maxHeight = value; Draw(); } }

		//private double _maxWidth = 0.0;
		//public double MaxWidth { get { return _maxWidth; } set { if (_maxWidth == value) return; _maxWidth = value; Draw(); } }

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

		private readonly List<Adorner> _adorners = new List<Adorner>();

		#region ctor and dispose
		public BattutaHWrapAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Voice> voices, Size maxSize) : base(parentLayoutRoot)
		{
			Voices = voices;
			MaxSize = maxSize;
			// Draw();
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
				
				double nextY = 0.0;
				double lastWidth = 0.0;
				for (int i = 0; i < maxBattute; i++)
				{
					// var battute = new SwitchableObservableCollection<Battuta>();
					var battute = new List<Battuta>();
					foreach (var voice in _voices)
					{
						if (voice.Battute.Count > i) battute.Add(voice.Battute[i]);
						else battute.Add(new Battuta(Chiavi.Violino, new Misura()));
					}
					var adorner = new BattutaVStackAdorner(_layoutRoot, battute);

					double nextObjHeight = adorner.GetHeight();
					double nextObjWidth = adorner.GetWidth();

					if (nextObjWidth >= _maxSize.Height) throw new ArgumentException("BattutaHWrapAdorner.Draw(): MaxHeight too small");
					if (nextObjHeight >= _maxSize.Width) throw new ArgumentException("BattutaHWrapAdorner.Draw(): MaxWidth too small");
					if (lastWidth + nextObjWidth <= _maxSize.Width)
					{ // append to current line
						Canvas.SetLeft(adorner.GetLayoutRoot(), lastWidth);
						Canvas.SetTop(adorner.GetLayoutRoot(), nextY);
						lastWidth += nextObjWidth;
					}
					else
					{ // wrap into next line
						nextY += nextObjHeight; // LOLLO TODO I assume here that all vertical stacks of battute have the same height
						if (nextY + nextObjHeight > _maxSize.Height) break; // no more space in page: get out
						Canvas.SetLeft(adorner.GetLayoutRoot(), 0.0);
						Canvas.SetTop(adorner.GetLayoutRoot(), nextY);
						lastWidth = nextObjWidth;
					}
				}
			});
		}
		#endregion draw

		public override double GetHeight()
		{
			return GetSize().Height;
		}

		public override double GetWidth()
		{
			return GetSize().Width;
		}

		public Size GetSize()
		{
			var result = Size.Empty;


			if (_voices == null) throw new ArgumentNullException("BattutaHWrapAdorner needs voices");

			int maxBattute = 0;
			foreach (var voice in _voices)
			{
				maxBattute = Math.Max(voice.Battute.Count, maxBattute);
			}

			double nextY = 0.0;
			double lastWidth = 0.0;
			for (int i = 0; i < maxBattute; i++)
			{
				// var battute = new SwitchableObservableCollection<Battuta>();
				var battute = new List<Battuta>();
				foreach (var voice in _voices)
				{
					if (voice.Battute.Count > i) battute.Add(voice.Battute[i]);
					else battute.Add(new Battuta(Chiavi.Violino, new Misura()));
				}
				var adorner = new BattutaVStackAdorner(null, battute);

				double nextObjHeight = adorner.GetHeight();
				double nextObjWidth = adorner.GetWidth();

				if (nextObjWidth >= _maxSize.Height) throw new ArgumentException("BattutaHWrapAdorner.Draw(): MaxHeight too small");
				if (nextObjHeight >= _maxSize.Width) throw new ArgumentException("BattutaHWrapAdorner.Draw(): MaxWidth too small");
				if (lastWidth + nextObjWidth <= _maxSize.Width)
				{ // append to current line
					lastWidth += nextObjWidth;
				}
				else
				{ // wrap into next line
					nextY += nextObjHeight; // LOLLO TODO I assume here that all vertical stacks of battute have the same height
					if (nextY + nextObjHeight > _maxSize.Height) break; // no more space in page: get out
					lastWidth = nextObjWidth;
				}
				result.Width = Math.Max(result.Width, lastWidth);
				result.Height = nextY + adorner.GetHeight(); // LOLLO TODO I assume here that all vertical stacks of battute have the same height
			}

			return result;
		}
	}
}
