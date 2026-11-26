namespace OVRDisabler.Core.Models;

public record GameInstallationInfo(
    bool IsFound,
    string? InstallPath,
    string? ExecutablePath,
    string? FileVersion,
    string? ProductVersion,
    string? SteamAppId,
    string? SteamBuildId,
    string DetectionSource,
    string Message
);
