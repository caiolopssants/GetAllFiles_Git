using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
namespace GetAllFiles
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Insert the directory path: ");
            string path = Console.ReadLine();

            Console.WriteLine("If you wanna include file or directory acess denied notification, press 'y', otherwise press any key:");
            bool includeAcessDenied = Console.ReadKey().Key == ConsoleKey.Y;

            List<string> files = GetFilesFromDirectory(path, includeAcessDenied);

            Console.WriteLine("\nAnalisy Process Finished!!");

            OpenFileWithContents(files);
            files.Clear();
                                 
            //Console.WriteLine("\nDo you wanna save this information into a file? Press 'Y', otherwise press any key:");
            //bool saveFile = Console.ReadKey().Key == ConsoleKey.Y;
            //if (saveFile)
            //{
            //    using (SaveFileDialog sFD = new SaveFileDialog() { Title = "Selecet file address", Filter = "Text File|*.txt" })
            //    {
            //        if(sFD.ShowDialog() == DialogResult.OK)
            //        {
            //            File.WriteAllLines(sFD.FileName, files);
            //        }
            //    }
            //}

            Console.ReadKey();
        }

        private static List<string> GetFilesFromDirectory(string directoryPath, bool includeAcessDenied)
        {
            List<string> directories = Directory.GetDirectories(directoryPath).ToList();
            List<string> files = Directory.GetFiles(directoryPath).ToList();

            while(directories.Count>0)
            {
                try
                {
                    files.AddRange(Directory.GetFiles(directories[0]).ToList());
                }
                catch(Exception error)
                {
                    if (includeAcessDenied && directories.Count > 0)
                    {
                        files.Add($"Files Acess Denied/ Directory: {directories[0]}/ Error: {error.Message}");
                    }
                }


                try
                {
                    directories.AddRange(Directory.GetDirectories(directories[0]).ToList());
                }
                catch (Exception error)
                {
                    if (includeAcessDenied && directories.Count > 0)
                    {
                        files.Add($"Directories Acess Denied/ Directory: {directories[0]}/ Error: {error.Message}");
                    }
                }
                finally
                {
                    if (directories.Count > 0)
                    {
                        directories.RemoveAt(0);
                    }
                }
            }
            return files;
        }
        private static void OpenFileWithContents(List<string> contents)
        {
            Guid fileName = Guid.NewGuid();
            File.WriteAllLines($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{fileName}.txt", contents);
            while (!File.Exists($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{fileName}.txt")) { }
            Process.Start($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{fileName}.txt");
            while (Process.GetProcesses().Where(prc => { try { return prc.MainWindowTitle.ToLower().Contains($@"{fileName}.txt"); } catch { return false; } }).Count() == 0) { }
            File.Delete($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{fileName}.txt");
        }
    }
}
