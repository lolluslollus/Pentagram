﻿using Pentagram.PersistentData;
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
	public sealed class InstantAdorner : Adorner
	{
		private readonly Chiavi _chiave;
		private readonly Ritmi _ritmo;

		private InstantWithTouches _instant = null;
		public InstantWithTouches Instant
		{
			get { return _instant; }
			private set
			{
				bool isChanged = value != _instant;
				if (isChanged)
				{
					if (_instant != null)
					{
						_instant.PropertyChanged -= OnInstant_PropertyChanged;
						_instant.SoundsOrTabs.CollectionChanged -= OnSoundsOrTabs_CollectionChanged;
					}
					_instant = value;
					if (_instant != null)
					{
						_instant.PropertyChanged += OnInstant_PropertyChanged;
						_instant.SoundsOrTabs.CollectionChanged += OnSoundsOrTabs_CollectionChanged;
					}
				}
			}
		}
		private void OnSoundsOrTabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}
		private void OnInstant_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Draw();
		}

		private readonly List<Adorner> _adorners = new List<Adorner>();

		#region ctor and dispose
		public InstantAdorner(Canvas parentLayoutRoot, Chiavi chiave, Ritmi ritmo, InstantWithTouches instant) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			Instant = instant;
			Draw();
		}
		public override void Dispose()
		{
			if (_instant != null)
			{
				_instant.PropertyChanged -= OnInstant_PropertyChanged;
				_instant.SoundsOrTabs.CollectionChanged -= OnSoundsOrTabs_CollectionChanged;
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

				if (_instant?.SoundsOrTabs == null) return;
				//// set width
				//var red = new SolidColorBrush(Colors.LightPink);
				//var blue = new SolidColorBrush(Colors.LightBlue);
				//var bkg = red;
				//double layoutRootWidth = Adorner.LINE_GAP * 5.0; // 3.0; // LOLLO TODO restore when done testing
				//foreach (var sound in Sounds)
				//{
				//	if (sound is Chord && ((sound as Chord).NextJoinedChord != null /*|| (sound as Chord).PrevJoinedChords != null*/))
				//	{
				//		layoutRootWidth = Adorner.LINE_GAP * 2.0;
				//		bkg = blue;
				//		break;
				//	}
				//}
				//// LayoutRoot.Width = layoutRootWidth;
				//_layoutRoot.Width = Adorner.NOTE_BALL_WIDTH;
				//_layoutRoot.Height = Adorner.PENTAGRAM_HEIGHT;
				//_layoutRoot.Background = bkg;

				// draw children
				foreach (var soundOrTab in _instant.SoundsOrTabs)
				{
					Adorner adorner = null;
					if (soundOrTab is Pause) adorner = new PauseAdorner(_layoutRoot, _chiave, soundOrTab as Pause);
					else if (soundOrTab is Chord) adorner = new ChordAdorner(_layoutRoot, _chiave, soundOrTab as Chord);
					else if (soundOrTab is Tab) adorner = new TabAdorner(_layoutRoot, soundOrTab as Tab, _chiave, _ritmo);
					if (adorner != null) _adorners.Add(adorner);
				}

			});
		}
		#endregion draw

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_adorners == null) throw new ArgumentNullException("InstantAdorner.GetWidth() needs an instant");
			double result = 0.0;
			foreach (var adorner in _adorners)
			{
				result = Math.Max(result, adorner.GetWidth());
			}
			return result;
		}
	}
}