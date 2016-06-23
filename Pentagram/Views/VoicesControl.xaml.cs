using Pentagram.Adorners;
using Pentagram.PersistentData;
using Pentagram.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilz;
using Utilz.Controlz;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls;
using System.Threading.Tasks;
using System.Diagnostics;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pentagram.Views
{
	public sealed partial class VoicesControl : OpenableObservableControl
	{
		public SwitchableObservableCollection<Voice> Voices
		{
			get { return (SwitchableObservableCollection<Voice>)GetValue(VoicesProperty); }
			set { SetValue(VoicesProperty, value); }
		}
		public static readonly DependencyProperty VoicesProperty =
			DependencyProperty.Register("Voices", typeof(SwitchableObservableCollection<Voice>), typeof(VoicesControl), new PropertyMetadata(null, OnVoicesChanged));
		private static void OnVoicesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (args.NewValue == args.OldValue) return;
			var instance = obj as VoicesControl;
			instance.UpdateAfterVoicesChangedAsync();
		}

		private VoicesVM _vm = null;
		public VoicesVM VM { get { return _vm; } private set { _vm = value; RaisePropertyChanged_UI(); } }

		private BattutaHWrapAdorner _bhwa = null;


		#region lifecycle
		public VoicesControl()
		{
			InitializeComponent();
		}
		protected override async Task OpenMayOverrideAsync(object args = null)
		{
			await Settings.GetCreateInstance().OpenAsync();
			SizeChanged += OnSizeChanged;
			await UpdateAfterVoicesChanged2Async();
		}

		protected override Task CloseMayOverrideAsync()
		{
			SizeChanged -= OnSizeChanged;
			_bhwa?.Dispose();
			_bhwa = null;
			return Task.CompletedTask;
		}
		#endregion lifecycle

		private Task UpdateAfterVoicesChangedAsync()
		{
			return RunFunctionIfOpenAsyncT(UpdateAfterVoicesChanged2Async);
		}

		private Task UpdateAfterVoicesChanged2Async()
		{
			return RunInUiThreadAsync(() =>
			{
				var voices = Voices;

				_bhwa?.Dispose();
				if (voices == null)
				{
					_bhwa = null;
					VM = null;
				}
				else
				{
					_bhwa = new BattutaHWrapAdorner(LayoutRoot, voices, new Size(ActualWidth, ActualHeight), Settings.GetCreateInstance().FirstBattutaIndex);
					VM = new VoicesVM(voices);
				}
			});
		}

		public void OnPrevious_Click(object sender, RoutedEventArgs e)
		{
			Task goBack = RunFunctionIfOpenAsyncA(() =>
			{
				var bhwa = _bhwa;
				if (bhwa == null || bhwa.FirstBattutaIndex <= 0) return;

				var currentBIP = new BattutaHWrapAdornerEstimator(Voices, new Size(ActualWidth, ActualHeight), Settings.GetCreateInstance().FirstBattutaIndex);
				var currentBIP_bp = currentBIP.GetBattuteInPage();
				int tryStartBattutaIndex = Math.Max(0, bhwa.FirstBattutaIndex - currentBIP_bp.LastDrawnBattutaIndex);

				while (tryStartBattutaIndex <= currentBIP_bp.LastTotalBattutaIndex && tryStartBattutaIndex >= 0)
				{
					var tryBhwa = new BattutaHWrapAdornerEstimator(Voices, new Size(ActualWidth, ActualHeight), tryStartBattutaIndex);
					var tryBIP = tryBhwa.GetBattuteInPage();

					if (tryBIP.FirstBattutaIndex <= 0)
					{
						bhwa.FirstBattutaIndex = 0;
						Settings.GetCreateInstance().FirstBattutaIndex = 0;
						return;
					}

					if (tryBIP.LastDrawnBattutaIndex + 1 < currentBIP_bp.FirstBattutaIndex)
					{
						tryStartBattutaIndex++;
					}
					else if (tryBIP.LastDrawnBattutaIndex + 1 > currentBIP_bp.FirstBattutaIndex)
					{
						tryStartBattutaIndex--;
					}
					else
					{
						bhwa.FirstBattutaIndex = tryStartBattutaIndex;
						Settings.GetCreateInstance().FirstBattutaIndex = tryStartBattutaIndex;
						return;
					}
				}
			});
		}

		public void OnNext_Click(object sender, RoutedEventArgs e)
		{
			Task goForward = RunFunctionIfOpenAsyncA(() =>
			{
				var bhwa = _bhwa;
				if (bhwa == null) return;

				var currentBhwa = new BattutaHWrapAdornerEstimator(Voices, new Size(ActualWidth, ActualHeight), Settings.GetCreateInstance().FirstBattutaIndex);
				var currentBIP = currentBhwa.GetBattuteInPage();

				if (currentBIP.LastTotalBattutaIndex <= currentBIP.LastDrawnBattutaIndex) return;

				bhwa.FirstBattutaIndex = currentBIP.LastDrawnBattutaIndex + 1;
				Settings.GetCreateInstance().FirstBattutaIndex = bhwa.FirstBattutaIndex;
			});
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width == e.PreviousSize.Width && e.NewSize.Height == e.PreviousSize.Height) return;
			//Debug.WriteLine("newSize: Width=" + e.NewSize.Width + " Height=" + e.NewSize.Height);
			//Debug.WriteLine("ActualWidth=" + ActualWidth + " ActualHeight=" + ActualHeight);
			Task resize = RunFunctionIfOpenAsyncA(() =>
			{
				var bhwa = _bhwa;
				if (bhwa == null) return;
				bhwa.MaxSize = e.NewSize;
			});
		}

		private void OnPentagram_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Task task = RunFunctionIfOpenAsyncT(() =>
			{
				return RunInUiThreadAsync(() =>
				{
					_vm.AddNote(Voices[0], Chiavi.Violino, new Misura(), DurateCanoniche.Croma, PuntiDiValore.Nil, SegniSuNote.Nil, false, 3, NoteBianche.@do, Accidenti.Diesis);
				});
			});
		}
	}
}
