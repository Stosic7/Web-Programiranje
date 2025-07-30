namespace WebTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AerodromController : ControllerBase
    {
        public IspitContext Context { get; set; }

        public AerodromController(IspitContext context)
        {
            Context = context;
        }

        [HttpPost("DodajAerodrom")]
        public async Task<ActionResult> DodajAerodrom([FromBody] Aerodrom a)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a.Naziv))
                {
                    return BadRequest("Naziv mora da postoji.");
                }

                await Context.Aerodromi.AddAsync(a);
                await Context.SaveChangesAsync();

                return Ok($"Aerodrom sa nazivom {a.Naziv} i ID-em {a.ID} je dodat.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("IzbrisiAerodrom/{idAerodroma}")]
        public async Task<IActionResult> IzbrisiAerodrom(int idAerodroma)
        {
            try
            {
                var aerodrom = await Context
                    .Aerodromi.Include(a => a.PolazniLetovi)
                    .Include(a => a.DolazniLetovi)
                    .FirstOrDefaultAsync(a => a.ID == idAerodroma);

                if (aerodrom == null)
                {
                    return NotFound("Aerodrom nije pronađen.");
                }

                Context.Letovi.RemoveRange(aerodrom.PolazniLetovi!);
                Context.Letovi.RemoveRange(aerodrom.DolazniLetovi!);
                Context.Aerodromi.Remove(aerodrom);
                await Context.SaveChangesAsync();

                return Ok($"Uspesno obrisan aerodrom sa ID: {idAerodroma}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}