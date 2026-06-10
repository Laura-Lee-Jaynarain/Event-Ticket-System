using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;

namespace PRG281_Project
{
    // Enum ticket types
    public enum TicketType { Standard, VIP }
    // Enum payment methods
    public enum PaymentMethod { Cash, Card, EFT }

    internal class Program
    {
        // List all guests
        private static readonly List<Guest> Guests = new List<Guest>();
        // Event manager
        private static readonly EventManager EventBus = new EventManager();

        // Dictionary for ticket prices
        private static readonly Dictionary<TicketType, decimal> TicketPrices = new Dictionary<TicketType, decimal>
        {
            { TicketType.Standard, 100m },
            { TicketType.VIP, 200m }
        };

        // Method to wait for user input with added animation
        private static void WaitForContinue()
        {
            Console.WriteLine("Processing...");
            // Display animation while waiting
            string[] animation = new string[] { "/", "-", "\\", "|" };
            ConsoleColor[] colors = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow };
            for (int i = 0; i < 24; i++)
            {
                Console.ForegroundColor = colors[i % 4];
                Console.Write("\r" + animation[i % 4]);
                Thread.Sleep(100);
            }
            // HCI guidance , press enter to continue
            Console.ResetColor();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            // Clear the console for next operation
            Console.Clear();
        }

