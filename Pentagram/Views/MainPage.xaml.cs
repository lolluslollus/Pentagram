using Pentagram.PersistentData;
using Pentagram.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pentagram.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly AllVM _vm = null;
		public AllVM VM { get { return _vm; } }

		public MainPage()
		{
			_vm = new AllVM();
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Task open = _vm.OpenAsync();
		}
		private void OnAddSong_Click(object sender, RoutedEventArgs e)
		{
			_vm.AddSongAsync("new song");
		}

		private void OnRemoveSong_Click(object sender, RoutedEventArgs e)
		{
			var songHeader = (e.OriginalSource as FrameworkElement).DataContext as SongHeader;
			_vm.RemoveSongAsync(songHeader);
		}

		private async void OnSetCurrentSong_Click(object sender, RoutedEventArgs e)
		{
			var songHeader = (e.OriginalSource as FrameworkElement).DataContext as SongHeader;
			if (await _vm.TrySetCurrentSongAsync(songHeader))
			{
				Frame.Navigate(typeof(SongPage), songHeader);
			}
		}
	}
}
