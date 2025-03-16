using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var all = dbContext.Pets.Include(p => p.Breed).ToListAsync();
        return Ok(all);
    }

    [HttpGet("{id}", Name = nameof(GetPetById))]
    public async Task<IActionResult> GetPetById(int id)
    {
        var pet = await dbContext.Pets.Include(p => p.Breed)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        return Ok(pet);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewPet newPet)
    {
        try
        {
            var pet = newPet.ToPet();
            await dbContext.Pets.AddAsync(pet);
            await dbContext.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetPetById), new { id = pet.Id }, newPet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message, newPet);
            return StatusCode(500);
        }
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, PetUpdate petUpdate)
    {
        try
        {
            var pet = await dbContext.Pets.FindAsync(id);

            if (pet == null)
            {
                return NotFound(id);
            }

            pet.Name = petUpdate.Name;
            pet.Age = petUpdate.Age;
            pet.BreedId = petUpdate.BreedId;
            await dbContext.SaveChangesAsync();

            return Ok(petUpdate);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.ToString());
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}

public record NewPet(string Name, int Age, int BreedId)
{
    public Pet ToPet()
    {
        return new Pet() { Age = Age, Name = Name, BreedId = BreedId };
    }
};

public record PetUpdate(string Name, int Age, int BreedId);
