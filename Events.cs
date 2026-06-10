using System;

namespace PRG281_Project
{
    public class TicketSoldEventArgs : EventArgs
    {
        internal Guest Guest { get; }
        internal Tickets Ticket { get; }
        public decimal Price { get; }

        internal TicketSoldEventArgs(Guest guest, Tickets ticket, decimal price)
        {
            // validate parameters
            Guest = guest ?? throw new ArgumentNullException(nameof(guest));
            Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
            Price = price;
        }
    }

    public class EventManager
    {
        // custom event using built-in EventHandler<T>
        public event EventHandler<TicketSoldEventArgs> OnTicketSold;

        internal void TriggerTicketSold(Guest guest, Tickets ticket, decimal price)
        {
            // invoke the event with the provided parameters
            OnTicketSold?.Invoke(this, new TicketSoldEventArgs(guest, ticket, price));
        }
    }
}
