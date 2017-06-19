using System;
using System.Collections.Generic;
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
				client = new Client(skey: "skey_test_589yy3jkgojk1cev053");
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

		private async Task<Charge> ChargeCustomer(string omiseCustId, string omiseCardId, double amount)
		{
			try
			{
				var request = new CreateChargeRequest
				{
					Amount = (long)(amount*100),
					Currency = "THB",
					Customer = omiseCustId,
					Card = omiseCardId
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

				//Debit card
				if (token.Card.SecurityCodeCheck == false)
				{
					try
					{
						//charge 20
						var charge = await ChargeCustomer(omiseCustId, omiseCardId, 20);
						//refund 20
						await Refund(charge.Id, 20);
					}
					catch (Exception)
					{
						//delete card
						await DeleteOmiseCard(omiseCustId, omiseCardId);

						throw;
					}
				}
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

				var tranId = charge.Transaction;	//https://dashboard.omise.co/test/dashboard
				var chargeId = charge.Id;			//https://dashboard.omise.co/test/charges
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