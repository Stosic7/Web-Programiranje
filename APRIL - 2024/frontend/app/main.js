import { Application } from "./application.js";

const projekcije = await fetch("https://localhost:7080/Ispit/VratiProjekcije").then((response) => response.json());
const sale = await fetch("https://localhost:7080/Ispit/VratiSale").then((response) => response.json());

const projekcija = projekcije[0];
const sala = sale[2];

const app = new Application(projekcija, sala);
app.draw(document.body);