using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Queues;
using TicketHubAPI.Data;
using TicketHubAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace TicketHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly TicketHubAPIContext _context;

        public ContactsController(TicketHubAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContact()
        {
            return await _context.Contact.ToListAsync();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contact.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            if (id != contact.ConcertId)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                string queueName = "contacts-queue";

                string? connectionString = _configuration["AzureStorageConnectionString"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new { message = "Error: No connection string was provided." });
                }

                try
                {
                    QueueClient queueClient = new QueueClient(connectionString, queueName);

                    string message = JsonSerializer.Serialize(contact);
                    await queueClient.SendMessageAsync(message);

                    return Ok(new { message = $"Contact information was sent successfully.\nHello {contact.FullName}!" });
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = "An error was encountered while sending contact info to queue.", error = e.Message });
                }
            }

            return BadRequest(new { message = "Error: A required field was missing." });
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.ConcertId == id);
        }
    }
}
