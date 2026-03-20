using JewelryStore.Infra;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JewelryStore.Areas.Admin.Models
{
	public class Offer : EntitiesBase
	{
		public string OfferName { get; set; }
		public string OfferType { get; set; }
		public string DiscountType { get; set; }

		public decimal? DiscountValue { get; set; }
		public decimal? MaxDiscountAmount { get; set; }

		public decimal? MinOrderAmount { get; set; }
		public int? MinQty { get; set; }

		public bool IsCouponRequired { get; set; }
		public string CouponCode { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public TimeSpan? StartTime { get; set; }
		public TimeSpan? EndTime { get; set; }

		public int? MaxUsage { get; set; }
		public int? PerUserUsage { get; set; }

		public bool IsFirstOrderOnly { get; set; }
		public bool IsPrimeOnly { get; set; }

		public bool AllowWithOtherOffers { get; set; }
		public int Priority { get; set; }

		public bool IsActive { get; set; }

		public DateTime? CreatedDate { get; set; }
		public DateTime? LastModifiedDate { get; set; }

		/* 🔥 UI / Derived Fields */
		public string Applicability { get; set; }
		public string ApplyOnly_Category { get; set; }
		public string ApplyOnly_Product { get; set; }
		public int UsedCount { get; set; }

		/* 🔥 CHILD COLLECTIONS */
		public List<OfferApplicability> OfferApplicability { get; set; }
		public List<OfferCondition> OfferConditions { get; set; }
		public List<OfferBenefit> OfferBenefits { get; set; }
	}

	public class OfferApplicability
	{
		public long Id { get; set; }
		public long OfferId { get; set; }

		public long? ProductId { get; set; }
		public long? VariantId { get; set; }
		public long? CategoryId { get; set; }

		/* Optional Display */
		public string ProductName { get; set; }
		public string CategoryName { get; set; }
	}

	public class OfferCondition
	{
		public long Id { get; set; }
		public long OfferId { get; set; }

		public string ConditionType { get; set; }
		public string ConditionValue { get; set; }

	}

	public class OfferBenefit
	{
		public long Id { get; set; }
		public long OfferId { get; set; }

		public string BenefitType { get; set; }

		public decimal? Value { get; set; }

		public long? FreeProductId { get; set; }
		public int? FreeQty { get; set; }

	}

	public class OfferUsage
	{
		public long Id { get; set; }
		public long OfferId { get; set; }

		public long UserId { get; set; }
		public long OrderId { get; set; }

		public DateTime UsedDate { get; set; }
	}

	public class OffersViewModel
	{
		public Offer Offer { get; set; }

		public string ApplicabilityData { get; set; }
		public string ConditionData { get; set; }
		public string BenefitData { get; set; }
	}

	public static class OfferHelper
	{
		public static string BuildApplicability(List<OfferApplicability> list)
		{
			return string.Join(",", list.Select(x =>
				$"{x.ProductId ?? 0}~{x.VariantId ?? 0}~{x.CategoryId ?? 0}"
			));
		}

		public static string BuildConditions(List<OfferCondition> list)
		{
			return string.Join(",", list.Select(x =>
				$"{x.ConditionType}~{x.ConditionValue}"
			));
		}

		public static string BuildBenefits(List<OfferBenefit> list)
		{
			return string.Join(",", list.Select(x =>
				$"{x.BenefitType}~{x.Value ?? 0}~{x.FreeProductId ?? 0}~{x.FreeQty ?? 0}"
			));
		}
	}


	public enum OfferTypeEnum
	{
		PRICE_DISCOUNT,
		CART_DISCOUNT,
		QTY_BASED,
		CASHBACK,
		FREE_SHIPPING
	}

	public enum OfferConditionTypeEnum
	{
		CATEGORY,
		PRODUCT,
		CART_VALUE,
		CUSTOMER_TYPE,
		DAY_OF_WEEK,
		FIRST_ORDER,
		BIRTHDAY
	}

	public enum OfferBenefitTypeEnum
	{
		DISCOUNT,
		CASHBACK,
		FREE_PRODUCT,
		FREE_SHIPPING
	}

	public enum DiscountTypeEnum
	{
		PERCENT,
		FLAT
	}

	public enum CustomerTypeEnum
	{
		REGULAR,
		PRIME,
		NEW
	}

	public enum DayOfWeekEnum
	{
		MONDAY,
		TUESDAY,
		WEDNESDAY,
		THURSDAY,
		FRIDAY,
		SATURDAY,
		SUNDAY
	}

	public static class EnumHelper
	{
		public static List<SelectListItem_Custom> ToSelectList<T>(string selectedGroup = null) where T : Enum
		{
			return Enum.GetValues(typeof(T))
				.Cast<T>()
				.Select(e => new SelectListItem_Custom(e.ToString(), FormatText(e.ToString()), selectedGroup)).ToList();
		}

		private static string FormatText(string text)
		{
			return text.Replace("_", " ");
		}
	}
}
