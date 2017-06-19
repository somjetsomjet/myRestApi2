using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;
using myRestApi2.Models;

using myRestApi2.Business;
using System.Web.Mvc;
using System.Web.Helpers;

namespace myRestApi2.Controllers
{
    public class PaymentController : ApiController
    {
		public async Task<Object> PostAddCreditCard(ForAddCreditCard forAddCreditCard)
		{
			try
			{
				var payment = new Payment();
				await payment.AddCreditCard(forAddCreditCard.userId, forAddCreditCard.token);
				return new { status = "ok" };
			}
			catch (Exception ex)
			{
				return new { status = "ng", msg = ex.Message };
			}
		}

		public async Task<Object> PostDelCreditCard(ForDelCreditCard forDelCreditCard)
		{
			try
			{
				var payment = new Payment();
				await payment.DelCreditCard(forDelCreditCard.userId, forDelCreditCard.omiseCardId);
				return new { status = "ok" };
			}
			catch (Exception ex)
			{
				return new { status = "ng", msg = ex.Message };
			}
		}

		public async Task<Object> PostGetCreditCardList(ForListCreditCard forListCreditCard)
		{
			try
			{
				var payment = new Payment();
				var list = await payment.GetCreditCardList(forListCreditCard.userId);
				return new { status = "ok", list = list.Select(x => new ForListCreditCard() { omiseCardId = x.Id, creditCardName = x.Name })};
			}
			catch (Exception ex)
			{
				return new { status = "ng", msg = ex.Message };
			}
		}

		public async Task<Object> PostChargeCreditCard(ForChargeCreditCard forChargeCreditCard)
		{
			try
			{
				var payment = new Payment();
				await payment.ChargeCreditCard(forChargeCreditCard.userId, forChargeCreditCard.omiseCardId, forChargeCreditCard.amount);
				return new { status = "ok" };
			}
			catch (Exception ex)
			{
				return new { status = "ng", msg = ex.Message };
			}
		}
	}
}
