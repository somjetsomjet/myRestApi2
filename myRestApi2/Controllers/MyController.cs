using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using myRestApi2.Models;

namespace myRestApi2.Controllers
{
	public class MyFile
	{
		public string fileName { get; set; }
		public string dataString { get; set; }
	}

	public class Result
	{
		public string status { get; set; }
		public string etc { get; set; }
	}

	public class MyController : ApiController
	{

		public MyClass[] Get()
		{
			return new MyClass[]
			{
				new MyClass(){ Id = 1, namE = "na2"},
				new MyClass(){ Id = 2, namE = "na2"}

			};
		}



		//http://localhost:29875/api/my/Get222?a=1&C=8       b optional   c useless
		public MyClass[] Getwww(string a, int b = 2)
		{
			return new MyClass[]
			{
				new MyClass(){ Id = 1, namE = "na2"},
				new MyClass(){ Id = 2, namE = "na2"}

			};
		}

		public MyFile Get123(string fileName)
		{
			return new MyFile() { dataString = Convert.ToBase64String(File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/upload/" + fileName.Replace(":", "")))) };
		}



		public Result Post123(MyFile a)
		{
			try
			{
				File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/upload/" + a.fileName.Replace(":", "")), Convert.FromBase64String(a.dataString));

				return new Result() { status = "ok" };
			}
			catch (Exception ex)
			{
				return new Result() { status = "ng", etc = ex.Message.ToString() };
			}
		}

		public MyClass[] Post456()
		{
			return new MyClass[]
			{
				new MyClass(){ Id = 1, namE = "na3"},
				new MyClass(){ Id = 2, namE = "na3"}

			};
		}




	}
}
