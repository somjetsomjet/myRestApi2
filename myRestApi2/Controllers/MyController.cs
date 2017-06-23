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

		public  Object PostGetContain(LatLng location)
		{
			try
			{
				var _vertices = new List<LatLng>();
				_vertices.Add(new LatLng() { Latitude = 13.696792, Longitude = 100.495216 });
				_vertices.Add(new LatLng() { Latitude = 13.694749, Longitude = 100.497319 });
				_vertices.Add(new LatLng() { Latitude = 13.692831, Longitude = 100.495559 });
				_vertices.Add(new LatLng() { Latitude = 13.691392, Longitude = 100.500237 });
				_vertices.Add(new LatLng() { Latitude = 13.693039, Longitude = 100.505773 });
				_vertices.Add(new LatLng() { Latitude = 13.697501, Longitude = 100.505880 });
				_vertices.Add(new LatLng() { Latitude = 13.696729, Longitude = 100.502769 });
				_vertices.Add(new LatLng() { Latitude = 13.694061, Longitude = 100.503670 });
				_vertices.Add(new LatLng() { Latitude = 13.693998, Longitude = 100.499915 });
				_vertices.Add(new LatLng() { Latitude = 13.699752, Longitude = 100.501288 });
				_vertices.Add(new LatLng() { Latitude = 13.696792, Longitude = 100.495216 });



				var lastPoint = _vertices[_vertices.Count - 1];
				var isInside = false;
				var x = location.Longitude;
				foreach (var point in _vertices)
				{
					var x1 = lastPoint.Longitude;
					var x2 = point.Longitude;
					var dx = x2 - x1;

					if (Math.Abs(dx) > 180.0)
					{
						// we have, most likely, just jumped the dateline (could do further validation to this effect if needed).  normalise the numbers.
						if (x > 0)
						{
							while (x1 < 0)
								x1 += 360;
							while (x2 < 0)
								x2 += 360;
						}
						else
						{
							while (x1 > 0)
								x1 -= 360;
							while (x2 > 0)
								x2 -= 360;
						}
						dx = x2 - x1;
					}

					if ((x1 <= x && x2 > x) || (x1 >= x && x2 < x))
					{
						var grad = (point.Latitude - lastPoint.Latitude) / dx;
						var intersectAtLat = lastPoint.Latitude + ((x - x1) * grad);

						if (intersectAtLat > location.Latitude)
							isInside = !isInside;
					}
					lastPoint = point;
				}

				return new { status = isInside };
			}
			catch (Exception ex)
			{
				return new { status = "ng", msg = ex.Message };
			}
		}


	}
}
