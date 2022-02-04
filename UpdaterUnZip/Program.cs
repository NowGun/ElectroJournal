using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace UpdaterUnZip
{
    internal class Program
    {
        static void Main(string[] args)
        {

            CreateDir();
            Console.WriteLine("Создание временной директории");
            UnPackNewVersion();
            Console.WriteLine("Распаковка обновления");
            MoveFiles();
            Console.WriteLine("Перемещения файлов из временной папки");
            DeleteDir();
            Console.WriteLine("Удаление временной директории");
            Process.Start("ElectroJournal.exe");
            Environment.Exit(0);
        }
        
        static void CreateDir()
        {
            string root = @"newVersion";
            DirectoryInfo directory = new DirectoryInfo(root);
            if (!directory.Exists)
            {
                directory.Create();
            }
            
        }

        static void UnPackNewVersion()
        {
            // путь к архиву
            const string archivePath = @"ElectroJournal.zip";
            // путь к папке
            const string directoryPath = @"newVersion";

            // вызов метода для извлечения файлов из архива
            ZipFile.ExtractToDirectory(archivePath, directoryPath);
            MoveFiles();
        }

        static void MoveFiles()
        {
            FileSystem.CopyDirectory(@"newVersion", Directory.GetCurrentDirectory(), true);
            
        }

        static void DeleteDir()
        {
            string root = @"newVersion";
            try
            {
                DirectoryInfo directory = new DirectoryInfo(root);
                directory.Delete(true);
            }
            catch (Exception ex)
            {

            }
            System.IO.File.Delete(@"ElectroJournal.zip");            
        }
    }
}
