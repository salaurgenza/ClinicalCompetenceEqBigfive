// tools_export_catalog.js
const fs = require("fs");
const path = require("path");

const input = process.argv[2];
if (!input) {
    console.error("Uso: node tools_export_catalog.js /percorso/indexLS.html");
    process.exit(1);
}

const outDir = path.join(process.cwd(), "wwwroot", "catalog");
fs.mkdirSync(outDir, { recursive: true });

const html = fs.readFileSync(input, "utf8");

// Estrae: const COMPETENCE_DATA = { ... };
const m = html.match(/const\s+COMPETENCE_DATA\s*=\s*({[\s\S]*?})\s*;/);
if (!m) {
    console.error("Non trovo `const COMPETENCE_DATA = {...};` nel file.");
    process.exit(1);
}

const objText = m[1];

// Valuta l'oggetto JS (è locale e tuo). Funziona anche con apici singoli.
let data;
try {
    data = Function('"use strict"; return (' + objText + ");")();
} catch (e) {
    console.error("Errore nel parsing di COMPETENCE_DATA:", e);
    process.exit(1);
}

// slug per filename
function slugify(s) {
    return s
        .normalize("NFD").replace(/[\u0300-\u036f]/g, "")
        .replace(/[^a-zA-Z0-9]+/g, "-")
        .replace(/^-+|-+$/g, "")
        .toLowerCase();
}

// Scrive 1 file per reparto
for (const reparto of Object.keys(data)) {
    const areas = data[reparto] || [];
    const payload = areas.map(a => ({
        key: a.key,
        questions: a.questions || []
    }));

    const file = path.join(outDir, `${slugify(reparto)}.json`);
    fs.writeFileSync(file, JSON.stringify(payload, null, 2), "utf8");
    console.log("Scritto:", file);
}

// Scrive anche una mappa reparto->file (utile al backend)
const map = {};
for (const reparto of Object.keys(data)) map[reparto] = `${slugify(reparto)}.json`;
fs.writeFileSync(path.join(outDir, `_map.json`), JSON.stringify(map, null, 2), "utf8");
console.log("Scritto:", path.join(outDir, "_map.json"));
