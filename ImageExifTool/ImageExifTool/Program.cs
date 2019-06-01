using System;

namespace ImageExifTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入照片时间调整的秒数。如果需要根据文件名生成拍摄时间，输入0");
            var arg = Console.ReadLine();
            var beginTime = DateTime.Now;
            var result = ImageExifEntry.ImageExifEdit(arg);
            var endTime = DateTime.Now;
            var timeDiff = endTime - beginTime;
            Console.WriteLine(string.Format("共处理文件{0}个，耗时{1}秒", result, timeDiff.TotalSeconds));
            Console.ReadKey();
        }
    }
}
