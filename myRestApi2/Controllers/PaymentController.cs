using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;
using myRestApi2.Models;
using Omise;
using Omise.Models;

namespace myRestApi2.Controllers
{
    public class PaymentController : ApiController
    {
		private Client client { get; set; }

		public void init() {
			client = new Client(skey: "skey_test_589yy3jkgojk1cev053");
		}

		public void GetCreateCustomer(string token)
		{
			init();
		}

		public string PostCreateCustomer(MyClass token)
		{
			try
			{

				init();
				//new PageAsyncTask(() => CreateCustomer(token.namE));

				var task = CreateCustomer(token.namE);
				task.Wait();

				
			}
			catch (Exception ex)
			{
				var a = ex;
			}
			return "ok";
		}


		private async Task CreateCustomer(string token)
		{

			try
			{

				var request = new CreateCustomerRequest
				{
					Description = "test des",
					Card = token,
				};

				var customer = await client.Customers.Create(request);

				var a = request.Description;
				var b = customer.Description;


				new PageAsyncTask(() => ChargeCustomer(customer.Id));

				var task = ChargeCustomer(customer.Id);
				task.Wait();

			}
			catch (Exception ex) {
				var a = ex;
			}
		}

		private async Task ChargeCustomer(string cust)
		{

			var request = new CreateChargeRequest
		{
			Amount = 100000, // THB 1,000.00
			Currency = "THB",
			Customer = cust,
		};

			var charge = await client.Charges.Create(request);
			var a = 100000;
			var b = charge.Amount;
		}

	}
}
