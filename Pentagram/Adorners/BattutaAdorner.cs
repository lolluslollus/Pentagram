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
	public sealed class BattutaAdorner : CanvasAdorner
	{
		private Battuta _battuta = null;
		public Battuta Battuta
		{
			get { return _battuta; }
			private set
			{
				bool isChanged = value != _battuta;
				if (isChanged)
				{
					if (_battuta != null)
					{
						_battuta.PropertyChanged -= OnBattuta_PropertyChanged;
						_battuta.Instants.CollectionChanged -= OnInstants_CollectionChanged;
					}
					_battuta = value;
					if (_battuta != null)
					{
						_battuta.PropertyChanged += OnBattuta_PropertyChanged;
						_battuta.Instants.CollectionChanged += OnInstants_CollectionChanged;
					}
				}
			}
		}
		private void OnInstants_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}
		private void OnBattuta_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
		public BattutaAdorner(Canvas parentLayoutRoot, Battuta battuta, bool isFirstInRow) : base(parentLayoutRoot)
		{
			Battuta = battuta;
			_isFirstInRow = isFirstInRow;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			var battuta = _battuta;
			if (battuta != null)
			{
				battuta.PropertyChanged -= OnBattuta_PropertyChanged;
				battuta.Instants.CollectionChanged -= OnInstants_CollectionChanged;
			}
			var adorners = _adorners;
			if (adorners == null) return;
			foreach (var item in adorners)
			{
				item?.Dispose();
			}
		}
		#endregion ctor and dispose

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

				if (_battuta?.Instants == null) return;

				double width = 0.0;
				// prepend chiave, armatura and misura if missing
				if (_isFirstInRow)
				{
					var first = _battuta.Instants.FirstOrDefault();
					if (first == null) return;

					bool hasChiave = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Chiave);
					bool hasArmatura = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Armatura);
					bool hasMisura = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Chiave);

					if (!hasChiave)
					{
						var adorner = new TabAdorner(_layoutRoot, new Tab(TabSymbols.Chiave), _battuta.Chiave, _battuta.Misura);
						_adorners.Add(adorner);
						Canvas.SetLeft(adorner.GetLayoutRoot(), width);
						width += adorner.GetWidth();
					}
					if (!hasArmatura)
					{
						var adorner = new TabAdorner(_layoutRoot, new Tab(TabSymbols.Armatura), _battuta.Chiave, _battuta.Misura);
						_adorners.Add(adorner);
						Canvas.SetLeft(adorner.GetLayoutRoot(), width);
						width += adorner.GetWidth();
					}
					if (!hasMisura)
					{
						var adorner = new TabAdorner(_layoutRoot, new Tab(TabSymbols.Misura), _battuta.Chiave, _battuta.Misura);
						_adorners.Add(adorner);
						Canvas.SetLeft(adorner.GetLayoutRoot(), width);
						width += adorner.GetWidth();
					}
				}

				foreach (var instant in _battuta.Instants)
				{
					CanvasAdorner adorner = new InstantAdorner(_layoutRoot, _battuta.Chiave, _battuta.Misura, instant);
					_adorners.Add(adorner);
					Canvas.SetLeft(adorner.GetLayoutRoot(), width);
					width += adorner.GetWidth();
				}
			});
		}

		public override double GetHeight()
		{
			var estimator = new BattutaAdornerEstimator(_battuta, _isFirstInRow);
			return estimator.GetHeight();
		}
		public override double GetWidth()
		{
			var estimator = new BattutaAdornerEstimator(_battuta, _isFirstInRow);
			return estimator.GetWidth();
		}
	}

	public sealed class BattutaAdornerEstimator : CanvasAdornerBase
	{
		private Battuta _battuta = null;
		private bool _isFirstInRow = false;

		private readonly List<CanvasAdorner> _adorners = new List<CanvasAdorner>();

		#region ctor and dispose
		public BattutaAdornerEstimator(Battuta battuta, bool isFirstInRow)
		{
			_battuta = battuta;
			_isFirstInRow = isFirstInRow;
		}
		#endregion ctor and dispose

		public override double GetHeight()
		{
			return PENTAGRAM_HEIGHT;
		}
		public override double GetWidth()
		{
			if (_battuta?.Instants == null) throw new ArgumentNullException("BattutaAdorner.GetWidth() needs a battuta with instants");

			double result = 0.0;
			// prepend chiave, armatura and misura if missing
			if (_isFirstInRow)
			{
				var first = _battuta.Instants.FirstOrDefault();
				if (first == null) return result;

				bool hasChiave = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Chiave);
				bool hasArmatura = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Armatura);
				bool hasMisura = first.SoundsOrTabs.Any(sot => sot is Tab && (sot as Tab).TabSymbol == TabSymbols.Chiave);

				if (!hasChiave)
				{
					var tryAdorner = new TabAdornerEstimator(new Tab(TabSymbols.Chiave), _battuta.Chiave, _battuta.Misura);
					result += tryAdorner.GetWidth();
				}
				if (!hasArmatura)
				{
					var tryAdorner = new TabAdornerEstimator(new Tab(TabSymbols.Armatura), _battuta.Chiave, _battuta.Misura);
					result += tryAdorner.GetWidth();
				}
				if (!hasMisura)
				{
					var tryAdorner = new TabAdornerEstimator(new Tab(TabSymbols.Misura), _battuta.Chiave, _battuta.Misura);
					result += tryAdorner.GetWidth();
				}
			}

			foreach (var instant in _battuta.Instants)
			{
				var tryAdorner = new InstantAdornerEstimator(_battuta.Chiave, _battuta.Misura, instant);
				result += tryAdorner.GetWidth();
			}

			return result;
		}
	}
}
