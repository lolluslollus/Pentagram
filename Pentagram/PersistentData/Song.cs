using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilz;
using Utilz.Data;
using Windows.Storage;

namespace Pentagram.PersistentData
{
	[DataContract]
	public class SongBase : OpenableObservableData
	{
		protected string _id = string.Empty;
		// the setter is only for the serialiser
		[DataMember]
		public string Id { get { return _id; } private set { if (_id == value) return; _id = value; RaisePropertyChanged(); } }

		[IgnoreDataMember]
		public bool IsValid { get { return !string.IsNullOrWhiteSpace(_id); } }

		public SongBase(string id)
		{
			_id = id;
		}
		public SongBase()
		{
			_id = Guid.NewGuid().ToString();
		}

		#region utilz
		protected const string DIR_NAME = "LolloSessionSongs";
		protected const string BODY_FILE_NAME = "Body.xml";

		internal async Task<StorageFolder> GetDirectoryAsync()
		{
			if (!IsValid) return null;
			var root = await GetRootDirectoryAsync().ConfigureAwait(false);
			return await root.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
		}
		internal static async Task<StorageFolder> GetRootDirectoryAsync()
		{
			return await ApplicationData.Current.LocalFolder.CreateFolderAsync(DIR_NAME, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
		}
		//internal static async Task<List<string>> GetIdsFromFilesAsync()
		//{
		//	var result = new List<string>();
		//	var rootDir = await GetRootDirectoryAsync().ConfigureAwait(false);
		//	var dirs = await rootDir.GetFoldersAsync().AsTask().ConfigureAwait(false);
		//	foreach (var dir in dirs)
		//	{
		//		result.Add(Path.GetDirectoryName(dir.Name));
		//	}
		//	return result;
		//}
		//private string GetFileName()
		//{
		//	if (!IsValid) throw new ArgumentException("song id must not be null");
		//	return _id + ".xml";
		//}
		#endregion utilz
	}

	[DataContract]
	public class SongHeader : SongBase
	{
		protected string _name = null;
		[DataMember]
		public string Name { get { return _name; } set { if (_name == value) return; _name = value; RaisePropertyChanged(); } }

		public SongHeader(string id) : base(id) { }
		public SongHeader(string name, bool newSong) : base()
		{
			Name = name;
		}
		public static SongHeader Clone(SongHeader source)
		{
			if (source?.IsValid != true) return null;
			return new SongHeader(source.Id) { Name = source.Name };
		}
	}

	[DataContract]
	public class Song : SongBase
	{
		// we cannot make this readonly because it is serialised. we only use the setter for serialising.
		private SwitchableObservableCollection<Voice> _voices = new SwitchableObservableCollection<Voice>();
		[DataMember]
		public SwitchableObservableCollection<Voice> Voices { get { return _voices; } private set { _voices = value; } }

		#region ctor
		private static Song _instance = null;
		private static readonly Semaphore _instanceSemaphore = new Semaphore(1, 1);
		internal static async Task<Song> GetCreateInstanceAsync(string id)
		{
			try
			{
				_instanceSemaphore.WaitOne();
				if (_instance == null)
				{
					_instance = new Song(id);
				}
				else if (_instance.Id != id)
				{
					await _instance.CloseAsync().ConfigureAwait(false);
					_instance = new Song(id);
				}
				return _instance;
			}
			finally
			{
				_instanceSemaphore.TryRelease();
			}
		}
		internal static Song GetInstance()
		{
			try
			{
				_instanceSemaphore.WaitOne();
				return _instance;
			}
			finally
			{
				_instanceSemaphore.TryRelease();
			}
		}

		private Song(string id) : base(id) { }
		#endregion ctor

		#region while open
		public Task<bool> AddVoiceAsync(Voice voice)
		{
			return RunFunctionIfOpenAsyncT(() =>
			{
				if (voice == null) return Task.CompletedTask;
				return RunInUiThreadAsync(() =>
					_voices.Add(voice));
			});
		}
		public Task<bool> RemoveVoiceAsync(Voice voice)
		{
			return RunFunctionIfOpenAsyncT(() =>
			{
				if (voice == null) return Task.CompletedTask;
				return RunInUiThreadAsync(() =>
					_voices.Remove(voice));
			});
		}
		#endregion while open

		#region open close
		protected override Task OpenMayOverrideAsync(object args = null)
		{
			return LoadAsync();
		}
		protected override Task CloseMayOverrideAsync()
		{
			return SaveAsync();
		}
		private readonly static List<Type> _knownTypes = new List<Type>()
		{
			typeof(Voice), typeof(InstantWithTouches), typeof(Chord), /*typeof(Note), */typeof(Tone), typeof(Pause), typeof(Duration)
		};
		private async Task LoadAsync()
		{
			Song newSong = null;

			try
			{
				var dir = await GetDirectoryAsync().ConfigureAwait(false);
				var file = await dir
					.CreateFileAsync(BODY_FILE_NAME, CreationCollisionOption.OpenIfExists)
					.AsTask().ConfigureAwait(false);

				using (var inStream = await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
				{
					using (var iinStream = inStream.AsStreamForRead())
					{
						var serializer = new DataContractSerializer(typeof(Song), new DataContractSerializerSettings() { PreserveObjectReferences = true, KnownTypes = _knownTypes });
						iinStream.Position = 0;
						if (iinStream.Length > 0) // we may be dealing with a new song: no errors
						{
							newSong = (Song)(serializer.ReadObject(iinStream));
							await iinStream.FlushAsync().ConfigureAwait(false);
						}
					}
				}
			}
			catch (FileNotFoundException ex) //ignore file not found, this may be the first run just after installing
			{
				await Logger.AddAsync(ex.ToString(), Logger.FileErrorLogFilename);
			}
			catch (Exception ex) //same, must be tolerant or the app might crash when starting
			{
				await Logger.AddAsync(ex.ToString(), Logger.FileErrorLogFilename);
			}

			if (newSong != null) await CopyFromAsync(newSong).ConfigureAwait(false);
		}
		private Task CopyFromAsync(Song source)
		{
			return RunInUiThreadAsync(() =>
			{
				// no Id!
				_voices.Clear();
				_voices.AddRange(source.Voices);
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
				var dir = await GetDirectoryAsync().ConfigureAwait(false);

				var bodyFile = await dir
					.CreateFileAsync(BODY_FILE_NAME, CreationCollisionOption.ReplaceExisting)
					.AsTask().ConfigureAwait(false);

				var bodySerializer = new DataContractSerializer(typeof(Song), new DataContractSerializerSettings() { PreserveObjectReferences = true, KnownTypes = _knownTypes });

				using (MemoryStream memoryStream = new MemoryStream())
				{
					bodySerializer.WriteObject(memoryStream, this);

					using (var fileStream = await bodyFile.OpenStreamForWriteAsync().ConfigureAwait(false))
					{
						fileStream.SetLength(0); // avoid leaving crap at the end if overwriting a file that was longer
						memoryStream.Seek(0, SeekOrigin.Begin);
						await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
						await memoryStream.FlushAsync().ConfigureAwait(false);
						await fileStream.FlushAsync().ConfigureAwait(false);
					}
				}
			}
			// LOLLO TODO Object graph for type 'Pentagram.PersistentData.Chord' contains cycles and cannot be serialized if references are not tracked. 
			// Consider using the DataContractAttribute with the IsReference property set to true.
			catch (Exception ex)
			{
				Logger.Add_TPL(ex.ToString(), Logger.FileErrorLogFilename);
			}
		}
		#endregion open close
	}
}
