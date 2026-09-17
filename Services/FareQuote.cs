namespace CabManagementSystem.Services
{
    public class FareQuote
    {
        public decimal BaseFare { get; init; }
        public decimal Surcharge { get; init; }
        public int? PricingRuleId { get; init; }
        public string? SurchargeLabel { get; init; }
        public decimal Total => BaseFare + Surcharge;
        public bool HasSurcharge => Surcharge > 0;
    }
}
