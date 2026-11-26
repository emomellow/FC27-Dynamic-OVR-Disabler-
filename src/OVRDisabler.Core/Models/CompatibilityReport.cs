namespace OVRDisabler.Core.Models;

public record CompatibilityReport(
    GameCompatibilityStatus Status,
    string StatusTurkish,
    string ExecutableVersion,
    string ProductVersion,
    string? SteamBuildId,
    string TargetGameTitle,
    string TargetUpdate,
    bool CanDirectDeploy,
    string ExplanationTurkish
);
