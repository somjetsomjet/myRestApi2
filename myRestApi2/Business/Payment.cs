using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Omise;
using Omise.Models;

namespace myRestApi2.Business
{
	public class Payment
	{
		private Client client { get; set; }

		public Payment()
		{
			try
			{
				client = new Client(pkey: "pkey_test_589yy3jk6y6hp498vvl", skey: "skey_test_589yy3jkgojk1cev053");
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<string> GetOmiseCustId(string userId)
		{
			try
			{
				var customers = await client.Customers.GetList();
				var customer = customers.FirstOrDefault(x => x.Description == userId);
				return customer == null ? "" : customer.Id;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<Customer> CreateOmiseCustAndCard(string userId, string token)
		{
			try
			{
				var request = new CreateCustomerRequest
				{
					Description = userId,
					Card = token,
				};

				var customer = await client.Customers.Create(request);
				return customer;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<Customer> AddOmiseCard(string omiseCustId, string token)
		{
			try
			{
				var request = new UpdateCustomerRequest
				{
					Card = token,
				};

				var customer = await client.Customers.Update(omiseCustId, request);
				return customer;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task DeleteOmiseCard(string omiseCustId, string omiseCardId)
		{
			try
			{
				await client.Customer(omiseCustId).Cards.Destroy(omiseCardId);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<ScopedList<Card>> ListOmiseCard(string omiseCustId)
		{
			try
			{
				var cards = await client.Customer(omiseCustId).Cards.GetList();
				return cards;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<Charge> ChargeCustomer(string omiseCustId, string omiseCardId, double amount, string urlComplete = null)
		{
			try
			{
				var request = new CreateChargeRequest
				{
					Amount = (long)(amount*100),
					Currency = "THB",
					Customer = omiseCustId,
					Card = omiseCardId,
					ReturnUri = urlComplete
				};

				var charge = await client.Charges.Create(request);
				return charge;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task Refund(string omiseChargeId, double amount)
		{
			try
			{
				var request = new CreateRefundRequest
				{
					Amount = (long)(amount * 100)
				};

				await client.Charge(omiseChargeId).Refunds.Create(request);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task DeleteCard(string omiseCustId, string omiseCardId)
		{
			try
			{
				await client.Customer(omiseCustId).Cards.Destroy(omiseCardId);
			}
			catch (Exception)
			{
				throw;
			}
		}


		private async Task<Token> GetToken(string tokenId)
		{
			try
			{
				System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				var token = await client.Tokens.Get(tokenId);
				return token;
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task AddCreditCard(string userId, string tokenId)
		{
			try
			{
				var omiseCustId = await GetOmiseCustId(userId);
				var token = await GetToken(tokenId);
				var omiseCardId = token.Card.Id;

				if (String.IsNullOrEmpty(omiseCustId))
				{
					var customer = await CreateOmiseCustAndCard(userId, tokenId);
					omiseCustId = customer.Id;
				}
				else
				{
					await AddOmiseCard(omiseCustId, tokenId);
				}

				////Debit card
				//if (token.Card.SecurityCodeCheck == false)
				//{
					
					//charge 20
					var charge = await ChargeCustomer(omiseCustId, omiseCardId, 20);

					if (charge.FailureCode == null)
					{
						//refund 20
						await Refund(charge.Id, 20);
					}
					else
					{
						//delete card
						await DeleteOmiseCard(omiseCustId, omiseCardId);
						throw new Exception(charge.FailureCode);
					}

				//}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<List<Card>> GetCreditCardList(string userId)
		{
			try
			{
				var omiseCustId = await GetOmiseCustId(userId);
				var list = await ListOmiseCard(omiseCustId);
				return list.ToList();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task ChargeCreditCard(string userId, string omiseCardId, double amount)
		{
			try
			{
				var omiseCustId = await GetOmiseCustId(userId);
				var charge = await ChargeCustomer(omiseCustId, omiseCardId, amount);

				//charge fail
				if (charge.FailureCode != null)
					throw new Exception(charge.FailureCode);
				
				//charge complete
				var tranId = charge.Transaction;	//https://dashboard.omise.co/test/dashboard
				var chargeId = charge.Id;			//https://dashboard.omise.co/test/charges
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<string> ChargeCreditCard3ds(string userId, string omiseCardId, double amount, int orderNo)
		{
			try
			{
				var omiseCustId = await GetOmiseCustId(userId);
				var charge = await ChargeCustomer(omiseCustId, omiseCardId, amount, "http://192.168.0.30:31956/api/Payment/GetChargeCreditCard3dsComplete?orderNo=" + orderNo.ToString());

				//charge fail
				if (charge.FailureCode != null)
					throw new Exception(charge.FailureCode);


				//save db orderNo, chargeId
				using (var conn = new SqlConnection(@"Data Source=CNX-NBTON\MSSQL2014; 
							  Initial Catalog=Messanger;
							  User id=sa;
							  Password=Connex@123;"))
					
				using (var command = new SqlCommand("usp_in_up_payment", conn) { CommandType = CommandType.StoredProcedure })
				{
					command.Parameters.AddWithValue("@order_no", orderNo);
					command.Parameters.AddWithValue("@charge_id", charge.Id);
					command.Parameters.AddWithValue("@paid", 0);
					command.Parameters.AddWithValue("@mod_datetime", DateTime.Now);
					conn.Open();
					command.ExecuteNonQuery();
				}

				return charge.AuthorizeURI;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<string> ChargeCreditCard3dsCheck(int orderNo)
		{
			try
			{
				string chargeId = "";

				//get db chargeId
				using (var conn = new SqlConnection(@"Data Source=CNX-NBTON\MSSQL2014; 
							  Initial Catalog=Messanger;
							  User id=sa;
							  Password=Connex@123;"))

				using (var command = new SqlCommand("usp_get_charge_id", conn) { CommandType = CommandType.StoredProcedure })
				{
					command.Parameters.AddWithValue("@order_no", orderNo);
					conn.Open();
					chargeId = (string)command.ExecuteScalar();
				}

				var charge = await client.Charges.Get(chargeId);

				if (charge.Paid == true)
				{
					//save db orderNo, chargeId
					using (var conn = new SqlConnection(@"Data Source=CNX-NBTON\MSSQL2014; 
							  Initial Catalog=Messanger;
							  User id=sa;
							  Password=Connex@123;"))

					using (var command = new SqlCommand("usp_in_up_payment", conn) { CommandType = CommandType.StoredProcedure })
					{
						command.Parameters.AddWithValue("@order_no", orderNo);
						command.Parameters.AddWithValue("@charge_id", charge.Id);
						command.Parameters.AddWithValue("@paid", 1);
						command.Parameters.AddWithValue("@mod_datetime", DateTime.Now);
						conn.Open();
						command.ExecuteNonQuery();
					}
				}
				else {
					throw new Exception(charge.FailureCode);
				}

				return "ok";
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task DelCreditCard(string userId, string omiseCardId)
		{
			try
			{
				var omiseCustId = await GetOmiseCustId(userId);
				await DeleteCard(omiseCustId, omiseCardId);
			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}