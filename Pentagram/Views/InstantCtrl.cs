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

namespace Pentagram.Views
{
	public sealed class InstantCtrl : ObservableControl
	{
		public Chiavi Chiave
		{
			get { return (Chiavi)GetValue(ChiaveProperty); }
			set { SetValue(ChiaveProperty, value); }
		}
		public static readonly DependencyProperty ChiaveProperty =
			DependencyProperty.Register("Chiave", typeof(Chiavi), typeof(InstantCtrl), new PropertyMetadata(All.DEFAULT_CHIAVE));

		public SwitchableObservableCollection<Sound> Sounds
		{
			get { return (SwitchableObservableCollection<Sound>)GetValue(SoundsProperty); }
			set { SetValue(SoundsProperty, value); }
		}
		public static readonly DependencyProperty SoundsProperty =
			DependencyProperty.Register("Sounds", typeof(SwitchableObservableCollection<Sound>), typeof(InstantCtrl), new PropertyMetadata(null, OnSoundsChanged));
		private static void OnSoundsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var instance = obj as InstantCtrl;
			var oldValue = args.OldValue as SwitchableObservableCollection<Sound>;
			var newValue = args.NewValue as SwitchableObservableCollection<Sound>;
			if (oldValue != newValue && instance != null)
			{
				instance.RemoveHandlers(oldValue);
				instance.AddHandlers(newValue);
				instance.Draw();
			}
		}

		private readonly List<Adorner> _adorners = new List<Adorner>();

		private readonly Canvas LayoutRoot = null;
		public InstantCtrl()
		{
			LayoutRoot = new Canvas() { Name = "LayoutRoot" };
			Content = LayoutRoot;
			AddHandlers(Sounds);
			Draw();
		}

		private void AddHandlers(SwitchableObservableCollection<Sound> sounds)
		{
			if (sounds == null) return;
			sounds.CollectionChanged += OnSounds_CollectionChanged;
			//foreach (var sound in sounds)
			//{
			//	sound.PropertyChanged += OnSound_PropertyChanged;
			//}
		}

		private void RemoveHandlers(SwitchableObservableCollection<Sound> sounds)
		{
			if (sounds == null) return;
			//foreach (var sound in sounds)
			//{
			//	sound.PropertyChanged -= OnSound_PropertyChanged;
			//}
			sounds.CollectionChanged -= OnSounds_CollectionChanged;
		}

		//private void OnSound_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		//{
		//	Draw();
		//}

		private void OnSounds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Draw();
		}

		private void Draw()
		{
			Task upd = RunInUiThreadAsync(() =>
			{
				foreach (var child in _adorners)
				{
					child?.Dispose();
				}
				_adorners.Clear();
				LayoutRoot.Children.Clear();

				if (Sounds == null) return;
				// set width
				var red = new SolidColorBrush(Colors.LightPink);
				var blue = new SolidColorBrush(Colors.LightBlue);
				var bkg = red;
				double layoutRootWidth = Adorner.LINE_GAP * 5.0; // 3.0; // LOLLO TODO restore when done testing
				foreach (var sound in Sounds)
				{
					if (sound is Chord && ((sound as Chord).NextJoinedChord != null /*|| (sound as Chord).PrevJoinedChords != null*/))
					{
						layoutRootWidth = Adorner.LINE_GAP * 2.0;
						bkg = blue;
						break;
					}
				}
				LayoutRoot.Width = layoutRootWidth;
				LayoutRoot.Height = Adorner.PENTAGRAM_HEIGHT;
				LayoutRoot.Background = bkg;
				// draw children
				foreach (var sound in Sounds)
				{
					Adorner adorner = null;
					if (sound is Pause) adorner = new PauseAdorner(LayoutRoot, Chiave, sound as Pause);
					else if (sound is Chord) adorner = new ChordAdorner(LayoutRoot, Chiave, sound as Chord);
					if (adorner != null) _adorners.Add(adorner);
				}
			});
		}
	}

	public abstract class Adorner : ObservableData, IDisposable
	{
		public readonly static int HOW_MANY_WHITE_NOTES;
		public readonly static double PENTAGRAM_HEIGHT;
		public readonly static double LINE_GAP;

		static Adorner()
		{
			PENTAGRAM_HEIGHT = (double)App.Current.Resources["PentagramHeight"];
			LINE_GAP = (double)App.Current.Resources["LineGap"];
			HOW_MANY_WHITE_NOTES = Enum.GetValues(typeof(NoteBianche)).GetLength(0);
		}
		public abstract void Dispose();
	}
}
