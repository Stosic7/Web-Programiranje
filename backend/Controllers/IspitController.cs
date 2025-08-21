[ApiController]
[Route("[controller]")]
public class IspitController : ControllerBase
{
    public IspitContext Context { get; }
    public IspitController(IspitContext context) => Context = context;

    [HttpPost("DodajProjekciju")]
    public async Task<ActionResult> DodajProjekciju([FromBody] Projekcija p)
    {
        if (string.IsNullOrWhiteSpace(p.Naziv))
            return BadRequest("Naziv je obavezan.");
            
        try
        {
            await Context.Projekcije.AddAsync(p);
            await Context.SaveChangesAsync();
            return Ok(new { poruka = "Dodata projekcija", p.ID, p.Naziv, p.VremePrikazivanja });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("DodajSalu")]
    public async Task<ActionResult> DodajSalu([FromBody] Sala s)
    {
        if (string.IsNullOrWhiteSpace(s.Naziv))
            return BadRequest("Naziv je obavezan.");

        if (s.UkupanBrojRedova <= 0 || s.UkupanBrojSedista <= 0)
            return BadRequest("Broj redova i broj sedišta moraju biti > 0.");

        if (s.UkupanBrojSedista % s.UkupanBrojRedova != 0)
            return BadRequest("Ukupan broj sedišta mora biti deljiv brojem redova.");

        try
        {
            await Context.Sale.AddAsync(s);
            await Context.SaveChangesAsync();
            return Ok(new { poruka = "Dodata sala", s.ID, s.Naziv, s.UkupanBrojRedova, s.UkupanBrojSedista });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private decimal IzracunajCenu(int red, decimal osnovnaCena = 500m)
    {
        if (red <= 1) return osnovnaCena;
        return Math.Round(osnovnaCena * (decimal)Math.Pow(0.97, red - 1), 2);
    }

    [HttpPost("KupiKartu/{projekcijaId}/{salaId}/{red}/{sediste}")]
    public async Task<ActionResult> KupiKartu(int projekcijaId, int salaId, int red, int sediste)
    {
        try
        {
            var projekcija = await Context.Projekcije.FindAsync(projekcijaId);
            if (projekcija == null)
                return NotFound("Ne postoji ta projekcija");

            var sala = await Context.Sale.FindAsync(salaId);
            if (sala == null)
                return BadRequest("Sala ne postoji");

            var sedistaPoRedu = sala.UkupanBrojSedista / sala.UkupanBrojRedova;
            if (red < 1 || red > sala.UkupanBrojRedova || sediste < 1 || sediste > sedistaPoRedu)
                return BadRequest("Neispravan red ili sedište");

            var zauzetaKarta = await Context.Karte
                .FirstOrDefaultAsync(k => k.sifraProjekcije == projekcijaId &&
                                        k.Red == red &&
                                        k.Sediste == sediste &&
                                        k.Zauzeto);
            if (zauzetaKarta != null)
                return BadRequest("To sedište je već zauzeto.");

            decimal cena = IzracunajCenu(red);

            var novaRezervacija = new Karta
            {
                Placena = true,
                Zauzeto = true,
                Red = red,
                Sediste = sediste,
                Cena = cena,
                sifraProjekcije = projekcijaId,
                Projekcija = projekcija,
                Sala = sala
            };

            await Context.Karte.AddAsync(novaRezervacija);
            await Context.SaveChangesAsync();

            return Ok(new
            {
                Poruka = "Karta uspešno kupljena",
                novaRezervacija.ID,
                novaRezervacija.Red,
                novaRezervacija.Sediste,
                novaRezervacija.Cena
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("SelektujSediste/{projekcijaId}/{salaId}/{red}/{sediste}")]
    public async Task<ActionResult> SelektujSediste(int projekcijaId, int salaId, int red, int sediste)
    {
        var projekcija = await Context.Projekcije.FindAsync(projekcijaId);
        if (projekcija == null)
            return NotFound("Projekcija ne postoji");

        var sala = await Context.Sale.FindAsync(salaId);
        if (sala == null)
            return BadRequest("Sala ne postoji");

        var sedistaPoRedu = sala.UkupanBrojSedista / sala.UkupanBrojRedova;
        if (red < 1 || red > sala.UkupanBrojRedova || sediste < 1 || sediste > sedistaPoRedu)
            return BadRequest("Neispravan red ili sedište");

        decimal cena = IzracunajCenu(red);

        bool zauzeto = await Context.Karte.AnyAsync(k => k.sifraProjekcije == projekcijaId &&
                                                        k.Red == red &&
                                                        k.Sediste == sediste &&
                                                        k.Zauzeto);

        return Ok(new
        {
            sifraProjekcije = projekcijaId,
            SalaId = salaId,
            Red = red,
            Sediste = sediste,
            Cena = cena,
            Zauzeto = zauzeto,
            NazivProjekcije = projekcija.Naziv,
            Vreme = projekcija.VremePrikazivanja
        });
    }

    [HttpGet]
    [Route("/VratiZauzetaSedista/{projekcijaId}")]
    public async Task<ActionResult> VratiZauzetaSedista(int projekcijaId) {
        try{
            var zauzeta = await Context.Karte
                .Where(k => k.sifraProjekcije == projekcijaId && k.Zauzeto == true)
                .Select(k => new{
                    k.Red,
                    k.Sediste
                }).ToListAsync();

            return Ok(zauzeta);
        }catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("VratiProjekcije")]
    public async Task<ActionResult> VratiProjekcije() {
        var list = await Context.Projekcije
        .Select(p => new { id = p.ID, naziv = p.Naziv })
        .ToListAsync();
        return Ok(list);
    }

    [HttpGet("VratiSale")]
    public async Task<ActionResult> VratiSale()
    {
        var list = await Context.Sale
            .Select(s => new { id = s.ID, naziv = s.Naziv, ukupanBrojRedova = s.UkupanBrojRedova, ukupanBrojSedista = s.UkupanBrojSedista })
            .ToListAsync();
        return Ok(list);
    }
}
