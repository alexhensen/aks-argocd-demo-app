# aks-argocd-demo-app

Demo applicatie voor een presentatie over Argo CD en ephemeral environments.
Een .NET minimal API met een dashboardpagina die laat zien *welke* omgeving je
voor je hebt: omgevingsnaam, pull request nummer, commit en pod.

De accentkleur wordt afgeleid uit de omgevingsnaam, zodat twee previews naast
elkaar op een scherm direct van elkaar te onderscheiden zijn.

## Endpoints

| Endpoint | Doel |
|---|---|
| `/` | Dashboardpagina |
| `/api/info` | Omgevingsgegevens |
| `/api/countries` | Lijst, toevoegen en verwijderen |
| `/healthz`, `/readyz` | Probes |

De lijst met landen staat in het geheugen van de pod. Dat is bewust: het maakt
in een demo zichtbaar dat elke preview zijn eigen staat heeft en dat die staat
samen met de pull request verdwijnt.

## Lokaal draaien

```bash
dotnet run --project src/DemoApp.csproj
```

## Preview omgevingen

Een pull request krijgt een eigen omgeving zodra de build slaagt. De workflow
zet daarna het label `preview`, waarop Argo CD reageert. De URL verschijnt als
comment op de pull request.

De uitrol zelf staat in [aks-argocd-demo-gitops](https://github.com/alexhensen/aks-argocd-demo-gitops).
