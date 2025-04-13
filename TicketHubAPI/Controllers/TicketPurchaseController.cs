using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Queues;
using TicketHubAPI.Models;
using Azure.Storage.Queues.Models;
using System.Text;

namespace TicketHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketPurchaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TicketPurchaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/TicketPurchaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketPurchase>>> GetTicketPurchaces()
        {
            string? queueName = _configuration["QueueName"];
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
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

                //await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                return Ok(new { Message = message });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error retrieving message: {e.Message}");
            }
        }

        // POST: api/TicketPurchaces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TicketPurchase>> PostTicketPurchace(TicketPurchase ticketPurchase)
        {
            if (ModelState.IsValid)
            {
                string? queueName = _configuration["QueueName"];
                string? connectionString = _configuration["AzureStorageConnectionString"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new { message = "Error: No connection string was provided." });
                }

                if(!IsValidExpiration(ticketPurchase.Expiration))
                {
                    return BadRequest("Card has expired, or expiration date was not formatted correctly ('MM/YY').");
                }

                try
                {
                    QueueClient queueClient = new QueueClient(connectionString, queueName);

                    string message = JsonSerializer.Serialize(ticketPurchase);
                    await queueClient.SendMessageAsync(message);

                    var plainTextBytes = Encoding.UTF8.GetBytes(message);
                    await queueClient.SendMessageAsync(Convert.ToBase64String(plainTextBytes));

                    return Ok("Hello " + ticketPurchase.FirstName + ". Purchase info sent to queue.");
                }
                catch (Exception e)
                {
                    return StatusCode(500, new { message = "An error was encountered while sending info to queue.", error = e.Message });
                }
            }
            return BadRequest("Error: A required field was missing.");
        }

        // Checking credit card expiration date vs current date
        private bool IsValidExpiration(string expiration)
        {
            if(DateTime.TryParseExact(expiration, "MM/yy", null, System.Globalization.DateTimeStyles.None, out DateTime expDate))
            {
                return expDate > DateTime.UtcNow;
            }
            return false;
        }
    }
}
