using System;

namespace LibraryManagement.Application.Features.Penalties;

public static class PenaltyCalculator
{
    public const decimal FinePerDay = 5_000m;

    public static decimal Calculate(
        DateTime dueDate,
        DateTime returnDate)
    {
        if (returnDate <= dueDate)
            return 0m;

        var overdueDays = (int)Math.Ceiling(
            (returnDate - dueDate).TotalDays);

        return overdueDays * FinePerDay;
    }
}
