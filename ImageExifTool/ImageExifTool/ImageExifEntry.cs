using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageExifTool
{
    public static class ImageExifEntry
    {
        public static int ImageExifEdit(string arg)
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
            var fileInfos = directory.GetFiles();
            int seconds = int.Parse(arg);
            var result = ChangePhotoTime(fileInfos, seconds);
            return result;
        }
        private static int ChangePhotoTime(IEnumerable<FileInfo> fileInfos,int seconds)
        {
            int count = 0;
            Image dummyImage = new Bitmap(Environment.CurrentDirectory + "/dummy.jpg");
            var dummyTime = dummyImage.GetPropertyItem(0x0132);
            foreach(var fileInfo in fileInfos)
            {
                var extName = Path.GetExtension(fileInfo.Name).ToUpper();
                if ((extName != ".JPG" && extName != ".JPEG") || fileInfo.Name == "dummy.jpg" || fileInfo.Name.Contains("_Done"))
                {
                    continue;
                }

                Bitmap image = new Bitmap(fileInfo.FullName);
                DateTime photoDate;
                //从文件名中获取时间
                if (seconds == 0)
                {
                    var fNameFrags = fileInfo.Name.Split('_');
                    int year = 2000 + int.Parse(fNameFrags[1].Substring(0, 4));
                    int month = int.Parse(fNameFrags[1].Substring(4, 2));
                    int day = int.Parse(fNameFrags[1].Substring(6, 2));
                    int hour = int.Parse(fNameFrags[2].Substring(0, 2));
                    int minute = int.Parse(fNameFrags[2].Substring(2, 2));
                    int second = int.Parse(fNameFrags[2].Substring(4, 2));
                    photoDate = new DateTime(year, month, day, hour, minute, second);
                    var newTimeString = photoDate.ToString("yyyy:MM:dd HH:mm:ss");
                    newTimeString += "\0";
                    dummyTime.Value = Encoding.ASCII.GetBytes(newTimeString);
                    dummyTime.Len = dummyTime.Value.Length;
                    image.SetPropertyItem(dummyTime);
                }
                else
                {
                    var originTime = image.GetPropertyItem(0x0132);
                    var timeString = Encoding.ASCII.GetString(originTime.Value);
                    timeString = timeString.Replace(" ", ":");
                    var timeFrags = timeString.Split(':');
                    int year = int.Parse(timeFrags[0]);
                    int month = int.Parse(timeFrags[1]);
                    int day = int.Parse(timeFrags[2]);
                    int hour = int.Parse(timeFrags[3]);
                    int minute = int.Parse(timeFrags[4]);
                    int second = int.Parse(timeFrags[5]);
                    photoDate = new DateTime(year, month, day, hour, minute, second);
                    photoDate = photoDate.AddSeconds(seconds);
                    var newTimeString = photoDate.ToString("yyyy:MM:dd HH:mm:ss");
                    newTimeString += "\0";
                    originTime.Value = Encoding.ASCII.GetBytes(newTimeString);
                    originTime.Len = originTime.Value.Length;
                    image.SetPropertyItem(originTime);
                }
                EncoderParameters encoderParameters = new EncoderParameters(1);
                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                encoderParameters.Param[0] = new EncoderParameter(myEncoder, 100L);
                var myImageCodecInfo = GetEncoderInfo("image/jpeg");
                var newFileName = Path.GetFileNameWithoutExtension(fileInfo.Name) + "_Done" + extName;
                image.Save(newFileName, myImageCodecInfo, encoderParameters);
                image.Dispose();
                count++;
            }

            return count;
        }
        public static ImageCodecInfo GetEncoderInfo(String mimeType)

        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
