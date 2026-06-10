using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PRG281_Project
{
    public abstract class Person
    {
        private string _name = "";
        private string _phoneNo = "";
        private string _email = "";

        //accessor properties with validation
        public string Name
        {
            get => _name;
            set
            {
                // Validate name: not empty, max length 50 characters
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                if (value.Length > 50)
                    throw new ArgumentException("Name cannot be more than 50 characters.");
                _name = value.Trim();
            }
        }

        public string PhoneNo
        {
            get => _phoneNo;
            set
            {
                // Regex to validate phone number (10-15 digits, optional leading +)
                if (!Regex.IsMatch(value ?? "", @"^\+?\d{10,15}$"))
                    throw new ArgumentException("Invalid phone number (expect 10–15 digits, optional +).");
                _phoneNo = value;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                // Regex to validate email format
                if (!Regex.IsMatch(value ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new ArgumentException("Invalid email format.");
                _email = value.Trim();
            }
        }

        protected Person() { }

        protected Person(string name, string phoneNo, string email)
        {
            Name = name;
            PhoneNo = phoneNo;
            Email = email;
        }

        public virtual string DisplayPersonalInfo()
        {
            return $"Person ------\n\tName: {Name}\n\tPhone: {PhoneNo}\n\tEmail: {Email}\n-------------------------";
        }
    }

    // class Guest inherits from Person and implements IEquatable<Guest>
    // This class represents a guest with a unique GuestID and a list of tickets.
    internal class Guest : Person, IEquatable<Guest>
    {
        private static int _nextId = 1;
        public static void SetNextId(int next) => _nextId = Math.Max(next, 1);

        public string GuestID { get; }
        public List<Tickets> Tickets { get; } = new List<Tickets>();

        public Guest(string name, string phoneNo, string email) : base(name, phoneNo, email)
        {
            GuestID = $"G{_nextId++:D4}";
        }

        public void AddTicket(Tickets ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            Tickets.Add(ticket);
        }

        public override string DisplayPersonalInfo()
        {
            return base.DisplayPersonalInfo() + $"\n\tGuest ID: {GuestID}";
        }

        // Implement IEquatable<Guest> for comparing Guest objects
        public bool Equals(Guest other)
        {
            return other != null && other.GuestID == GuestID;
        }
        // Override GetHashCode and Equals for proper comparison
        public override int GetHashCode()
        {
            return GuestID.GetHashCode();
        }
        // Override Equals to compare Guest objects
        public override bool Equals(object obj)
        {
            return obj is Guest g && Equals(g);
        }
    }
}
