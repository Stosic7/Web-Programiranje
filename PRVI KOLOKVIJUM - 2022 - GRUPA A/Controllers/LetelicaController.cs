namespace WebTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LetelicaController : ControllerBase
    {
        public IspitContext Context { get; set; }

        public LetelicaController(IspitContext context)
        {
            Context = context;
        }

        [HttpPost("DodajLetelicu")]
        public async Task<ActionResult> DodajLetelicu([FromBody] Letelica l)
        {
            try
            {
                await Context.Letelice.AddAsync(l);
                await Context.SaveChangesAsync();

                return Ok($"Letelica sa nazivom {l.Naziv} i ID-em {l.ID} je dodat.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}