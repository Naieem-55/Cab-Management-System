namespace CabManagementSystem.Services;

public class PromoValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public int? PromoCodeId { get; set; }
    public string Code { get; set; } = string.Empty;

    public static PromoValidationResult Fail(string message)
        => new() { IsValid = false, Message = message };
}