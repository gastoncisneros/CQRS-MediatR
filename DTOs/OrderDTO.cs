public record OrderDTO(
    int Id,
    string FirstName,
    string LastName,
    string Status,
    DateTime CreatedAt,
    Decimal TotalCost
);
