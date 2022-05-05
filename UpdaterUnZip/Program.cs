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

            Console.WriteLine("Создание временной директории");
            CreateDir();
            Console.WriteLine("Распаковка обновления");
            UnPackNewVersion();
            Console.WriteLine("Перемещения файлов из временной папки");
            MoveFiles();
            Console.WriteLine("Удаление временной директории");
            DeleteDir();
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
            else if (directory.Exists)
            {
                directory.Delete(true);
            }
            
        }
        static void UnPackNewVersion()
        {
            // путь к архиву
            const string archivePath = @"ElectroJournal.zip";
            // путь к папке
            const string directoryPath = @"newVersion";

            // вызов метода для извлечения файлов из архива
            try
            {
                ZipFile.ExtractToDirectory(archivePath, directoryPath);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Файла для распаковки нету, попробуйте обновить приложение заново");
                Console.ReadLine();
                Environment.Exit(0);
            }
            
        }
        static void MoveFiles()
        {
            FileSystem.MoveDirectory(@"newVersion", Directory.GetCurrentDirectory(), true);
        }
        static void DeleteDir()
        {
            string root = @"newVersion";
            DirectoryInfo directory = new DirectoryInfo(root);

            if (directory.Exists)
            {
                directory.Delete(true);
            }
            System.IO.File.Delete(@"ElectroJournal.zip");            
        }
    }
}
