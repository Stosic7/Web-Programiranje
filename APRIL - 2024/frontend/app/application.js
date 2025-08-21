import { Projekcija } from "./Projekcija.js";
import { Sala } from "./Sala.js";
import { Karta } from "./Karta.js";

export class Application {
  /**
   * @param {{id:number,naziv:string,vremePrikazivanja?:string}} projekcijaDTO
   * @param {{id:number,naziv:string,ukupanBrojRedova:number,ukupanBrojSedista:number}} salaDTO
   */
  constructor(projekcijaDTO, salaDTO) {
    this.projekcija = new Projekcija(
      projekcijaDTO.id,
      projekcijaDTO.naziv,
      projekcijaDTO.vremePrikazivanja ?? "-"
    );

    this.sala = new Sala(
      salaDTO.id,
      salaDTO.naziv,
      salaDTO.ukupanBrojRedova,
      salaDTO.ukupanBrojSedista
    );
  }

  async draw(container) {
    const body = document.createElement("div");
    body.classList.add("bodyContainer");

    // gornji naslov
    const header = document.createElement("div");
    header.classList.add("headerNaslov");
    header.innerText = `${this.projekcija.naziv}: ${this.projekcija.vremePrikazivanja} - ${this.sala.naziv}`;
    body.appendChild(header);

    const panel = document.createElement("div");
    panel.classList.add("panelWrap");

    //levi deo
    const forma = document.createElement("div");
    forma.classList.add("kriterijumiPretrage");
    this.drawForma(forma);
    panel.appendChild(forma);

    //desni deo
    const rezultatiPretrage = document.createElement("div");
    rezultatiPretrage.classList.add("rezultatiPretrage");
    panel.appendChild(rezultatiPretrage);

    const grid = document.createElement("div");
    grid.classList.add("salaGrid");
    rezultatiPretrage.appendChild(grid);
    this.$grid = grid;

    panel.appendChild(rezultatiPretrage);

    body.appendChild(panel);
    container.appendChild(body);

    this.nacrtajGrid();
  }

  drawForma(container) {
    const title = document.createElement("div");
    title.classList.add("kupovinaKarata");
    title.textContent = "Kupi Kartu";
    container.appendChild(title);

    const addField = (lbl,cls) => {
      const l = document.createElement("label");
      l.classList.add("margin-10");
      l.textContent = lbl;
      container.appendChild(l);

      const i = document.createElement("input");
      i.type = "text";
      i.readOnly = true;
      i.classList.add(`input-${cls}`, "margin-10");
      container.appendChild(i);
      return i;
    }

    this.$red     = addField("Red:", "red");
    this.$sediste = addField("Broj sedišta:", "sediste");
    this.$cena    = addField("Cena karte:", "cena");
    this.$sifra   = addField("Šifra:", "sifra");

    this.$btnKupi = document.createElement("input");
    this.$btnKupi.type = "button";
    this.$btnKupi.value = "Kupi kartu";
    this.$btnKupi.classList.add("kupiDugme");
    this.$btnKupi.disabled = true;
    container.appendChild(this.$btnKupi);

    this.$btnKupi.addEventListener("click", () => this.kupiKartu());
  }

  nacrtajGrid() {
  const kolone = this.sala.sedistaPoRed;
  this.$grid.innerHTML = "";

  for (let r = 1; r <= this.sala.ukupanBrojRedova; r++) {
      const redRow = document.createElement("div");
      redRow.classList.add("redRow");
      redRow.style.display = "grid";
      redRow.style.gridTemplateColumns = `repeat(${kolone}, minmax(48px, 1fr))`;
      redRow.style.gap = "8px";
      redRow.style.marginBottom = "12px";

      for (let s = 1; s <= kolone; s++) {
        const seat = document.createElement("div");
        seat.className = "sediste slobodno";
        seat.dataset.red = r;
        seat.dataset.sediste = s;
        seat.textContent = `${r} - ${s}`;

        seat.addEventListener("click", () => {
          if (seat.classList.contains("zauzeto")) return;
          this.handleKlikNaSediste(r, s);
        });
        redRow.appendChild(seat);
      }

      this.$grid.appendChild(redRow);
    }
  }

  async handleKlikNaSediste(red, sediste) {
    try{
      const url = `https://localhost:7080/Ispit/SelektujSediste/${this.projekcija.id}/${this.sala.id}/${red}/${sediste}`;
      const resp = await fetch(url);
      if (!resp.ok) {
        throw new Error(await resp.text());
      }

      const dto = await resp.json();

      const norm = {
        red:            dto.Red            ?? dto.red,
        sediste:        dto.Sediste        ?? dto.sediste,
        cena:           dto.Cena           ?? dto.cena,
        zauzeto:        dto.Zauzeto        ?? dto.zauzeto,
        sifraProjekcije:dto.sifraProjekcije ?? dto.SifraProjekcije ?? dto.sifra
      };

      if (dto.Zauzeto) {
        this.popuniFormu({
          red: norm.red,
          sediste: norm.sediste,
          cena: norm.cena,
          sifra: norm.sifraProjekcije
        });

        this.$btnKupi.disabled = true;
        this.obeleziSelektovano(red, sediste, true);
        return;
      }

      this.popuniFormu({
        red: norm.red,
        sediste: norm.sediste,
        cena: norm.cena,
        sifra: norm.sifraProjekcije
      });

      this.$btnKupi.disabled = false;
      this.obeleziSelektovano(red, sediste, false);
    }catch(err){
      console.error("SelektujSediste gresak: ", err);
      alert("Ne mogu da selektujem sediste");
    }
  }

  async kupiKartu() {
    try{
      const red = Number(this.$red.value);
      const sediste = Number(this.$sediste.value);

      if (!red || !sediste) {
        alert("Niste izabrali sediste");
        return;
      }

      const url = `https://localhost:7080/Ispit/KupiKartu/${this.projekcija.id}/${this.sala.id}/${red}/${sediste}`;
      const resp = await fetch(url, {method: "POST"});
      if (!resp.ok) throw new Error(await resp.text());

      const result = await resp.json();

      this.obeleziSelektovano(red, sediste, true);

      this.popuniFormu({ red: "", sediste: "", cena: "", sifra: "" });
      this.$btnKupi.disabled = true;

      alert(result.Poruka ?? "Karta uspesno kupljena");
    } catch(err) {
      console.error("Greška pri kupovini:", err);
      alert("Kupovina nije uspela: " + err.message);
    }
  }

  popuniFormu({ red = "", sediste = "", cena = "", sifra = "" }) {
    if (this.$red)     this.$red.value = red;
    if (this.$sediste) this.$sediste.value = sediste;
    if (this.$cena)    this.$cena.value = (cena ?? "") === "" ? "" : String(cena);
    if (this.$sifra)   this.$sifra.value = (sifra ?? "") === "" ? "" : String(sifra);
  }

  obeleziSelektovano(red, sediste, zauzeto) {
    this.$grid.querySelectorAll(".sediste.selektovano")
      .forEach(el => el.classList.remove("selektovano"));
    
    const seatEl = [...this.$grid.querySelectorAll(".sediste")]
      .find(el => Number(el.dataset.red) === red && Number(el.dataset.sediste) === sediste);

    if (seatEl) {
      seatEl.classList.add("selektovano");
      seatEl.classList.toggle("zauzeto", !!zauzeto);
      seatEl.classList.toggle("slobodno", !zauzeto);
    }
  }
}