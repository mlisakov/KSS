using System.IO;
using System.Web;
using HomeProjectWebForms.Consts;

namespace HomeProjectWebForms.Helpers
{
    public static class ImageHelpers
    {
        private static MemoryStream _defaultMaleStream;
        private static MemoryStream _defaultFemaleStream;

        public static MemoryStream MaleImage
        {
            get { return _defaultMaleStream; }
        }

        public static MemoryStream FemaleImage
        {
            get { return _defaultFemaleStream; }
        }


        public static void InitImageHelpers(HttpServerUtility server)
        {
            string path = server.MapPath("~/Images/");

            var fileStream = new FileStream(path + UIConsts.DefaultMaleImage, FileMode.Open);
            var memStream = new MemoryStream();
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int) fileStream.Length);
            fileStream.Close();
            _defaultMaleStream = memStream;

            fileStream = new FileStream(path + UIConsts.DefaultFemaleImage, FileMode.Open);
            memStream = new MemoryStream();
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int) fileStream.Length);
            fileStream.Close();
            _defaultFemaleStream = memStream;
        }
    }
}