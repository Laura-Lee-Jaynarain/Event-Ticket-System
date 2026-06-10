using System;

namespace PRG281_Project
{
    internal class Payment
    {
        public decimal Amount { get; }
        // enumeration value
        public PaymentMethod Method { get; }
        public DateTime PaidAt { get; } = DateTime.Now;

        public Payment(decimal amount, PaymentMethod method)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            Amount = amount;
            Method = method;
        }

        public void LogToConsole()
        {
            // display Date and Time with Custom Date format
            Console.WriteLine($"Payment: R{Amount:0.00} via {Method} at {PaidAt:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
