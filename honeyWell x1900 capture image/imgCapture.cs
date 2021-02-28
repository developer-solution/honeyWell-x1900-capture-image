using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace honeyWell_x1900_capture_image
{
    static class imgCapture
    {
        const string hwImgsnpCmd = "IMGSNP1B";//أمر التصوير
        const string hwImgshpCmd = "IMGSHP2P0L843R639B0T0M8D1S6F";//أمر جلب الصورة من القارئ
        public readonly static string hwPictureCmd = string.Format("{0};{1}.", hwImgsnpCmd, hwImgshpCmd);//جمع الأمرين مع بعض
        private readonly static int honeyWellPictureCmdLength = hwPictureCmd.Length; //طول الأمر

        //
        public static Image getImage(string imageStr)
        {
            byte[] imageArray = Encoding.Default.GetBytes(imageStr);
            int startIndex = 0;

            for (int i = 0; i < imageArray.Length; i++)
            {
                if (imageArray[i] == 0x1d) //بدء الصورة من ال Byte الذي قيمته = 
                                           //0x1d
                {
                    startIndex = i;
                    break;
                }
            }
            startIndex++;

            Image img = null;
            try
            {
                //تحويل البيانات المستلمة من القارئ إلى صورة
                MemoryStream ms = new MemoryStream(imageArray, startIndex, imageArray.Length - (startIndex + honeyWellPictureCmdLength + 2));
                img = Image.FromStream(ms);
            }
            catch (Exception)
            {
            }

            return img;
        }
    }
}
