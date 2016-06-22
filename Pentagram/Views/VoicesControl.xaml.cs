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
		protected override Task OpenMayOverrideAsync(object args = null)
		{
			SizeChanged += OnVoicesControl_SizeChanged;
			return UpdateAfterVoicesChanged2Async();
		}

		private void OnVoicesControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width == e.PreviousSize.Width && e.NewSize.Height == e.PreviousSize.Height) return;
			var bhwa = _bhwa;
			if (bhwa != null) bhwa.MaxSize = e.NewSize;
		}

		protected override Task CloseMayOverrideAsync()
		{
			SizeChanged -= OnVoicesControl_SizeChanged;
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
					_bhwa = new BattutaHWrapAdorner(LayoutRoot, voices, new Size(ActualWidth, ActualHeight));
					VM = new VoicesVM(voices);
				}
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
