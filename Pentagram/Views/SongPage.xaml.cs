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
		public Song Song
		{
			get { return (Song)GetValue(SongProperty); }
			set { SetValue(SongProperty, value); }
		}
		public static readonly DependencyProperty SongProperty =
			DependencyProperty.Register("Song", typeof(Song), typeof(SongPage), new PropertyMetadata(null));

		private SongVM _vm = null;
		public SongVM VM { get { return _vm; } private set { _vm = value; RaisePropertyChanged_UI(); } }

		public SongPage()
        {			
			this.InitializeComponent();			
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			VM = new SongVM(e.Parameter as Song);
			base.OnNavigatedTo(e);
		}
		private void OnAddVoice_Click(object sender, RoutedEventArgs e)
		{
			_vm.AddVoice(Ritmi.qq, Chiavi.Violino);
		}

		private void OnGotoMain_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}
	}
}
