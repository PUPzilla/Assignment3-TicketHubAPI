using System.ComponentModel.DataAnnotations;

namespace TicketHubAPI.Models
{
    public class Contact
    {
        [Required]
        public int ConcertId { get; set; }

        [Required(ErrorMessage = "An email address is required."), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A first name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A last name is required.")]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "A phone number is required."), Phone]
        public string Phone { get; set; } = string.Empty;

        [Required, Range(1, int.MaxValue, ErrorMessage = "You must buy at least 1 ticket.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "A credit card is required."), CreditCard]
        public string CreditCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your card's expiration date is required."), RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiration date must be in 'MM/YY' format.")]// Regex checks that exp date is MM/YY
        public string Expiration { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must enter your card's security code."), StringLength(4, MinimumLength = 3, ErrorMessage = "Code must be 3 or 4 digits long.")]
        public string SecurityCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your city is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your province/state is required")]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your country is required")]
        public string Country { get; set; } = string.Empty;
    }
}