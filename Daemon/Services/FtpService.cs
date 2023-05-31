using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Services
{
    public class FtpService
    {
        public void GetFtpProps()
        {

        }


        public async void ConnectToFtp(FtpConfig ftpConfig, Source source)
        {
            // FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpConfig.FilePath);

            //request.Credentials = new NetworkCredential(ftpConfig.UserName, ftpConfig.Password);
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            //using (var fileStream = File.Open("filePath", FileMode.Open))
            //{
            //    using (var rs = request.GetRequestStream())
            //    {
            //        await fileStream.CopyToAsync(rs);
            //    }
            //}
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(ftpConfig.UserName, ftpConfig.Password);
            client.UploadFile(
                $"ftp://{ftpConfig.FilePath}", source.Path);

        }


    }
}
