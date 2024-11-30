using System;
using System.Collections.Generic;
using WooW.SB.ManagerDirectory.OptionsDirectory;

namespace WooW.SB.ManagerDirectory
{
    public class WoDirectory
    {
        #region CreateDirectory

        private static WoCreateDirectory _woCreateDirectory = new WoCreateDirectory();

        public static void CreateDirectory(string pathDirectory)
        {
            _woCreateDirectory.CreateDirectory(pathDirectory);
        }

        public static void CreateFile(string pathFile)
        {
            _woCreateDirectory.CreateFile(pathFile);
        }

        #endregion CreateDirectory

        #region DeleteDirectory

        private static WoDeleteDirectory _woDeleteDirectory = new WoDeleteDirectory();

        public static void DeleteDirectory(string pathDirectory)
        {
            try
            {
                _woDeleteDirectory.DeleteDirectory(pathDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteFile(string pathFile)
        {
            try
            {
                _woDeleteDirectory.DeleteFile(pathFile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion DeleteDirectory

        #region WriteTemplate

        private static WoWriteTemplate _woWriteTemplate = new WoWriteTemplate();

        public static void WriteTemplate(string pathTemplate, string data)
        {
            _woWriteTemplate.WriteTemplate(pathTemplate, data);
        }

        #endregion WriteTemplate

        #region WriteFile

        private static WoWriteFile _woWriteFile = new WoWriteFile();

        public static void WriteFile(string path, string data)
        {
            try
            {
                _woWriteFile.WriteFile(path, data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void AddText(string path, string text)
        {
            _woWriteFile.AddText(path, text);
        }

        #endregion WirteFile

        #region ReadFile

        private static WoReadFile _woReadFile = new WoReadFile();

        public static string ReadFile(string path)
        {
            try
            {
                return _woReadFile.ReadFile(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion ReadFile

        #region MoveDirectory

        private static WoMoveDirectory _woMoveDirectory = new WoMoveDirectory();

        public static void MoveDirectory(string oldPath, string newPath)
        {
            _woMoveDirectory.MoveDirectory(oldPath, newPath);
        }

        public static void MoveFile(string oldPath, string newPath)
        {
            _woMoveDirectory.MoveFile(oldPath, newPath);
        }

        #endregion MoveDirectory

        #region CopyDirectory

        private static WoCopyDirectory _woCopyDirectory = new WoCopyDirectory();

        public static void CopyFile(string oldPath, string newPath, bool remplace = false)
        {
            _woCopyDirectory.CopyFile(oldPath, newPath, remplace);
        }

        public static void CopyDirectory(string oldPath, string newPath)
        {
            _woCopyDirectory.CopyDirectory(oldPath, newPath);
        }

        #endregion CopyDirectory

        #region RenameDirectory

        private static WoRenameDirectory _woRenameDirectory = new WoRenameDirectory();

        public static void RenameFile(string path, string newName)
        {
            _woRenameDirectory.RenameFile(path, newName);
        }

        public static void RenameDirectory(string path, string newName)
        {
            _woRenameDirectory.RenameDirectory(path, newName);
        }

        #endregion RenameDirectory

        #region ReadDirectory

        private static WoReadDirectory _woReadDirectory = new WoReadDirectory();

        public static List<string> ReadDirectoryFiles(string path, bool onlyNames = false)
        {
            return _woReadDirectory.ReadDirectoryFiles(path, onlyNames);
        }

        public static List<(string path, string name)> ReadDirectoryFilesPathName(
            string path,
            bool create = false
        )
        {
            return _woReadDirectory.ReadDirectoryFilesPathName(path, create);
        }

        public static List<string> ReadDirectoryDirectories(string path, bool onlyNames = false)
        {
            return _woReadDirectory.ReadDirectoryDirectories(path, onlyNames);
        }

        public static List<(string path, string name)> ReadDirectoryDirectoriesPathName(
            string path,
            bool create = false
        )
        {
            return _woReadDirectory.ReadDirectoryDirectoriesPathName(path, create);
        }

        #endregion ReadDirectory
    }
}
