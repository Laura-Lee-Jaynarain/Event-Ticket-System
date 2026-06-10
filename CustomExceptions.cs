using System;

namespace PRG281_Project
{
    public class InvalidCouponException : Exception
    {
        public InvalidCouponException(string code)
            : base($"Coupon '{code}' is invalid or expired.") { }
    }
}
