namespace OVRDisabler.Core.Models;

public record DisablerOptions(
    bool DisableForm = true,
    bool DisableMorale = true,
    bool DisableMatchFitness = true
);
