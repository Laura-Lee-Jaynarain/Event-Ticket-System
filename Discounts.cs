using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PRG281_Project
{
    public delegate decimal discountDelegate(decimal amount);

    public interface IDiscount
    {

        string Label { get; }
        // property to get the discount label
        decimal Apply(decimal totalAmount);
        // method to apply the discount to a total amount
    }

    public class NoDiscount : IDiscount
    {
        public string Label { get; } = "No Discount";
        public decimal Apply(decimal totalAmount) => totalAmount;
    }

    public class CouponDiscount : IDiscount
    {
        public string Code { get; }
        public decimal Percent { get; }
        public string Label { get; } = "Coupon Discount";

        public CouponDiscount(string code, decimal percent)
        {
            if (percent < 0 || percent > 1) throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 1.");
            Code = code;
            Percent = percent;
        }

        public decimal Apply(decimal totalAmount) => Math.Round(totalAmount * (1 - Percent), 2);
    }

    public class GroupDiscount : IDiscount
    {
        public int Quantity { get; }
        public decimal Percent { get; }
        public string Label { get; } = "Group Discount";

        public GroupDiscount(int quantity)
        {
            Quantity = Math.Max(0, quantity);
            Percent = ComputePercent(quantity);
        }

        public static decimal ComputePercent(int q)
        {
            if (q >= 20) return 0.30m;
            if (q >= 10) return 0.20m;
            if (q >= 5) return 0.10m;
            return 0m;
            
        }

        public decimal Apply(decimal totalAmount)
        {
            return Math.Round(totalAmount * (1 - Percent), 2);
        }
    }

    public class CombinedDiscount : IDiscount
    {
        public string Label { get; } = "Combined Discount";
        public List<IDiscount> Discounts { get; } = new List<IDiscount>();

        public CombinedDiscount(IEnumerable<IDiscount> list)
        {
            // Ensure we have at least one discount option
            if (list == null || !list.Any()) throw new ArgumentException("No discount options provided");
            // Add all discounts to the list
            Discounts.AddRange(list);
        }

        public decimal Apply(decimal totalAmount)
        {
            decimal x = totalAmount;
            foreach (var d in Discounts) x = d.Apply(x);
            return Math.Round(x, 2);
        }
    }

    public static class CouponBook
    {
        private static readonly object _lock = new object();
        private static Dictionary<string, decimal> _codes = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        public static void EnsureDefaultCoupons()
        {
            if (_codes.Count == 0)
            {
                // Initialize with default coupons
                _codes["SAVE10"] = 0.10m;
                _codes["VIP15"] = 0.15m;
            }
        }

        public static bool TryGetPercent(string code, out decimal percent)
        {
            // Ensure default coupons are loaded
            lock (_lock)
                return _codes.TryGetValue(code ?? "", out percent);
        }

        public static List<(string code, decimal percent)> ListCoupons()
        {
            
            lock (_lock)
                return _codes.Select(kv => (kv.Key, kv.Value)).OrderBy(t => t.Key).ToList();
            
        }

        public static void LoadFromFile(string path)
        {
            var full = Path.GetFullPath(path);
            var dir = Path.GetDirectoryName(full);
            if (!Directory.Exists(dir)) throw new DirectoryNotFoundException(dir);

            var tmp = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(full))
            {
                var raw = (line ?? "").Trim();
                if (string.IsNullOrWhiteSpace(raw) || raw.StartsWith("#")) continue;

                var parts = raw.Split(',');
                if (parts.Length != 2) continue;

                var code = parts[0].Trim();
                if (decimal.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var pct)
                    && pct >= 0 && pct <= 1)
                {
                    tmp[code] = pct;
                }
            }
            
            lock (_lock) _codes = tmp;
        }
    }
}
