// from http://saftsack.fs.uni-bayreuth.de/~dun3/archives/howto-savely-move-a-file-using-c/145.html
// Howto safely move a file using C#
// by Tobias Hertkorn on March 1st, 2008
// If possible this version uses the special File.Replace on NTFS 
// and graciously falls back on Delete+Move on any other file system. 
// Nothing else to comment about here. I am just amazed that this 
// kind of save moving is not supported by the framework itself. 
// Instead the framework's move routine throws an exception 
// if there is an file existing at the target location. Weird. ;)
// Just a heads up: Don't use this in performance critical parts...
// Andy Pearmund - Jan 2009 - added backup support for non-NTFS file systems.

using System;
using System.IO;

namespace ID3Tagging.MP3Lib.Utils
{
    /// <summary>
    /// Provides FileMove function to wrap System.IO.File.Replace
    /// </summary>
    public static class FileMover
    {
        #region Public Methods and Operators

        /// <summary>
        /// Securely moves a file to a new location. Overwrites any
        /// preexisting file at new location (= replacing file).
        /// </summary>
        /// <remarks>
        /// If NTFS is available this is done via File.Replace.
        /// If NTFS is not available it will be moved via deleting
        /// any preexisting file and moving. Do NOT rely on the
        /// backupFile being there - or not - after the move process.
        /// That is not predetermined. This method is clearly
        /// optimized for the case that NTFS is available. Consider NOT
        /// using it on any other file system, if performance is an issue!
        /// </remarks>
        /// <param name="sourceLocation">
        /// The file to be moved.
        /// </param>
        /// <param name="targetLocation">
        /// The new resting place of the file.
        /// </param>
        /// <param name="backupLocation">
        /// A backup location that is used when replacing.
        /// could be null if no backup required
        /// </param>
        public static void FileMove(FileInfo sourceLocation, FileInfo targetLocation, FileInfo backupLocation)
        {
            if (targetLocation.Exists)
            {
                try
                {
                    File.Replace(
                        sourceLocation.FullName,
                        targetLocation.FullName,
                        backupLocation != null ? backupLocation.FullName : null,
                        true);
                }
                catch (PlatformNotSupportedException)
                {
                    // Not operating on an NTFS volume.
                    if (backupLocation != null)
                    {
                        NonNtfsReplace(sourceLocation, targetLocation, backupLocation);
                    }
                    else
                    {
                        NonNtfsReplace(sourceLocation, targetLocation);
                    }
                }
            }
            else
            {
                File.Move(sourceLocation.FullName, targetLocation.FullName);
            }
        }

        /// <summary>
        /// replace for non NTFS file systems, keeping a backup.
        /// </summary>
        /// <param name="sourceLocation">
        /// The file to be moved.
        /// </param>
        /// <param name="targetLocation">
        /// The new resting place of the file.
        /// </param>
        /// <param name="backupLocation">
        /// A backup location that will contain the old target afterwards.
        /// </param>
        public static void NonNtfsReplace(FileInfo sourceLocation, FileInfo targetLocation, FileInfo backupLocation)
        {
            if (backupLocation == null)
            {
                throw new ArgumentNullException("backupLocation");
            }

            // a unique name for the safety backup. 
            FileInfo safetyBackupLocation = null;

            if (backupLocation.Exists)
            {
                // The caller supplied a backup location. 
                // If that file already exists we must not destroy it 
                // unless we can replace it with a good file

                // create a unique name for safetyBackup
                safetyBackupLocation = GetTempBackup(backupLocation);

                // rename the old backup to safetyBackup
                // if this fails we just throw; no permanent changes have been made yet.
                File.Move(backupLocation.FullName, safetyBackupLocation.FullName);
            }

            try
            {
                // rename the old target to backup
                // if this fails we need to rename the old safetyBackup back (if any)
                File.Move(targetLocation.FullName, backupLocation.FullName);
            }
            catch
            {
                if (safetyBackupLocation != null)
                {
                    // we've got a safetyBackup to unwind too
                    // we managed to rename it once before, so this will probably work again
                    File.Move(safetyBackupLocation.FullName, backupLocation.FullName);
                }

                throw;
            }

            try
            {
                // rename source to target
                // if this fails we need to rename the old target back
                File.Move(sourceLocation.FullName, targetLocation.FullName);
            }
            catch
            {
                // we managed to rename it once before, so this will probably work again
                File.Move(backupLocation.FullName, targetLocation.FullName);

                if (safetyBackupLocation != null)
                {
                    // we've got a safetyBackup to unwind too
                    File.Move(safetyBackupLocation.FullName, backupLocation.FullName);
                }

                throw;
            }

            // now finally get rid of the left-over backups
            if (safetyBackupLocation != null)
            {
                try
                {
                    safetyBackupLocation.Delete();
                }
                catch
                {
                    // if this fails we can ignore it, as the main operation has succeeded.
                }
            }
        }

        /// <summary>
        /// replace for non NTFS file systems without keeping a backup.
        /// </summary>
        /// <param name="sourceLocation">
        /// The file to be moved.
        /// </param>
        /// <param name="targetLocation">
        /// The new resting place of the file.
        /// </param>
        /// <remarks>
        /// it uses a temporary backup location while swapping the files, but deletes it afterwards.
        /// </remarks>
        public static void NonNtfsReplace(FileInfo sourceLocation, FileInfo targetLocation)
        {
            // The caller didn't supply a backup location,
            // so they don't want a backup kept.
            // create a unique name for the temporary Backup. This won't exist yet, so we don't have to test it.
            FileInfo backupLocation = GetTempBackup(targetLocation);

            // rename the old target to backup
            // if this fails we just throw; no permanent changes have been made yet.
            File.Move(targetLocation.FullName, backupLocation.FullName);

            try
            {
                // rename source to target
                // if this fails we need to rename the old target back
                File.Move(sourceLocation.FullName, targetLocation.FullName);
            }
            catch
            {
                // we managed to rename it once before, so this will probably work again
                File.Move(backupLocation.FullName, targetLocation.FullName);
                throw;
            }

            try
            {
                backupLocation.Delete();
            }
            catch
            {
                // if this fails we can ignore it, as the main operation has succeeded.
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a temporary backup filename
        /// </summary>
        /// <param name="originalLocation">
        /// The original location.
        /// </param>
        /// <returns>
        /// a new filename in the same directory as the original file
        /// </returns>
        private static FileInfo GetTempBackup(FileInfo originalLocation)
        {
            if (originalLocation == null)
            {
                throw new ArgumentNullException("originalLocation");
            }

            string tempBackupName = Path.Combine(originalLocation.DirectoryName, Path.GetRandomFileName());
            return new FileInfo(tempBackupName);
        }

        #endregion
    }
}