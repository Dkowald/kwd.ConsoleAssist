using System.IO;

namespace kwd.ConsoleAssist.Demo.App
{
    public static class DirectoryInfoExtensions
    {
        public static long Size(this DirectoryInfo dir)
        {
            //Credit:
            // https://stackoverflow.com/users/370307/alexandre-pepin
            //https://stackoverflow.com/questions/468119/whats-the-best-way-to-calculate-the-size-of-a-directory-in-net

            long size = 0;    
            // Add file sizes.
            FileInfo[] fis = dir.GetFiles();
            foreach (FileInfo fi in fis) 
            {      
                size += fi.Length;    
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = dir.GetDirectories();
            foreach (DirectoryInfo di in dis) 
            {
                size += di.Size();   
            }
            return size;  
        }
    }
}