        // Data directory, file paths
        private static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "data");
        private static readonly string GuestsCsv = Path.Combine(DataDir, "guests.csv");
        private static readonly string TicketsCsv = Path.Combine(DataDir, "tickets.csv");
        private static readonly string PaymentsCsv = Path.Combine(DataDir, "payments.csv");
        private static readonly string CouponsCsv = Path.Combine(DataDir, "coupons.csv");

        static void Main(string[] args)
        {

            // login
            bool isLoggedIn = false;
            Console.WriteLine("Login by using your credentials");
            while (!isLoggedIn)
            {
                try
                {
                    Console.Write("Enter username: ");
                    string username = Console.ReadLine()?.Trim() ?? "";

                    Console.Write("Enter password: ");
                    string password = ReadPasswordFromConsoleInput();

                    if (username == "administrator" && password == "1233212")
                    {
                        isLoggedIn = true;
                        Console.WriteLine();
                        WaitForContinue();
                        Console.WriteLine("\nLogin successful.");
                        Thread.Sleep(1500); // Pause to show success message
                    }
                    else
                    {
                        Console.Clear();
                        WaitForContinue();
                        Console.WriteLine("\nIncorrect username or password. Try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError during login: {ex.Message}");
                }
            }
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Directory.CreateDirectory(DataDir);

            // Event handler for ticket sold
            EventBus.OnTicketSold += (s, e) =>
            {
                Console.WriteLine($"\n[EVENT] Ticket sold → {e.Guest.Name} | {e.Ticket.Type} | TicketID {e.Ticket.TicketID} | Price R{e.Price:0.00}");
            };

            // Load data from CSV 
            DataStore.LoadAll(GuestsCsv, TicketsCsv, out var loadedGuests, out var nextGuestId, out var nextTicketId);
            // Set next IDs and clear existing guests
            Guests.Clear();
            // Add loaded guests and set next IDs
            Guests.AddRange(loadedGuests);
            // Set next guest ID
            Guest.SetNextId(nextGuestId);
            // Set next ticket ID
            Tickets.SetNextId(nextTicketId);

            // Ensure default coupons and load from file (if exists)
            CouponBook.EnsureDefaultCoupons();
            // Load coupons from CSV file if exists
            if (File.Exists(CouponsCsv)) CouponBook.LoadFromFile(CouponsCsv);

            //watch for coupon changes
            var watcher = new DirectoryMonitor(DataDir, Path.GetFileName(CouponsCsv), OnCouponsFileChanged);
            // GUI - menu representation and options
            bool exit = false;
            while (!exit)
            {
                //colouring
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("█▀▀ █░█ █▀▀ █▄░█ ▀█▀   █▀▀ █░█ █▀▀ █▀ ▀█▀");
                Console.WriteLine("██▄ ▀▄▀ ██▄ █░▀█ ░█░   █▄█ █▄█ ██▄ ▄█ ░█░");
                Console.WriteLine();
                Console.WriteLine("▄▀█ █▄░█ █▀▄");
                Console.WriteLine("█▀█ █░▀█ █▄▀");
                Console.WriteLine();
                Console.WriteLine("▀█▀ █ █▀▀ █▄▀ █▀▀ ▀█▀   █▀▄▀█ ▄▀█ █▄░█ ▄▀█ █▀▀ █▀▀ █▀▄▀█ █▀▀ █▄░█ ▀█▀");
                Console.WriteLine("░█░ █ █▄▄ █░█ ██▄ ░█░   █░▀░█ █▀█ █░▀█ █▀█ █▄█ ██▄ █░▀░█ ██▄ █░▀█ ░█░");
                Console.ResetColor();
                //menu options
                Console.WriteLine("1. Register New Guest");
                Console.WriteLine("2. View All Guests");
                Console.WriteLine("3. Sell Ticket");
                Console.WriteLine("4. Search Guest by Name");
                Console.WriteLine("5. View Loaded Coupons");
                Console.WriteLine("6. Save & Exit");
                Console.Write("Enter option: ");
                // user menu choice
                var choice = (Console.ReadLine() ?? "").Trim();
                Console.Clear();
                switch (choice)
                {
                    case "1":
                        // register new guest
                        Console.Clear();
                        RegisterGuest();
                        WaitForContinue();
                        break;
                    case "2":
                        // view all guests
                        Console.Clear();
                        DisplayGuests();
                        WaitForContinue();
                        break;
                    case "3":
                        // sell ticket
                        Console.Clear();
                        SellTicket();
                        WaitForContinue();
                        break;
                    case "4":
                        // search guest by name
                        Console.Clear();
                        SearchGuest();
                        WaitForContinue();
                        break;
                    case "5":
                        // view loaded coupons
                        Console.Clear();
                        ShowCoupons();
                        WaitForContinue();
                        break;
                    case "6":
                        // save all data and exit
                        Console.Clear();
                        SaveAll();
                        exit = true;
                        break;
                    default:
                        // invalid choice
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        // Handle file change events for the coupons file.
        private static void OnCouponsFileChanged()
        {
            // load file for coupons
            try
            {
                if (File.Exists(CouponsCsv))
                {
                    CouponBook.LoadFromFile(CouponsCsv);
                    // Notify user of successful reload
                    Console.WriteLine("[WATCHER] Coupons reloaded from file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WATCHER] Failed to reload coupons: {ex.Message}");
            }
        }

        private static void RegisterGuest()
        {
            // GUI - option display
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("█▀█ █▀▀ █▀▀ █ █▀ ▀█▀ █▀▀ █▀█   █▀▀ █░█ █▀▀ █▀ ▀█▀");
            Console.WriteLine("█▀▄ ██▄ █▄█ █ ▄█ ░█░ ██▄ █▀▄   █▄█ █▄█ ██▄ ▄█ ░█░");
            Console.ResetColor();

            try
            {
                // Read details from console input
                Console.Write("Enter guest name: ");
                string name = (Console.ReadLine() ?? "").Trim();

                Console.Write("Enter email: ");
                string email = (Console.ReadLine() ?? "").Trim();

                Console.Write("Enter phone number: ");
                string phone = (Console.ReadLine() ?? "").Trim();

                // Create new guest,  add to list
                var g = new Guest(name, phone, email);
                Guests.Add(g);

                Console.WriteLine($"Guest registered. ID: {g.GuestID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private static void DisplayGuests()
        {
            // GUI - option display
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("█░█ █ █▀▀ █░█░█   █▀▀ █░█ █▀▀ █▀ ▀█▀ █▀");
            Console.WriteLine("▀▄▀ █ ██▄ ▀▄▀▄▀   █▄█ █▄█ ██▄ ▄█ ░█░ ▄█");
            Console.ResetColor();

            if (Guests.Count == 0)
            {
                Console.WriteLine("No guests registered yet.");
                return;
            }

            foreach (var g in Guests.OrderBy(x => x.Name))
            {
                Console.WriteLine(g.DisplayPersonalInfo());

                decimal totalAmount = 0;
                int ticketCount = 0;

                if (g.Tickets.Count > 0)
                {
                    Console.WriteLine($"\tTickets ({g.Tickets.Count}):");

                    // Display each ticket with its details
                    foreach (var t in g.Tickets)
                    {
                        totalAmount += t.GetPrice(); // Add ticket price to total
                        ticketCount++;
                        Console.WriteLine($"\t  - {t.TicketID} | {t.Type} | Base R{t.BasePrice:0.00} | Price R{t.GetPrice():0.00}");
                    }

                    var groupDiscount = new GroupDiscount(ticketCount); // Calculate discount
                    // Apply discount to total amount
                    decimal discountedAmount = groupDiscount.Apply(totalAmount);
                    // Display total amount and discount
                    Console.WriteLine($"\tTotal: R{totalAmount:0.00} (Discounted: R{discountedAmount:0.00})");
                    Console.WriteLine($"\tDiscount: {groupDiscount.Percent:P2}");
                }

                Console.WriteLine(new string('-', 40));
            }
        }

        private static void SellTicket()
        {
            // GUI - option display
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("█▀ █▀▀ █░░ █░░   ▀█▀ █ █▀▀ █▄▀ █▀▀ ▀█▀");
            Console.WriteLine("▄█ ██▄ █▄▄ █▄▄   ░█░ █ █▄▄ █░█ ██▄ ░█░");
            Console.ResetColor();

            Console.Write("Enter guest name: ");
            // Read guest name from console input with coalescing null operator check
            string guestName = (Console.ReadLine() ?? "").Trim();

            // Find guest by name
            // Use case-insensitive comparison for guest name
            var guest = Guests.FirstOrDefault(g => g.Name.Equals(guestName, StringComparison.OrdinalIgnoreCase));
            if (guest == null)
            {
                Console.WriteLine("Guest not found. Please register first.");
                return;
            }

            Console.WriteLine("Available ticket types:");
            // Display available ticket types and their prices
            foreach (var kv in TicketPrices)
                Console.WriteLine($"{kv.Key} - R{kv.Value:0.00}");

            Console.Write("Select ticket type (Standard/VIP): ");
            string chosen = (Console.ReadLine() ?? "").Trim();
            if (!Enum.TryParse(chosen, true, out TicketType type))
            {
                Console.WriteLine("Invalid ticket type.");
                return;
            }
            Console.Write("Enter quantity to buy: ");
            int quantity;
            // Validate quantity input
            while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
            {
                Console.Write("Invalid quantity. Please enter a positive integer: ");
            }

            // using ternary operator to create ticket based on type
            Tickets ticket = (type == TicketType.Standard)
            ? (Tickets)new StandardTicket(TicketPrices[type])
            : new VIPTicket(TicketPrices[type], 0m);

            // Calculate total price and apply discount
            decimal basePrice = ticket.GetPrice();
            decimal totalAmount = basePrice * quantity;

            var groupDiscount = new GroupDiscount(quantity);
            decimal discountedAmount = groupDiscount.Apply(totalAmount);

            Console.Write("Enter coupon code (press ENTER if none): ");
            string code = (Console.ReadLine() ?? "").Trim();

            // Apply coupon if provided
            IDiscount applied = new NoDiscount();
            if (!string.IsNullOrWhiteSpace(code))
            {
                if (CouponBook.TryGetPercent(code, out var pct))
                    applied = new CouponDiscount(code, pct);
                else
                    Console.WriteLine("[INFO] Coupon not found. Continuing without coupon.");
            }

            discountDelegate discountFn = amount => applied.Apply(amount);
            decimal finalPrice = discountFn(discountedAmount);

            Console.Write("Payment method (Cash/Card/EFT): ");
            string pm = (Console.ReadLine() ?? "").Trim();
            if (!Enum.TryParse(pm, true, out PaymentMethod method))
            {
                Console.WriteLine("Invalid payment method.");
                return;
            }

            // Process ticket on separate thread
            new Thread(() => ProcessTicket(guest)).Start();

            // Log payment
            var payment = new Payment(finalPrice, method);
            payment.LogToConsole();

            for (int i = 0; i < quantity; i++)
            {
                guest.AddTicket(ticket);
            }
            EventBus.TriggerTicketSold(guest, ticket, finalPrice);
            DataStore.AppendPayment(PaymentsCsv, payment, guest, ticket);
        }

        private static void SearchGuest()
        {
            // GUI - option display
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("█▀ █▀▀ ▄▀█ █▀█ █▀▀ █░█   █▀▀ █░█ █▀▀ █▀ ▀█▀");
            Console.WriteLine("▄█ ██▄ █▀█ █▀▄ █▄▄ █▀█   █▄█ █▄█ ██▄ ▄█ ░█░");
            Console.ResetColor();

            Console.Write("Enter name to search: ");
            string q = (Console.ReadLine() ?? "").Trim();

            var matches = Guests
                .Where(g => g.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(g => g.Name)
                .ToList();

            if (matches.Count == 0)
            {
                Console.WriteLine("No matching guests found.");
                return;
            }

            // Display matching guests
            foreach (var g in matches)
                Console.WriteLine($"{g.GuestID} | {g.Name} | {g.Email} | {g.PhoneNo}");
        }

        private static void ShowCoupons()
        {
            // GUI - option display
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("█▀ █░█ █▀█ █░█░█   █▀▀ █▀█ █░█ █▀█ █▀█ █▄░█ █▀");
            Console.WriteLine("▄█ █▀█ █▄█ ▀▄▀▄▀   █▄▄ █▄█ █▄█ █▀▀ █▄█ █░▀█ ▄█");
            Console.ResetColor();

            var list = CouponBook.ListCoupons();

            if (list.Count == 0)
            {
                Console.WriteLine("No coupons loaded.");
                Console.WriteLine($"Tip: create '{CouponsCsv}' with lines like: SAVE10,0.10");
                return;
            }

            // Display discounts for certain number of tickets
            Console.WriteLine("5 or more = 10% off");
            Console.WriteLine("10 or more = 20% off");
            Console.WriteLine("20 or more = 30% off");

            // Display loaded coupons
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loaded coupons:");
            Console.ResetColor();
            Console.WriteLine("----------------");
            Console.WriteLine("CODE | DISCOUNT");
            foreach (var (code, pct) in list)
                Console.WriteLine($"- {code} → {pct:P0}");
        }

        private static void SaveAll()
        {
            // Save guests and tickets to CSV files
            DataStore.SaveAll(GuestsCsv, TicketsCsv, Guests);

            // Display animation while saving
            Console.WriteLine("Saving data...");
            string[] animation = new string[] { "/", "-", "\\", "|" };
            ConsoleColor[] colors = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow };
            for (int i = 0; i < 50; i++)
            {
                Console.ForegroundColor = colors[i % 4];
                Console.Write("\r" + animation[i % 4]);
                Thread.Sleep(100);
            }
            Console.ResetColor();
            Console.WriteLine("\rSave complete!");
        }

        private static void ProcessTicket(Guest guest)
        {
            // Simulate processing delay
            Thread.Sleep(2000);

            // Display confirmation email sent to guest
            Console.WriteLine($"[THREAD] Confirmation email sent to {guest.Email}");
        }
        
                // Method to hide entered password with asterisks, while console reads input
        private static string ReadPasswordFromConsoleInput()
        {
            var password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        Console.Write("\b \b"); // remove character
                        password.Length--; // remove character from stringbuilder
                    }
                }
                else
                {
                    Console.Write("*");
                    password.Append(key.KeyChar);
                }
            }
            return password.ToString();
        }

    }
}

