using System;
using System.IO;
using System.Net;

namespace DMS_3
{
	class Data
	{	

		//Instance
		private static Data instance;
		//DATA User
		public static string userAndsoft;
		public static string userTransics;

		//Log DATA
		public static string log_file;

		//GPS
		public static string GPS;

		public static Data Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Data();
				}
				return instance;
			}
		}

		public bool  UploadFile(string FtpUrl, string fileName, string userName, string password,string UploadDirectory="")
		{
			try{
				string PureFileName = new FileInfo(fileName).Name;
				String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl,UploadDirectory,PureFileName);
				FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
				req.Proxy = null;
				req.Method = WebRequestMethods.Ftp.UploadFile;
				req.Credentials = new NetworkCredential(userName,password);
				req.UseBinary = true;
				req.UsePassive = true;
				byte[] data = System.IO.File.ReadAllBytes(fileName);
				req.ContentLength = data.Length;
				System.IO.Stream stream = req.GetRequestStream();
				stream.Write(data, 0, data.Length);
				stream.Close();
				FtpWebResponse res = (FtpWebResponse)req.GetResponse();
				File.AppendAllText(Data.log_file,"Upload file"+fileName+" good\n");
				Console.Out.Write("Upload file"+fileName+" good\n");
				return true;

			} catch (Exception ex) {
				File.AppendAllText(Data.log_file,"Upload file"+fileName+" error :"+ex+"\n");
				Console.Out.Write("Upload file"+fileName+" error\n");
				return false;
			}
		}



	}
}

