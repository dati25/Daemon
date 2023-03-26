using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Daemon.Backup.Services
{
    public class FilesService
    {
        private StreamWriter sr;
        
        public void Copy(string sourcePath, string destPath)
        {
			this.sr = new StreamWriter(destPath + @"\.snapshot\snapshot.txt");
			string name = Path.GetFileName(sourcePath);
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, String.Join(@"\", destPath, name), true);
                WriteToSnapshot(sourcePath, destPath);
                return;
            }
            this.CopyDir(sourcePath, destPath, name);
            sr.Close();
        }

        private void WriteToSnapshot(string sourcePath, string destPath)
        {
			FileInfo fi = new FileInfo(sourcePath);
			sr.WriteLine(sourcePath + '|' + fi.LastWriteTime);
		}

		private void CopyDir(string fullPath, string destPath, string name)
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] fils = dir.GetFiles();
            MkFol(destPath, name);
            foreach (FileInfo filItem in fils)
            {
                File.Copy(filItem.FullName, String.Join(@"\", destPath, name, filItem.Name));
                WriteToSnapshot(fullPath, destPath);
            }
            foreach (DirectoryInfo dirItem in directories)
            {
                CopyDir(dirItem.FullName, String.Join(@"\", destPath, name), dirItem.Name);
            }
        }
        private void MkDir(string path, string fileName, bool createType)
        {
            if (createType)
            {
                MkFol(path, fileName);
                return;
            }
            MkFil(path, fileName);
        }
        private void MkFol(string path, string fileName)
        {
            DirectoryInfo dir = new DirectoryInfo(String.Join(@"\", path, fileName));
            dir.Create();
        }
        private void MkFil(string path, string fileName)
        {
            FileInfo fil = new FileInfo(String.Join(@"\", path, fileName));
            fil.Create().Close();
        }
        public void Del(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return;
            }
            Directory.Delete(fullPath, true);
        }
        private void DelDir(string fullPath)
        {
            Directory.Delete(fullPath, true);
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            DirectoryInfo[] directories = dir.GetDirectories();
            FileInfo[] fils = dir.GetFiles();

            foreach (FileInfo filItem in fils)
            {
                File.Delete(filItem.FullName);
            }
            foreach (DirectoryInfo dirItem in directories)
            {
                DelDir(dirItem.FullName);
            }
            File.Delete(fullPath);
        }

    }
}
