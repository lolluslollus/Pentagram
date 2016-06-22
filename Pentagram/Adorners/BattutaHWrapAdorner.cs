﻿using Pentagram.PersistentData;
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
		private double _maxWidth = 0.0;
		public double MaxWidth { get { return _maxWidth; } set { if (_maxWidth == value) return; _maxWidth = value; Draw(); } }

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
		public BattutaHWrapAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Voice> voices, double maxWidth) : base(parentLayoutRoot)
		{
			Voices = voices;
			MaxWidth = maxWidth;
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
		private void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				foreach (var child in _adorners)
				{
					child?.Dispose();
				}
				_adorners.Clear();
				_layoutRoot.Children.Clear();

				_width = _height = 0.0;
				if (_voices == null) return;


				int maxBattute = 0;
				foreach (var voice in _voices)
				{
					maxBattute = Math.Max(voice.Battute.Count, maxBattute);
				}
				
				double lastHeight = 0.0;
				double lastWidth = 0.0;
				for (int i = 0; i < maxBattute; i++)
				{
					var battute = new SwitchableObservableCollection<Battuta>();
					foreach (var voice in _voices)
					{
						if (voice.Battute.Count > i) battute.Add(voice.Battute[i]);
						else battute.Add(new Battuta(Chiavi.Violino, new Misura()));
					}
					var adorner = new BattutaVStackAdorner(_layoutRoot, battute);

					double nextObjHeight = adorner.GetHeight();
					double nextObjWidth = adorner.GetWidth();

					if (nextObjWidth >= _maxWidth) throw new ArgumentException("BattutaHWrapAdorner.Draw(): MaxWidth too small");
					if (lastWidth + nextObjWidth <= _maxWidth)
					{ // append to current line
						Canvas.SetLeft(adorner.GetLayoutRoot(), lastWidth);
						Canvas.SetTop(adorner.GetLayoutRoot(), lastHeight);
						lastWidth += nextObjWidth;
					}
					else
					{ // wrap into next line
						lastHeight += adorner.GetHeight(); // LOLLO TODO I assume here that all vertical stacks of battute have the same height
						Canvas.SetLeft(adorner.GetLayoutRoot(), 0.0);
						Canvas.SetTop(adorner.GetLayoutRoot(), lastHeight);
						lastWidth = nextObjWidth;
					}
					_width = Math.Max(_width, lastWidth);
					_height = lastHeight + adorner.GetHeight(); // LOLLO TODO I assume here that all vertical stacks of battute have the same height
				}
			});
		}
		private double _height = 0.0;
		private double _width = 0.0;
		#endregion draw

		public override double GetHeight()
		{
			return _height;
			//if (_voices.Count < 1) return PENTAGRAM_HEIGHT;
			//return _voices.Count * PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			return _width;
			//double result = 0.0;
			//foreach (var adorner in _adorners)
			//{
			//	result = Math.Max(adorner.GetWidth(), result);
			//}
			//return result;
		}
	}
}
