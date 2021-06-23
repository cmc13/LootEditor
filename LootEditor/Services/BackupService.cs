using LootEditor.Models;
using System;
using System.IO;
using System.Windows.Threading;

namespace LootEditor.Services
{
    public class BackupService
    {
        private static readonly string BACKUP_FILE_NAME = Path.Combine(FileSystemService.AppDataDirectory, "backup.utl");
        private readonly DispatcherTimer backupTimer;
        private readonly FileSystemService fileSystemService;
        private LootFile backupFile = null;
        private string saveFileName = null;

        public BackupService()
            :this(new())
        { }

        public BackupService(FileSystemService fileSystemService)
        {
            backupTimer = new DispatcherTimer();
            backupTimer.Tick += BackupTimer_Tick;
            backupTimer.Interval = TimeSpan.FromMinutes(5);
            this.fileSystemService = fileSystemService;
        }

        private async void BackupTimer_Tick(object sender, EventArgs e)
        {
            using var fs = fileSystemService.OpenFileForWriteAccess(BACKUP_FILE_NAME);
            using var writer = new StreamWriter(fs);
            await writer.WriteLineAsync(saveFileName ?? "").ConfigureAwait(false);
            await backupFile.WriteFileAsync(writer).ConfigureAwait(false);
        }

        public void StartBackups(LootFile file, string fileName = null)
        {
            backupFile = file;
            saveFileName = fileName;
            if (!backupTimer.IsEnabled)
                backupTimer.Start();
        }

        public Stream OpenBackupFileForReadAccess()
        {
            var fs = fileSystemService.OpenFileForReadAccess(BACKUP_FILE_NAME);
            using var reader = new StreamReader(fs, leaveOpen: true);
            reader.ReadLine();
            return fs;
        }

        public bool BackupExists => fileSystemService.FileExists(BACKUP_FILE_NAME);

        public string BackupFileName
        {
            get
            {
                using var fs = fileSystemService.OpenFileForReadAccess(BACKUP_FILE_NAME);
                using var reader = new StreamReader(fs);
                var fileName = reader.ReadLine();
                if (string.IsNullOrEmpty(fileName))
                    fileName = null;
                return fileName;
            }
        }

        public void DeleteBackupFile()
        {
            fileSystemService.DeleteFile(BACKUP_FILE_NAME);
        }

        public void StopBackups()
        {
            if (backupTimer.IsEnabled)
                backupTimer.Stop();
            DeleteBackupFile();
            backupFile = null;
            saveFileName = null;
        }
    }
}
