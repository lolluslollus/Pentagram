using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;
using Windows.Storage;

namespace Pentagram.PersistentData
{
	[DataContract]
	public class All : OpenableObservableData
	{
		public const Chiavi DEFAULT_CHIAVE = Chiavi.Violino;
		public const Ritmi DEFAULT_RITMO = Ritmi.qq;
		private const string DIR_NAME = "LolloSessionAll";
		private const string FILENAME = "LolloSessionAll.xml";

		// we cannot make this prop readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<SongHeader> _songHeaders = new SwitchableObservableCollection<SongHeader>();
		[DataMember]
		public SwitchableObservableCollection<SongHeader> SongHeaders { get { return _songHeaders; } private set { _songHeaders = value; } }

		private SongHeader _currentSongHeader = null;
		[DataMember]
		public SongHeader CurrentSongHeader { get { return _currentSongHeader; } private set { _currentSongHeader = value; RaisePropertyChanged(); } }

		#region ctor
		private static All _instance = null;
		private static readonly object _instanceLocker = new object();
		internal static All GetCreateInstance()
		{
			lock (_instanceLocker)
			{
				if (_instance == null) _instance = new All();
				return _instance;
			}
		}

		private All() { }
		#endregion ctor

		#region while open
		public Task<bool> AddSongAsync(SongHeader song)
		{
			return RunFunctionIfOpenAsyncT(() =>
			{
				if (song == null) return Task.CompletedTask;
				return RunInUiThreadAsync(() =>
					_songHeaders.Add(song));
			});
		}
		public Task<bool> RemoveSongAsync(SongHeader songHeader)
		{
			return RunFunctionIfOpenAsyncT(async () =>
			{
				if (songHeader == null) return;

				bool isDeleteCurrentSong = _currentSongHeader?.Id == songHeader.Id;

				if (isDeleteCurrentSong)
				{
					var dir = await _currentSongHeader.GetDirectoryAsync().ConfigureAwait(false);
					await dir.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false);
				}

				await RunInUiThreadAsync(() =>
				{
					_songHeaders.Remove(songHeader);
					if (isDeleteCurrentSong) CurrentSongHeader = null;
				}).ConfigureAwait(false);
			});
		}
		public Task<bool> TrySetCurrentSongAsync(SongHeader songHeader)
		{
			return RunFunctionIfOpenAsyncTB(async () =>
			{
				if (songHeader?.IsValid != true) return false;
				if (_currentSongHeader?.Id == songHeader.Id)
				{
					await RunInUiThreadAsync(() =>
					{
						CurrentSongHeader = songHeader;
					}).ConfigureAwait(false);
					return true;
				}
				else
				{
					await RunInUiThreadAsync(() =>
					{
						CurrentSongHeader = songHeader;
					}).ConfigureAwait(false);
					return CurrentSongHeader?.IsValid == true;
				}
			});
		}
		#endregion while open

		#region open close
		protected override async Task OpenMayOverrideAsync(object args = null)
		{
			await LoadAsync();
		}
		protected override async Task CloseMayOverrideAsync()
		{
			await SaveAsync().ConfigureAwait(false);
		}
		private async Task LoadAsync()
		{
			All newAll = null;

			try
			{
				var dir = await GetDirectoryAsync().ConfigureAwait(false);
				var file = await dir.CreateFileAsync(FILENAME, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

				using (var inStream = await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
				{
					using (var iinStream = inStream.AsStreamForRead())
					{
						var serializer = new DataContractSerializer(typeof(All), new List<Type>() { typeof(SongHeader) });
						iinStream.Position = 0;
						if (iinStream.Length > 0) // it may be the first time: no errors
						{
							newAll = (All)(serializer.ReadObject(iinStream));
							await iinStream.FlushAsync().ConfigureAwait(false);
						}
					}
				}
			}
			catch (FileNotFoundException ex) //ignore file not found, this may be the first run just after installing
			{
				await Logger.AddAsync(ex.ToString(), Logger.FileErrorLogFilename);
			}
			catch (Exception ex) //same. must be tolerant or the app might crash when starting
			{
				await Logger.AddAsync(ex.ToString(), Logger.FileErrorLogFilename);
			}

			if (newAll != null) await CopyFromAsync(newAll).ConfigureAwait(false);
		}
		private Task CopyFromAsync(All source)
		{
			return RunInUiThreadAsync(() =>
			{
				CurrentSongHeader = source.CurrentSongHeader;
				_songHeaders.Clear();
				_songHeaders.AddRange(source.SongHeaders);
			});
		}
		private async Task SaveAsync()
		{
			//for (int i = 0; i < 100000000; i++) //wait a few seconds, for testing
			//{
			//    String aaa = i.ToString();
			//}

			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					var dir = await GetDirectoryAsync().ConfigureAwait(false);
					var file = await dir.CreateFileAsync(FILENAME, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);

					var sessionDataSerializer = new DataContractSerializer(typeof(All));
					_currentSongHeader = SongHeader.Clone(_currentSongHeader); // do not serialise any subclasses of SongHeader
					sessionDataSerializer.WriteObject(memoryStream, this);

					using (var fileStream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
					{
						fileStream.SetLength(0); // avoid leaving crap at the end if overwriting a file that was longer
						memoryStream.Seek(0, SeekOrigin.Begin);
						await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
						await memoryStream.FlushAsync().ConfigureAwait(false);
						await fileStream.FlushAsync().ConfigureAwait(false);
					}
				}
				Debug.WriteLine("ended method All.SaveAsync()");
			}
			catch (Exception ex)
			{
				Logger.Add_TPL(ex.ToString(), Logger.FileErrorLogFilename);
			}
		}
		#endregion open close

		#region utilz
		internal static Task<StorageFolder> GetDirectoryAsync()
		{
			var root = ApplicationData.Current.LocalFolder;
			return root.CreateFolderAsync(DIR_NAME, CreationCollisionOption.OpenIfExists).AsTask();
		}
		#endregion utilz
	}
}
