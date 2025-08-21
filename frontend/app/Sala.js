export class Sala {
    constructor(id, naziv, ukupanBrojRedova, ukupanBrojSedista) {
        this.id = id;
        this.naziv = naziv;
        this.ukupanBrojRedova = ukupanBrojRedova;
        this.ukupanBrojSedista = ukupanBrojSedista;
    }

    get sedistaPoRed() {
        return Math.floor(this.ukupanBrojSedista / this.ukupanBrojRedova);
    }
}