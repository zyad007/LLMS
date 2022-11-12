using LLS.Common.Models;
using LLS.DAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceMatchingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ResourceMatchingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add-Machine")]
        public async Task<IActionResult> AddMachine(Machine machine)
        {
            if (!String.IsNullOrWhiteSpace(machine.Name) && !String.IsNullOrWhiteSpace(machine.IP))
            {
                await _context.Machines.AddAsync(new Machine()
                {
                    Name = machine.Name,
                    IP = machine.IP
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Invalid payloud");
            }
        }

        [HttpPost("Add-Resource")]
        public async Task<IActionResult> AddResource(Resource resource)
        {
            if (!String.IsNullOrWhiteSpace(resource.Name))
            {
                await _context.Resources.AddAsync(new Resource()
                {
                    Name = resource.Name
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Invalid payloud");
            }
        }

        [HttpGet("Get-Machine")]
        public async Task<IActionResult> getMachine(Guid id)
        {
            var machine = await _context.Machines.FindAsync(id);

            if (machine == null)
            {
                return BadRequest("Invalid payloud");
            }

            var resource = _context.Resource_Machines.Where(x=> x.MachineId == id).Select(x=>x.Resource);

            
            return Ok(new {machine, resource});
        }

        [HttpPost("Add-Resource-To-Machine")]
        public async Task<IActionResult> AddResourceToMachine(Guid id, Guid resourceId)
        {
            if (!String.IsNullOrWhiteSpace(id.ToString()))
            {
                var machine = await _context.Machines.FindAsync(id);
                var resource = await _context.Resources.FindAsync(resourceId);

                if (machine == null && resource == null)
                {
                    return BadRequest("Invalid payloud");
                }

                await _context.Resource_Machines.AddAsync(new Resource_Machine()
                {
                    MachineId = machine.Id,
                    ResourceId = resource.Id,
                    Machine = machine,
                    Resource = resource
                });

                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Invalid payloud");
            }
        }

        [HttpGet("GetAll-Machines")]
        public async Task<IActionResult> GetAllMachines()
        {
            var machines = await _context.Machines.ToListAsync();
            return Ok(machines);
        }

        [HttpGet("GetAll-Resources")]
        public async Task<IActionResult> GetAllResources()
        {
            var resources = await _context.Resources.ToListAsync();
            return Ok(resources);
        }
    }
}
