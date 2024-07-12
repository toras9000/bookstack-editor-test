#r "nuget: BookStackApiClient, 24.5.0-lib.1"
#r "nuget: Lestaly, 0.65.0"
#r "nuget: Kokuban, 0.2.0"
#nullable enable
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using BookStackApiClient;
using Kokuban;
using Lestaly;
using Lestaly.Cx;

// Displays a page of books accessible to API users.

await Paved.RunAsync(config: o => o.AnyPause(), action: async () =>
{
    // Prepare console
    using var outenc = ConsoleWig.OutputEncodingPeriod(Encoding.UTF8);
    using var signal = ConsoleWig.CreateCancelKeyHandlePeriod();

    // Detect service port
    var composeFile = ThisSource.RelativeFile("./docker/compose.yml");
    var pubPort = await "docker".args("compose", "--file", composeFile.FullName, "port", "app", "80").silent().result().success().output();
    var portNum = pubPort.AsSpan().SkipToken(':').TryParseNumber<ushort>() ?? throw new PavedMessageException("Cannot detect port number");

    // Show access address
    var serviceUrl = new Uri($"http://localhost:{portNum}");
    WriteLine($"Service URL : {serviceUrl}");

    // Create client 
    var apiToken = "00001111222233334444555566667777";
    var apiSecret = "88889999aaaabbbbccccddddeeeeffff";
    var apiEntry = new Uri(serviceUrl, "/api/");
    using var client = new BookStackClient(apiEntry, apiToken, apiSecret);

    // Get a list of pages
    WriteLine($"Get pages");
    var pages = await client.ListPagesAsync(new(count: 500), signal.Token);
    foreach (var book in pages.data.GroupBy(p => p.book_id))
    {
        WriteLine($"  BookID={book.Key}");
        foreach (var page in book.OrderBy(p => p.priority))
        {
            WriteLine($"  Page={page.name}, editor={page.editor}");
        }
    }

});