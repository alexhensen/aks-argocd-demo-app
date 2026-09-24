namespace DemoApp;

/// <summary>
/// Deployment facts that Argo CD injects per environment, so a live page can prove
/// which pull request and commit it was built from.
/// </summary>
public sealed record BuildInfo(
    string EnvironmentName,
    string PullRequest,
    string CommitSha,
    string ImageTag,
    string Accent)
{
    public static BuildInfo FromEnvironment()
    {
        var environmentName = Read("ENVIRONMENT_NAME", "local");
        var pullRequest = Read("PULL_REQUEST", "-");
        return new BuildInfo(
            environmentName,
            pullRequest,
            Read("COMMIT_SHA", "unknown"),
            Read("IMAGE_TAG", "dev"),
            Read("ACCENT_COLOR", AccentFor(pullRequest)));
    }

    private static string Read(string key, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    /// <summary>
    /// Derives a stable colour per pull request so two preview environments are
    /// instantly distinguishable side by side on a projector. Permanent
    /// environments keep one fixed colour to contrast with every preview.
    /// </summary>
    private static string AccentFor(string pullRequest)
    {
        if (!int.TryParse(pullRequest, out var number))
        {
            return "#2563eb";
        }

        string[] palette = ["#db2777", "#16a34a", "#ea580c", "#7c3aed", "#0891b2"];
        return palette[Math.Abs(number) % palette.Length];
    }
}
