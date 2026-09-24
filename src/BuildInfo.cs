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
        return new BuildInfo(
            environmentName,
            Read("PULL_REQUEST", "-"),
            Read("COMMIT_SHA", "unknown"),
            Read("IMAGE_TAG", "dev"),
            Read("ACCENT_COLOR", AccentFor(environmentName)));
    }

    private static string Read(string key, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    /// <summary>
    /// Derives a stable colour from the environment name so two preview environments
    /// are instantly distinguishable side by side on a projector.
    /// </summary>
    private static string AccentFor(string environmentName)
    {
        string[] palette = ["#2563eb", "#16a34a", "#db2777", "#ea580c", "#7c3aed", "#0891b2"];
        var hash = 0;
        foreach (var c in environmentName)
        {
            hash = (hash * 31 + c) % 1000003;
        }

        return palette[Math.Abs(hash) % palette.Length];
    }
}
