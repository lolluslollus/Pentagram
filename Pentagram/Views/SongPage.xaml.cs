using Pentagram.PersistentData;
using Pentagram.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pentagram.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SongPage : ObservablePage
    {
		private SongVM _vm = null;
		public SongVM VM { get { return _vm; } private set { _vm = value; RaisePropertyChanged_UI(); } }

		public SongPage()
        {			
			InitializeComponent();			
        }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			VM = new SongVM(e.Parameter as SongHeader);
			await _vm.OpenAsync().ConfigureAwait(false);
			VoicesControl.Voices = VM.Song.Voices;
		}
		protected override async void OnNavigatedFrom(NavigationEventArgs e)
		{
			VoicesControl.Voices = null;
			await _vm.CloseAsync().ConfigureAwait(false);
		}

		private void OnAddVoice_Click(object sender, RoutedEventArgs e)
		{
			_vm.AddVoiceAsync(All.DEFAULT_RITMO, All.DEFAULT_CHIAVE);
		}

		private void OnGotoMain_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}
	}
}
