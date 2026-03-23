# ClinicalCompetence – starter (Angular + Bootstrap + ASP.NET Core + IIS)

Questa cartella è uno **starter** pronto per:
- UI in **Angular** + **Bootstrap**
- Backend in **ASP.NET Core Web API**
- Deploy su **IIS** (ASP.NET Core Hosting Bundle)

Include anche il tuo file originale per riferimento: `original/indexLS.html`.

---

## 1) Avvio in locale (sviluppo)

### Backend
```bash
cd ClinicalCompetence.Api
dotnet restore
dotnet run
```
- API: `https://localhost:5001`
- Swagger (dev): `https://localhost:5001/swagger`

### Frontend
```bash
cd ClinicalCompetence.Web
npm install
npm start
```

`proxy.conf.json` inoltra `/api` verso `https://localhost:5001`.

---

## 2) Build Angular + hosting dentro IIS (un solo sito)

1. Build Angular:
```bash
cd ClinicalCompetence.Web
npm run build
```

2. Copia il contenuto di:
`ClinicalCompetence.Web/dist/clinical-competence-web/`
dentro:
`ClinicalCompetence.Api/wwwroot/`

3. Pubblica l'API:
```bash
cd ClinicalCompetence.Api
dotnet publish -c Release -o publish
```

4. In IIS:
- Nuovo sito -> punta a `ClinicalCompetence.Api/publish`
- Application Pool: **No Managed Code**
- Installa sul server il **.NET Hosting Bundle** (necessario)

L'API servirà anche l'app Angular (SPA). Le rotte non-API vengono reindirizzate a `index.html` tramite:
`app.MapFallbackToFile("index.html");`

---

## 3) Prossimo step di migrazione dal tuo HTML

Nel tuo `indexLS.html` hai:
- stato UI (reparto selezionato, infermiere selezionato, modali)
- calcoli punteggi e sezioni
- Chart.js
- storage su localStorage

In Angular:
- DOM manipulation -> *binding* (template) + *state*
- localStorage -> API (poi DB)

Quando vuoi, posso:
1) creare i componenti "modale competenze" e "radar-chart" con Chart.js
2) mappare il tuo `COMPETENCE_DATA` in un file `competence-data.ts`
3) sostituire le funzioni di salvataggio con chiamate API reali.
