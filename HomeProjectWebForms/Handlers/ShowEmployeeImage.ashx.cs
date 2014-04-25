using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using HomeProjectWebForms.Consts;
using HomeProjectWebForms.Helpers;

namespace HomeProjectWebForms.Handlers
{
    /// <summary>
    ///     Summary description for Handler1
    /// </summary>
    public class ShowEmployeeImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Guid personGuid;
            if (context.Request.QueryString["id"] != null)
                personGuid = new Guid(context.Request.QueryString["Id"]);
            else
                throw new ArgumentException("No parameter specified");

            context.Response.ContentType = "image/jpeg";
            string path = context.Server.MapPath("~/Images/");
            MemoryStream strm = ShowAlbumImage(personGuid, path);
            if (strm == null)
                return;
            var buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public MemoryStream ShowAlbumImage(Guid personGuid, string path)
        {
            SqlConnection connection = DBHelper.GetConnection();
            string commandText = "SELECT Photo FROM Employee WHERE Id = '" + personGuid + "'";
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.Text};
            try
            {
                connection.Open();
                object img = cmd.ExecuteScalar();
                if (img != null && !string.IsNullOrEmpty(img.ToString()))
                    return new MemoryStream((byte[]) img);
                else
                    return GetDefaultImage(personGuid, path);
            }
            catch
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public MemoryStream GetDefaultImage(Guid personGuid, string path)
        {
            SqlConnection connection = DBHelper.GetConnection();
            string commandText = "SELECT Sex FROM Employee WHERE Id = '" + personGuid + "'";
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.Text};
            try
            {
                connection.Open();
                string sex = cmd.ExecuteScalar().ToString();
                if (sex == UIConsts.DBFemaleValueText)
                    return new MemoryStream(ImageHelpers.FemaleImage.ToArray());
                return new MemoryStream(ImageHelpers.MaleImage.ToArray());
                //path += sex == UIConsts.DBFemaleValueText ? UIConsts.DefaultFemaleImage : UIConsts.DefaultMaleImage;
                //FileStream fileStream = new FileStream(path, FileMode.Open);
                //MemoryStream memStream = new MemoryStream();
                //memStream.SetLength(fileStream.Length);
                //fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                //fileStream.Close();
                //return memStream;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}