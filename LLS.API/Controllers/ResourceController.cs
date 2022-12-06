using LLS.Common.Dto;
using LLS.Common.Models;
using LLS.Common.Transfere_Layer_Object;
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
    public class ResourceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ResourceController(AppDbContext context)
        {
            _context = context;
        }

        //[HttpPost("Add-Machine")]
        //public async Task<IActionResult> AddMachine(Machine machine)
        //{
        //    if (!String.IsNullOrWhiteSpace(machine.Name) && !String.IsNullOrWhiteSpace(machine.IP))
        //    {
        //        await _context.Machines.AddAsync(new Machine()
        //        {
        //            Name = machine.Name,
        //            IP = machine.IP
        //        });
        //        await _context.SaveChangesAsync();
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid payloud");
        //    }
        //}

        [HttpPost("Create")]
        public async Task<IActionResult> AddResource(ResourceDto resource)
        {
            if (!String.IsNullOrWhiteSpace(resource.Name))
            {
                await _context.Resources.AddAsync(new Resource()
                {
                    Name = resource.Name
                });
                await _context.SaveChangesAsync();
                return Ok(new Result()
                {
                    Status = true,
                    Message = "Added Successfully"
                });
            }
            else
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "Invalid Name"
                });
            }
        }

        [HttpPut("{id}/Edit")]
        public async Task<IActionResult> EditResource(Guid id, string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {

                var res = await _context.Resources.FirstOrDefaultAsync(x => x.Id == id);
                if(res == null)
                {
                    return BadRequest(new Result()
                    {
                        Status = false,
                        Message = "No Resources with this Id"
                    });
                }
                res.Name = name;

                await _context.SaveChangesAsync();
                return Ok(new Result()
                {
                    Status = true,
                    Message = "Updated Successfully"
                });
            }
            else
            {
                return BadRequest(new Result()
                {
                    Status = false,
                    Message = "Invalid Name"
                });
            }
        }

        [HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            
                var res = await _context.Resources.FirstOrDefaultAsync(x => x.Id == id);
                if (res == null)
                {
                    return BadRequest(new Result()
                    {
                        Status = false,
                        Message = "No Resources with this Id"
                    });
                }

                _context.Resources.Remove(res);
                await _context.SaveChangesAsync();

                return Ok(new Result()
                {
                    Status = true,
                    Message = "Deleted Successfully"
                });
        }

        //[HttpGet("Get-Machine")]
        //public async Task<IActionResult> getMachine(Guid id)
        //{
        //    var machine = await _context.Machines.FindAsync(id);

        //    if (machine == null)
        //    {
        //        return BadRequest("Invalid payloud");
        //    }

        //    var resource = _context.Resource_Machines.Where(x=> x.MachineId == id).Select(x=>x.Resource);


        //    return Ok(new {machine, resource});
        //}

        //[HttpPost("Add-Resource-To-Machine")]
        //public async Task<IActionResult> AddResourceToMachine(Guid id, Guid resourceId)
        //{
        //    if (!String.IsNullOrWhiteSpace(id.ToString()))
        //    {
        //        var machine = await _context.Machines.FindAsync(id);
        //        var resource = await _context.Resources.FindAsync(resourceId);

        //        if (machine == null && resource == null)
        //        {
        //            return BadRequest("Invalid payloud");
        //        }

        //        await _context.Resource_Machines.AddAsync(new Resource_Machine()
        //        {
        //            MachineId = machine.Id,
        //            ResourceId = resource.Id,
        //            Machine = machine,
        //            Resource = resource
        //        });

        //        await _context.SaveChangesAsync();
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid payloud");
        //    }
        //}

        //[HttpGet("GetAll-Machines")]
        //public async Task<IActionResult> GetAllMachines()
        //{
        //    var machines = await _context.Machines.ToListAsync();
        //    return Ok(machines);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllResources()
        {
            var resources = await _context.Resources.Select(x=>new ResourceDto()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return Ok(new Result()
            {
                Status = true,
                Data = resources
            });
        }
    }
}
