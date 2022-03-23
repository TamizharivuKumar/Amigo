#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Amigo.Models;
using Amigo.Helpers;
using Amigo.Entities;

namespace Amigo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AmigoContext _context;

        public AdminController(AmigoContext context)
        {
            _context = context;
        }

        // GET: api/Admin
        [Authorize(Role.Product)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmin()
        {
            return await _context.Admin.ToListAsync();
        }

        // GET: api/Admin/5
        [Authorize(Role.Product)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            try
            {
                var admin = await _context.Admin.FindAsync(id);

                if (admin == null)
                {
                    Sentry.SentrySdk.CaptureMessage("Admin Id Not Found");
                    return Ok(new { status = "Failed", data = admin, message = "No Admin with id Exsist" });
                }

                return admin;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "failed", message = "Get Admin Failed" });
            }
        }

        // PUT: api/Admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Role.Product)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {


            try
            {
                if (id != admin.Adminid)
                {
                    return Ok(new { status = "Failed", data = admin, message = "Admin Id not found" });
                }

                _context.Entry(admin).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(id))
                {
                    return Ok(new { status = "Failed", data = admin, messsage = "Admin Id is already available" });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { status = "Failed", data = admin, messsage = "Failed to put the admin" });
        }

        // POST: api/Admin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Role.Product)]
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            try
            {
                admin.Adminpassword = BCrypt.Net.BCrypt.HashPassword(admin.Adminpassword);
                _context.Admin.Add(admin);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAdmin", new { id = admin.Adminid }, Ok(new { data = admin }));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Post admin failed" });
            }
        }

        // DELETE: api/Admin/5
        [Authorize(Role.Product)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                var admin = await _context.Admin.FindAsync(id);
                if (admin == null)
                {
                    return Ok(new { status = "Failed", data = admin, message = "Admin Id not found" });
                }

                _context.Admin.Remove(admin);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", data = admin, messsage = "Admin Id has be deleted..." });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                Sentry.SentrySdk.CaptureException(ex);
                return BadRequest(new { status = "Failed", message = "Delete admin failed" });
            }
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.Adminid == id);
        }
    }
}
