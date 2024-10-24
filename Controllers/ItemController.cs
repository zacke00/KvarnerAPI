using Microsoft.AspNetCore.Mvc;
using KvarnerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace KvarnerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemContext context;
        private readonly IWebHostEnvironment hosting;

        public ItemController(ItemContext _context, IWebHostEnvironment _hosting){
        context = _context;
        hosting = _hosting;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Items>>> GetItems()
        {
            try
            {
                return await context.Items.ToListAsync();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Items>> GetItem(int id)
        {
            try
            {
                var item = await context.Items.FindAsync(id);

                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found.");
                }

                return item;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetImage/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var item = await context.Items.FindAsync(id);
            if (item != null && !string.IsNullOrEmpty(item.Image))
            {
                // Construct the path to the image file using the actual image name
                var imagePath = Path.Combine(hosting.WebRootPath, "images", item.Image);
                
                // Check if the file exists
                if (System.IO.File.Exists(imagePath))
                {
                    var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                    return File(imageBytes, "image/jpeg");
                }
                else
                {
                    return NotFound($"Image file for item with ID {id} was not found.");
                }
            }

            return NotFound($"Item with ID {id} does not have an associated image.");
        }

        // POST: api/Item
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Items>> PostItem(Items item)
        {
            try
            {
                context.Items.Add(item);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update errors specifically
                return StatusCode(500, $"Database error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("UploadImage/{name}")]
        public async Task<ActionResult> UploadImage(IFormFile file,string name)
        {
            try{

            string webRootPath = hosting.WebRootPath;
            string absolutePath = Path.Combine($"{webRootPath}/images/{name}.jpg");

            using(var fileStream = new FileStream(absolutePath, FileMode.Create)) 
            {
                file.CopyTo(fileStream);
            }
            return Ok();
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Item/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Items item)
        {
            if (id != item.Id)
            {
                return BadRequest("Item ID mismatch.");
            }

            context.Entry(item).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound($"Item with ID {id} not found.");
                }
                else
                {
                    return StatusCode(500, "A concurrency error occurred while updating the item.");
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update errors specifically
                return StatusCode(500, $"Database error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Item/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await context.Items.FindAsync(id);
                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found.");
                }

                context.Items.Remove(item);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update errors specifically
                return StatusCode(500, $"Database error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool ItemExists(int id)
        {
            try
            {
                return context.Items.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception and return false as a default value
                return false;
            }
        }
    }
}