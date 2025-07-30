namespace WebTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LetController : ControllerBase
    {
        public IspitContext Context { get; set; }

        public LetController(IspitContext context)
        {
            Context = context;
        }

        [HttpPost("DodajLet/{polazniId}/{dolazniId}/{letelicaId}")]
        public async Task<ActionResult> DodajLet(
            [FromBody] Let let,
            int polazniId,
            int dolazniId,
            int letelicaId
        )
        {
            try
            {
                var polazniAerodrom = await Context.Aerodromi.FindAsync(polazniId);

                if (polazniAerodrom == null)
                    return BadRequest("Polazni aerodrom ne postoji.");

                var dolazniAerodrom = await Context.Aerodromi.FindAsync(dolazniId);

                if (dolazniAerodrom == null)
                    return BadRequest("Dolazni aerodrom ne postoji.");

                var letelica = await Context.Letelice.FindAsync(letelicaId);

                if (letelica == null)
                    return BadRequest("Letelica ne postoji.");

                if (let.BrojPutnika > letelica.KapacitetPutnika)
                    return BadRequest(
                        $"Putnike nije moguce semstiti u letelicu sa ID: {letelica.ID}."
                    );

                let.PolazniAerodrom = polazniAerodrom;
                let.DolazniAerodrom = dolazniAerodrom;
                let.Letelica = letelica;

                await Context.Letovi.AddAsync(let);
                await Context.SaveChangesAsync();

                return Ok($"Let dodat.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("VratiInformacijeOLetu/{aerodromId}")]
        public async Task<ActionResult> VratiInformacijeOLetu(int aerodromId)
        {
            try
            {
                var letovi = await Context
                    .Letovi.Include(l => l.PolazniAerodrom)
                    .Include(l => l.DolazniAerodrom)
                    .Include(l => l.Letelica)
                    .Where(l => l.PolazniAerodrom!.ID == aerodromId)
                    .OrderBy(l => l.VremePoletanja)
                    .ToListAsync();

                return Ok(letovi);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("VratiProsecnuDuzinuLeta/{idPolaznog}/{idDolaznog}")]
        public async Task<IActionResult> VratiProsecnuDuzinuLeta(int idPolaznog, int idDolaznog)
        {
            try
            {
                var x = await Context.Letovi
                    .Include(l => l.PolazniAerodrom)
                    .Include(l => l.DolazniAerodrom)
                    .Where(l =>
                        l.PolazniAerodrom!.ID == idPolaznog && l.DolazniAerodrom!.ID == idDolaznog
                    )
                    .Select(l => (l.VremeSletanja - l.VremePoletanja).TotalHours)
                    .ToListAsync();

                if (x.Count == 0)
                    return BadRequest("Nema takvog leta.");

                return Ok(x.Average());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}