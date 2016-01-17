using EmpikTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EmpikTest
{
    public static class DirectoryUtils
    {
        const long Mb1 = 1024 * 1024;
        public static async Task<IEnumerable<ModelViewDirectoryInfo>>  BrowsePath(string path)
        {
            return await Task.Run<IEnumerable<ModelViewDirectoryInfo>>(() =>
             {
                 List<ModelViewDirectoryInfo> list = new List<ModelViewDirectoryInfo>();
                 if (path == "\\")
                 {
                     list.AddRange(from dir in DriveInfo.GetDrives() select new ModelViewDirectoryInfo() { Path = dir.RootDirectory.Name, Title = dir.RootDirectory.Name });
                 }
                 else
                 {
                     try {
                         DirectoryInfo parentInfo = Directory.GetParent(path);
                         if (parentInfo != null)
                             list.Add(new ModelViewDirectoryInfo() { Title = "...", Path = parentInfo.FullName });
                         else
                             list.Add(new ModelViewDirectoryInfo() { Title = "...", Path = "\\" });
                         list.AddRange(from dir in Directory.GetDirectories(path) select new ModelViewDirectoryInfo() { Path = dir, Title = Path.GetFileName(dir) });
                         list.AddRange(from dir in Directory.GetFiles(path) select new ModelViewDirectoryInfo() { Path = dir, Title = Path.GetFileName(dir), IsFile = true });
                     } catch (UnauthorizedAccessException e)
                     {
                         throw new UnauthorizedAccessException("Ошибка доступа к каталогу");
                     }
                    }
                 return list;
             });
        }
        
        public static async Task<long[]> CountFiles(string path)
        {
            return await Task.Run<long[]>(() =>
            {
                long[] amounts = new long[3];

                DirectoryInfo currentDir = new DirectoryInfo(path);

                var task = Task.Run(() =>
                {
                    WalkDirectory(currentDir, amounts);
                });
                try {
                    if (!task.Wait(5000))// Ждем подсчета только 5 секунд
                        throw new TaskCanceledException("Время подсчета файлов 5 секунд исчерпано");
                        
                } catch (AggregateException e)
                {
                    foreach (var ex in e.InnerExceptions)
                        throw ex;
                } catch(TaskCanceledException e)
                {
                    //throw e;
                }
                return amounts;
            });
        }
        private static void WalkDirectory(DirectoryInfo root, long[] counts)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            try
            {
                files = root.GetFiles("*.*");

                if (files != null)
                {
                    foreach (FileInfo file in files)
                    {
                        if (file.Length <= 10 * Mb1)
                            counts[0]++;
                        if (file.Length > 10 * Mb1 && file.Length <= 50 * Mb1)
                            counts[1]++;
                        if (file.Length >= 100 * Mb1)
                            counts[2]++;
                    }
                    subDirs = root.GetDirectories();
                    foreach (DirectoryInfo dirInfo in subDirs)
                    {
                        WalkDirectory(dirInfo, counts);
                    }
                }
            }
            catch (IOException e)
            {
                #if !DEBUG
                throw new IOException("Возникла ошибка ввода/вывода при подсчете");
                #endif
            }
            catch (UnauthorizedAccessException e)
            {
                #if !DEBUG
                throw new IOException("Ошибка доступа при подсчете");
                #endif
            }
        }
    }
}