using System.Collections.Concurrent;
using DemoApp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CountryStore>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var buildInfo = BuildInfo.FromEnvironment();

app.MapGet("/api/info", () => buildInfo);

app.MapGet("/api/countries", (CountryStore store) => store.All());

app.MapPost("/api/countries", (CountryStore store, CountryInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Name))
    {
        return Results.BadRequest(new { error = "Name is required." });
    }

    var country = store.Add(input.Name.Trim());
    return Results.Created($"/api/countries/{country.Id}", country);
});

app.MapDelete("/api/countries/{id:guid}", (CountryStore store, Guid id) =>
    store.Remove(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));
app.MapGet("/readyz", () => Results.Ok(new { status = "ready" }));

app.Run();

internal sealed record CountryInput(string Name);

internal sealed record Country(Guid Id, string Name, DateTimeOffset AddedAt);

/// <summary>
/// Deliberately in-memory: every preview environment starts from the same seeded data,
/// which makes it obvious during a demo that environments do not share state.
/// </summary>
internal sealed class CountryStore
{
    private readonly ConcurrentDictionary<Guid, Country> _countries = new();

    public CountryStore()
    {
        foreach (var name in new[] { "Nederland", "België", "Luxemburg" })
        {
            Add(name);
        }
    }

    public IReadOnlyCollection<Country> All() =>
        _countries.Values.OrderBy(c => c.AddedAt).ToList();

    public Country Add(string name)
    {
        var country = new Country(Guid.NewGuid(), name, DateTimeOffset.UtcNow);
        _countries[country.Id] = country;
        return country;
    }

    public bool Remove(Guid id) => _countries.TryRemove(id, out _);
}
