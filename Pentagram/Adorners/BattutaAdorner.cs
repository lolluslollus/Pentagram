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
	public sealed class BattutaAdorner : Adorner
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

		private readonly List<Adorner> _adorners = new List<Adorner>();

		#region ctor and dispose
		public BattutaAdorner(Canvas parentLayoutRoot, Battuta battuta) : base(parentLayoutRoot)
		{
			Battuta = battuta;
			Draw();
		}
		public override void Dispose()
		{
			if (_battuta != null)
			{
				_battuta.PropertyChanged -= OnBattuta_PropertyChanged;
				_battuta.Instants.CollectionChanged -= OnInstants_CollectionChanged;
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

				if (_battuta?.Instants == null) return;
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
				double width = 0.0;
				foreach (var instant in _battuta.Instants)
				{
					Adorner adorner = new InstantAdorner(_layoutRoot, _battuta.Chiave, _battuta.Ritmo, instant);
					_adorners.Add(adorner);
					Canvas.SetLeft(adorner.GetLayoutRoot(), width);
					width += adorner.GetWidth();
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
			if (_adorners == null) throw new ArgumentNullException("BattutaAdorner.GetWidth() needs an instant");
			double result = 0.0;
			foreach (var adorner in _adorners)
			{
				result += adorner.GetWidth();
			}
			return result;
		}
	}
}
