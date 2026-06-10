using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PRG281_Project
{
    public static class DataStore
    {
        internal static void LoadAll(string guestsCsv, string ticketsCsv,
            out List<Guest> guests, out int nextGuestId, out int nextTicketId)
        {
            // Initialize output parameters
            guests = new List<Guest>();
            nextGuestId = 1;
            nextTicketId = 1;

            // Load guests
            if (File.Exists(guestsCsv))
            {
                
                foreach (var line in File.ReadAllLines(guestsCsv))
                {
                    // Skip empty lines
                    var parts = (line ?? "").Split(',');
                    if (parts.Length != 4) continue;    // Skip lines that don't have exactly 4 parts
                    // Ensure we have exactly 4 parts: ID, Name, Phone, Email
                    var id = parts[0].Trim();
                    var name = parts[1].Trim();
                    var phone = parts[2].Trim();
                    var email = parts[3].Trim();

                    try
                    {
                        var g = new Guest(name, phone, email);
                        // Use reflection to set GuestID 
                        ReflectionHack.SetReadonlyField(g, nameof(Guest.GuestID), id);
                        guests.Add(g);

                        // Update nextGuestId based on the current GuestID
                        if (int.TryParse(id.Substring(1), out int n) && n >= nextGuestId)
                        {
                            nextGuestId = n + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading guest data: {ex.Message}");
                    }
                }
            }

            if (File.Exists(ticketsCsv))
            {
                // Load tickets and associate them with guests
                var map = guests.ToDictionary(g => g.GuestID, g => g);
                foreach (var line in File.ReadAllLines(ticketsCsv))
                {
                    var parts = (line ?? "").Split(',');
                    // Skip lines that don't have enough parts
                    if (parts.Length < 5) continue;
                    // Ensure we have at least 5 parts: TicketID, GuestID, Type, BasePrice, AdditionalCost
                    var tid = parts[0].Trim();
                    var gid = parts[1].Trim();
                    var typeStr = parts[2].Trim();
                    var baseStr = parts[3].Trim();
                    var addStr = parts[4].Trim();
                   
                    // Ensure the guest exists in the map
                    if (!map.TryGetValue(gid, out var guest)) continue;
                    // Ensure the type is valid
                    if (!Enum.TryParse(typeStr, true, out TicketType type)) continue;
                    // Ensure the base price is a valid decimal
                    if (!decimal.TryParse(baseStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var basePrice)) continue;
                    // Ensure the additional cost is a valid decimal, default to 0 if TryParsing fails
                    if (!decimal.TryParse(addStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var addCost)) addCost = 0m;


                    Tickets ticket = type == TicketType.Standard
                        ? new StandardTicket(basePrice)
                        : (Tickets)new VIPTicket(basePrice, addCost);

                    // Use reflection to set TicketID
                    ReflectionHack.SetReadonlyField(ticket, nameof(Tickets.TicketID), tid);
                    guest.AddTicket(ticket);

                    // Update nextTicketId based on the current TicketID
                    if (int.TryParse(tid.Substring(1), out int n) && n >= nextTicketId)
                    {
                        nextTicketId = n + 1;
                    }
                }
            }
        }

        internal static void SaveAll(string guestsCsv, string ticketsCsv, List<Guest> guests)
        {
            try
            {
                // Write header
                using (var sw = new StreamWriter(guestsCsv, false))
                {
                    
                    foreach (var g in guests)
                    {
                        // Ensure the guest input is not null
                        var name = Escape(g?.Name ?? string.Empty);
                        var phone = Escape(g?.PhoneNo ?? string.Empty);
                        var email = Escape(g?.Email ?? string.Empty);
                        
                        sw.WriteLine($"{g.GuestID},{name},{phone},{email}");
                        
                    }
                }

                // Write header
                using (var sw = new StreamWriter(ticketsCsv, false))
                {
                    
                    foreach (var g in guests)
                    {
                        // Ensure the guest has tickets
                        if (g?.Tickets == null) continue;

                        foreach (var t in g.Tickets)
                        {
                            // Ensure the ticket input is valid
                            var addCost = (t is VIPTicket vt) ? vt.AdditionalCost : 0m;
                            var ticketId = t?.TicketID ?? string.Empty;
                            var basePrice = t?.BasePrice.ToString(CultureInfo.InvariantCulture) ?? "0";
                            var additionalCost = addCost.ToString(CultureInfo.InvariantCulture);
                            var type = t?.Type.ToString() ?? "Standard";

                            sw.WriteLine($"{ticketId},{g.GuestID},{type},{basePrice},{additionalCost}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
                throw;
            }
        }

        internal static void AppendPayment(string paymentsCsv, Payment payment, Guest guest, Tickets ticket)
        {
            // append payment details to the CSV file
            File.AppendAllText(paymentsCsv,
            $"{payment.PaidAt:yyyy-MM-dd HH:mm:ss},{guest.GuestID},{ticket.TicketID},{payment.Method},{payment.Amount.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}");
            
        }

        private static string Escape(string s) => (s ?? "").Replace(",", " ");
       
    }

    internal static class ReflectionHack
    {
        public static void SetReadonlyField(object obj, string propertyName, string newValue)
        {
            var t = obj.GetType();
            
            var field = t.GetField($"<{propertyName}>k__BackingField",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
            
            // If the field is not found, try to get the property instead
            if (field == null) return;
            field.SetValue(obj, newValue);
        }
    }
}
