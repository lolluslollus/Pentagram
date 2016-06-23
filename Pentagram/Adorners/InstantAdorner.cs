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
	public sealed class InstantAdorner : CanvasAdorner
	{
		private readonly Chiavi _chiave;
		private readonly Misura _misura;

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

		private readonly List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		public InstantAdorner(Canvas parentLayoutRoot, Chiavi chiave, Misura misura, InstantWithTouches instant) : base(parentLayoutRoot)
		{
			_chiave = chiave;
			_misura = misura;
			Instant = instant;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_instant != null)
			{
				_instant.PropertyChanged -= OnInstant_PropertyChanged;
				_instant.SoundsOrTabs.CollectionChanged -= OnSoundsOrTabs_CollectionChanged;
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

				if (_instant?.SoundsOrTabs == null) return;

				foreach (var soundOrTab in _instant.SoundsOrTabs)
				{
					CanvasAdorner adorner = null;
					if (soundOrTab is Pause) adorner = new PauseAdorner(_layoutRoot, _chiave, soundOrTab as Pause);
					else if (soundOrTab is Chord) adorner = new ChordAdorner(_layoutRoot, _chiave, soundOrTab as Chord);
					else if (soundOrTab is Tab) adorner = new TabAdorner(_layoutRoot, soundOrTab as Tab, _chiave, _misura);
					if (adorner != null) _adorners.Add(adorner);
				}
			});
		}
		#endregion draw

		public override double GetHeight()
		{
			var estimator = new InstantAdornerEstimator(_chiave, _misura, _instant);
			return estimator.GetHeight();
		}

		public override double GetWidth()
		{
			var estimator = new InstantAdornerEstimator(_chiave, _misura, _instant);
			return estimator.GetWidth();
		}
	}

	public sealed class InstantAdornerEstimator : CanvasAdornerBase
	{
		private readonly Chiavi _chiave;
		private readonly Misura _misura;
		private InstantWithTouches _instant = null;

		private readonly List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		public InstantAdornerEstimator(Chiavi chiave, Misura misura, InstantWithTouches instant)
		{
			_chiave = chiave;
			_misura = misura;
			_instant = instant;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}

		public override double GetWidth()
		{
			if (_instant?.SoundsOrTabs == null) throw new ArgumentNullException("InstantAdorner.GetWidth() needs an instant with sounds or tabs");

			double result = 0.0;

			foreach (var soundOrTab in _instant.SoundsOrTabs)
			{
				CanvasAdornerBase adornerWoutCanvas = null;
				if (soundOrTab is Pause) adornerWoutCanvas = new PauseAdornerEstimator(_chiave, soundOrTab as Pause);
				else if (soundOrTab is Chord) adornerWoutCanvas = new ChordAdornerEstimator(_chiave, soundOrTab as Chord);
				else if (soundOrTab is Tab) adornerWoutCanvas = new TabAdornerEstimator(soundOrTab as Tab, _chiave, _misura);
				if (adornerWoutCanvas != null) result = Math.Max(adornerWoutCanvas.GetWidth(), result);
			}

			return result;
		}
	}
}
