using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Authenticatie;
namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowController : ControllerBase
{
    private readonly GebruikerContext _context;
    private Kalender _kalender = new Kalender();
    public ShowController(GebruikerContext context)
    {
        _context = context;
    }

    [HttpGet("GetShows/{voorstellingID}")]
    public async Task<ActionResult<VoorstellingData>> GetVoorstellingen(int voorstellingID)
    {
        Voorstelling Voorstelling = await _context.Voorstellingen.FirstOrDefaultAsync(v => v.VoorstellingId == voorstellingID);

        List<Show> shows = await _context.Shows.Where(s => s.VoorstellingId == voorstellingID).ToListAsync();

        if (Voorstelling == null) return NotFound();

        return new VoorstellingData() { voorstelling = Voorstelling, shows = shows };
    }

    [HttpPost("AddShow")]
    public async Task<ActionResult> AddShow([FromBody] HerhaalbareShow HerhaalShow)
    {
        //Authorisatie toevoegen
        Show show = new Show(HerhaalShow.Zaalnummer, HerhaalShow.StartDatum, HerhaalShow.VoorstellingId, _kalender.KalenderId);

        _context.Shows.Add(show);
        List<Show> showlist = _kalender.HerhaalOptie(HerhaalShow.Interval, HerhaalShow.AantalKeer, show);
        foreach (Show s in showlist)
        {
            _context.Shows.Add(s);
        }

        if (await _context.SaveChangesAsync() > 0)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("VerwijderShow/{id}")]
    public async Task<ActionResult> VerwijderShow(int id)
    {
        //Authorisatie toevoegen
        Show show = _context.Shows.Find(id);
        _context.Shows.Remove(show);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}

public class VoorstellingData
{
    public Voorstelling voorstelling { get; set; }
    public List<Show> shows { get; set; }
}

public class HerhaalbareShow{
    public int Zaalnummer { get; set; }
    public DateTime StartDatum { get; set; }
    public int VoorstellingId { get; set; }
    public string? Interval { get; set; }
    public int? AantalKeer { get; set; }
}