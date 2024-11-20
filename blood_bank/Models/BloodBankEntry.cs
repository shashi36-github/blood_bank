namespace blood_bank.Models
{
    public class BloodBankEntry
    {
        public int Id { get; set; } // Auto-generated unique identifier
        public string DonorName { get; set; }
        public int Age { get; set; }
        public string BloodType { get; set; } // e.g., A+, O-, B+
        public string ContactInfo { get; set; } // Phone number or email
        public int Quantity { get; set; } // Quantity in ml
        public DateTime CollectionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; } // e.g., "Available", "Requested", "Expired"
    }
}
