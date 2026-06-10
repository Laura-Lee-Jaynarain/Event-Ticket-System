using System;

namespace PRG281_Project
{
    internal abstract class Tickets
    {
        private static int _nextId = 1;

        public static void SetNextId(int next)
        {
            _nextId = Math.Max(next, 1);
        }

        public string TicketID { get; }
        public TicketType Type { get; protected set; }
        public decimal BasePrice { get; protected set; }

        //constructor
        protected Tickets(TicketType type, decimal basePrice)
        {
            Type = type;
            BasePrice = basePrice;
            TicketID = $"T{_nextId++:D5}";
        }

        // abstract method to get the price of the ticket
        public abstract decimal GetPrice();
    }

    internal class StandardTicket : Tickets
    {
        public StandardTicket(decimal basePrice) : base(TicketType.Standard, basePrice) 
        {
        }

        public override decimal GetPrice()
        {
            return BasePrice;
        }
    }

    internal class VIPTicket : Tickets
    {
        public decimal AdditionalCost { get; }

        public VIPTicket(decimal basePrice, decimal additionalCost) : base(TicketType.VIP, basePrice)
        {
            AdditionalCost = additionalCost;
        }

        public override decimal GetPrice()
        {
            return BasePrice + AdditionalCost;
        }
    }
}

