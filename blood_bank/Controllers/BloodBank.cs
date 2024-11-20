using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using blood_bank.Models;

namespace BloodBankManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodBankController : ControllerBase
    {
        // In-memory list to store blood bank entries
        private static List<BloodBankEntry> _bloodBankEntries = new List<BloodBankEntry>();
        private static int _nextId = 1;

        // 1. CRUD Operations

        // Create (POST /api/bloodbank)
        [HttpPost]
        public IActionResult CreateBloodBankEntry([FromBody] BloodBankEntry entry)
        {
            if (entry == null)
            {
                return BadRequest("Invalid entry data.");
            }

            // Auto-generate the Id
            entry.Id = _nextId++;
            _bloodBankEntries.Add(entry);

            return CreatedAtAction(nameof(GetBloodBankEntryById), new { id = entry.Id }, entry);
        }

        // Read (GET /api/bloodbank)
        [HttpGet]
        public IActionResult GetAllBloodBankEntries()
        {
            return Ok(_bloodBankEntries);
        }

        // Read (GET /api/bloodbank/{id})
        [HttpGet("{id}")]
        public IActionResult GetBloodBankEntryById(int id)
        {
            var entry = _bloodBankEntries.FirstOrDefault(e => e.Id == id);

            if (entry == null)
            {
                return NotFound($"Entry with Id = {id} not found.");
            }

            return Ok(entry);
        }

        // Update (PUT /api/bloodbank/{id})
        [HttpPut("{id}")]
        public IActionResult UpdateBloodBankEntry(int id, [FromBody] BloodBankEntry updatedEntry)
        {
            if (updatedEntry == null || id != updatedEntry.Id)
            {
                return BadRequest("Invalid entry data.");
            }

            var entry = _bloodBankEntries.FirstOrDefault(e => e.Id == id);

            if (entry == null)
            {
                return NotFound($"Entry with Id = {id} not found.");
            }

            // Update fields
            entry.DonorName = updatedEntry.DonorName;
            entry.Age = updatedEntry.Age;
            entry.BloodType = updatedEntry.BloodType;
            entry.ContactInfo = updatedEntry.ContactInfo;
            entry.Quantity = updatedEntry.Quantity;
            entry.CollectionDate = updatedEntry.CollectionDate;
            entry.ExpirationDate = updatedEntry.ExpirationDate;
            entry.Status = updatedEntry.Status;

            return NoContent();
        }

        // Delete (DELETE /api/bloodbank/{id})
        [HttpDelete("{id}")]
        public IActionResult DeleteBloodBankEntry(int id)
        {
            var entry = _bloodBankEntries.FirstOrDefault(e => e.Id == id);

            if (entry == null)
            {
                return NotFound($"Entry with Id = {id} not found.");
            }

            _bloodBankEntries.Remove(entry);
            return NoContent();
        }

        // 2. Pagination
        // GET /api/bloodbank?page={pageNumber}&size={pageSize}
        [HttpGet("page")]
        public IActionResult GetPaginatedBloodBankEntries(int page = 1, int size = 10)
        {
            var totalEntries = _bloodBankEntries.Count;
            var totalPages = (int)Math.Ceiling(totalEntries / (double)size);

            var paginatedEntries = _bloodBankEntries
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            var response = new
            {
                Page = page,
                Size = size,
                TotalPages = totalPages,
                TotalEntries = totalEntries,
                Entries = paginatedEntries
            };

            return Ok(response);
        }

        // 3. Search Functionality
        // GET /api/bloodbank/search?bloodType={bloodType}&status={status}&donorName={donorName}
        [HttpGet("search")]
        public IActionResult SearchBloodBankEntries([FromQuery] string bloodType, [FromQuery] string status, [FromQuery] string donorName)
        {
            var query = _bloodBankEntries.AsQueryable();

            if (!string.IsNullOrEmpty(bloodType))
            {
                query = query.Where(e => e.BloodType.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(donorName))
            {
                query = query.Where(e => e.DonorName.Contains(donorName, StringComparison.OrdinalIgnoreCase));
            }

            var result = query.ToList();

            if (!result.Any())
            {
                return NotFound("No entries match the search criteria.");
            }

            return Ok(result);
        }

        // Bonus: Sorting Functionality
        // GET /api/bloodbank/sorted?sortBy={field}
        [HttpGet("sorted")]
        public IActionResult GetSortedBloodBankEntries([FromQuery] string sortBy)
        {
            var entries = _bloodBankEntries.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "bloodtype":
                        entries = entries.OrderBy(e => e.BloodType);
                        break;
                    case "collectiondate":
                        entries = entries.OrderBy(e => e.CollectionDate);
                        break;
                    case "donorname":
                        entries = entries.OrderBy(e => e.DonorName);
                        break;
                    default:
                        return BadRequest("Invalid sort parameter. Use 'bloodType', 'collectionDate', or 'donorName'.");
                }
            }
            else
            {
                return BadRequest("Sort parameter is required.");
            }

            return Ok(entries.ToList());
        }
    }
}
