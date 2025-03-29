using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Queues;
using TicketHubAPI.Data;
using TicketHubAPI.Models;
using Azure.Storage.Queues.Models;

namespace TicketHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPurchaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly TicketHubAPIContext _context;

        public TicketPurchaseController(TicketHubAPIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/TicketPurchaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketPurchase>>> GetTicketPurchace()
        {
            string queueName = "tickethub";
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(_configuration["AzureStorageConnectionString"]))
            {
                return BadRequest(new { message = "Error: No connection string was provided." });
            }

            try
            {
                QueueClient queueClient = new QueueClient(connectionString, queueName);

                QueueMessage[] retrievedMessages = await queueClient.ReceiveMessagesAsync(maxMessages: 32);

                if (retrievedMessages.Length == 0)
                {
                    return NotFound("No messages were found in the queue");
                }

                var message = retrievedMessages[0];

                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                return Ok(new { Message = message });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error retrieving message: {e.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<TicketPurchase>> GetAllConcertTickets()
        {

        }

        // POST: api/TicketPurchaces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TicketPurchase>> PostTicketPurchace(TicketPurchase ticketPurchase)
        {
            if (ModelState.IsValid)
            {
                string queueName = "tickethub";

                string? connectionString = _configuration["AzureStorageConnectionString"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new { message = "Error: No connection string was provided." });
                }

                try
                {
                    QueueClient queueClient = new QueueClient(connectionString, queueName);

                    string message = JsonSerializer.Serialize(ticketPurchase);
                    await queueClient.SendMessageAsync(message);

                    return Ok(new { message = $"TicketPurchace information was sent successfully.\nHello {ticketPurchase.FullName}!" });
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = "An error was encountered while sending info to queue.", error = e.Message });
                }
            }

            return BadRequest(new { message = "Error: A required field was missing." });
        }

        // DELETE: api/TicketPurchaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketPurchace(int id)
        {
            var ticketPurchase = await _context.TicketPurchases.FindAsync(id);
            if (ticketPurchase== null)
            {
                return NotFound();
            }

            _context.TicketPurchases.Remove(ticketPurchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }        

        private bool TicketPurchaceExists(int id)
        {
            return _context.TicketPurchases.Any(e => e.ConcertId == id);
        }
    }
}
