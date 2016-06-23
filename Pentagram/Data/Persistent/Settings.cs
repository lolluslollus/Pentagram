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
	public class Settings : OpenableObservableData
	{
		private const string DIR_NAME = "LolloSessionSettings";
		private const string FILENAME = "LolloSessionSettings.xml";

		private int _firstBattutaIndex = 0;
		[DataMember]
		public int FirstBattutaIndex
		{
			get { return _firstBattutaIndex; }
			set
			{
				if (_firstBattutaIndex == value || value < 0) return; _firstBattutaIndex = value; RaisePropertyChanged();
			}
		}

		#region ctor
		private static Settings _instance = null;
		private static readonly object _instanceLocker = new object();
		internal static Settings GetCreateInstance()
		{
			lock (_instanceLocker)
			{
				if (_instance == null) _instance = new Settings();
				return _instance;
			}
		}
		internal static Settings GetInstance()
		{
			lock (_instanceLocker)
			{
				return _instance;
			}
		}

		private Settings() { }
		#endregion ctor

		#region open close
		protected override Task OpenMayOverrideAsync(object args = null)
		{
			return LoadAsync();
		}
		protected override Task CloseMayOverrideAsync()
		{
			return SaveAsync();
		}

		private async Task LoadAsync()
		{
			Settings newSettings = null;

			try
			{
				var dir = await GetDirectoryAsync().ConfigureAwait(false);
				var file = await dir.CreateFileAsync(FILENAME, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

				using (var inStream = await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
				{
					using (var iinStream = inStream.AsStreamForRead())
					{
						var serializer = new DataContractSerializer(typeof(Settings));
						iinStream.Position = 0;
						if (iinStream.Length > 0) // it may be the first time: no errors
						{
							newSettings = (Settings)(serializer.ReadObject(iinStream));
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

			if (newSettings != null) await CopyFromAsync(newSettings).ConfigureAwait(false);
		}
		private Task CopyFromAsync(Settings source)
		{
			return RunInUiThreadAsync(() =>
			{
				FirstBattutaIndex = source.FirstBattutaIndex;
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

					var sessionDataSerializer = new DataContractSerializer(typeof(Settings));
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
				Debug.WriteLine("ended method Settings.SaveAsync()");
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
