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
	public sealed class BattutaVStackAdorner : Adorner
	{
		// 1 battuta each voice
		private SwitchableObservableCollection<Battuta> _battute = new SwitchableObservableCollection<Battuta>();
		public SwitchableObservableCollection<Battuta> Battute
		{
			get { return _battute; }
			private set
			{
				bool isChanged = value != _battute;
				if (isChanged)
				{
					if (_battute != null)
					{
						_battute.CollectionChanged -= OnBattute_CollectionChanged;
					}
					_battute = value;
					if (_battute != null)
					{
						_battute.CollectionChanged += OnBattute_CollectionChanged;
					}
				}
			}
		}
		private void OnBattute_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}

		private readonly List<Adorner> _adorners = new List<Adorner>();

		#region ctor and dispose
		public BattutaVStackAdorner(Canvas parentLayoutRoot, SwitchableObservableCollection<Battuta> battute) : base(parentLayoutRoot)
		{
			Battute = battute;
			Draw();
		}
		protected override void Dispose(bool isDisposing)
		{
			if (_battute != null)
			{
				_battute.CollectionChanged -= OnBattute_CollectionChanged;
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

				if (_battute == null) return;

				double height = 0.0;
				foreach (var battuta in _battute)
				{
					var adorner = new BattutaAdorner(_layoutRoot, battuta);
					_adorners.Add(adorner);
					Canvas.SetTop(adorner.GetLayoutRoot(), height);
					height += adorner.GetHeight();
				}
			});
		}
		#endregion draw

		public override double GetHeight()
		{
			double result = 0.0;
			foreach (var adorner in _adorners)
			{
				result += adorner.GetHeight();
			}
			return result;
		}

		public override double GetWidth()
		{
			double result = 0.0;
			foreach (var adorner in _adorners)
			{
				result = Math.Max(adorner.GetWidth(), result);
			}
			return result;
		}
	}
}
