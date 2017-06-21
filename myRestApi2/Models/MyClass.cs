using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myRestApi2.Models
{
	public class MyClass
	{
		public string namE { get; set; }
		public int Id { get; set; }
	}


	public class ForAddCreditCard
	{
		public string userId { get; set; }
		public string token { get; set; }
	}

	public class ForDelCreditCard
	{
		public string userId { get; set; }
		public string omiseCardId { get; set; }
	}

	public class ForListCreditCard
	{
		public string userId { get; set; }
		public string omiseCardId { get; set; }
		public string creditCardName { get; set; }
	}

	public class ForChargeCreditCard
	{
		public int orderNo { get; set; }
		public string userId { get; set; }
		public string omiseCardId { get; set; }
		public double amount { get; set; }
	}
}