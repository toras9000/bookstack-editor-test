#r "nuget: Lestaly, 0.65.0"
#nullable enable
using Lestaly;
using Lestaly.Cx;

await Paved.RunAsync(config: c => c.AnyPause(), action: async () =>
{
    WriteLine("Stop service ...");
    var composeFile = ThisSource.RelativeFile("./docker/compose.yml");
    var bindFile = ThisSource.RelativeFile("./docker/volume-bind.yml");
    await "docker".args("compose", "--file", composeFile.FullName, "--file", bindFile.FullName, "down", "--remove-orphans");

    WriteLine("Delete volumes ...");
    var volumeDir = ThisSource.RelativeDirectory("./docker/volumes");
    volumeDir.DeleteRecurse();

    WriteLine("Completed.");
});
