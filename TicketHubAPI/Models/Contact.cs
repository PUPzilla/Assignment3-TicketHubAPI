using System.ComponentModel.DataAnnotations;

namespace TicketHubAPI.Models
{
    public class Contact
    {
        private readonly string _Name;
        private string _FirstName = string.Empty;
        private string _LastName = string.Empty;

        public required string FirstName 
        {
            get { return _FirstName; }
            set
            {
                _FirstName = value;
                SetName();
            }
        }

        public required string LastName 
        { 
            get { return _LastName; }
            set 
            {
                _LastName = value;
                SetName();
            }
        }

        public string Name
        {
            get { return _Name; }

            set
            {
                SetName();
            }
        }

        [Key]
        public int ContactId { get; set; }

        public int ConcertId { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Phone { get; set; }

        [CreditCard]
        [Required]
        public required string CreditCard { get; set; }

        [Required]
        public required string Expiration { get; set; }

        [Required]
        public required string SecurityCode { get; set; }

        public int Quantity { get; set; } = 0;
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Province { get; set; }
        public required string Country { get; set; }

        public string SetName()
        {

            return ($"{FirstName} {LastName}");
        }
    }
}