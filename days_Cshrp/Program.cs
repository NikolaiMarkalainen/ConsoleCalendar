using System.CommandLine;
using System.Globalization;
class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The CSV file to read",
            getDefaultValue: () => new FileInfo("Events.csv"));

        
        var todayOption = new Option<bool>(
            name: "--today",
            description: "Lists all events that are marked today.");

        var beforeOption = new Option<DateTime>(
            name: "--before",
            description: "Lists all events prior to today.");
        
        var afterOption = new Option<DateTime>(
           name: "--after",
           description: "List all events in the future." );

        var dateOption = new Option<DateTime>(
            name: "--date",
            description: "An event to set a calendar appointment");
        
        var categoryOption = new Option<String>(
            name: "--category",
            description: "A description field for calendar appointments");

        var descriptionOption = new Option<String>(
            name: "--description",
            description: "A way to add a description to the calendar");

        todayOption.IsRequired = false;


        var ListCommand = new Command("list", "Listing contents of the file")
        {
            todayOption,
            beforeOption,
            afterOption,
            fileOption,
            dateOption,
            categoryOption,
            descriptionOption,
            
        };

        var AddCommand = new Command("add", "Add an entry to the file")
        {
            dateOption,
            categoryOption,
            descriptionOption,
            fileOption
        };

        var DaysCommand= new Command("days", "Initial command to start the file")
        {
            ListCommand,
            AddCommand
        };

        var rootCommand = new RootCommand("sample");
        rootCommand.AddCommand(ListCommand);
        rootCommand.AddCommand(DaysCommand);
        rootCommand.AddCommand(AddCommand);


        DaysCommand.SetHandler((args) =>
        {
            Console.WriteLine("Use add or list for further usage");
        });

        ListCommand.SetHandler((file, today, before, after, date) =>
        {
            ReadFile(file!, today!, before, after, date);
        }, fileOption, todayOption, beforeOption, afterOption, dateOption);

        AddCommand.SetHandler((file, date, category, description) =>
        {
            AddEvent(file!, date, category, description);
        }, fileOption, dateOption, categoryOption, descriptionOption);

        return await rootCommand.InvokeAsync(args);
    }
internal static void ReadFile(FileInfo file, bool today, DateTime before, DateTime after, DateTime? date)
{
    var lines = File.ReadLines(file.FullName).ToList();
    List<(DateTime, string, string)> events = new List<(DateTime, string, string)>();
    foreach (string line in lines)
    {
        string[] fields = line.Split(',');
        DateTime eventDate = DateTime.Parse(fields[0]);
        string category = fields[1];
        string description = fields[2];
        if(date == DateTime.MinValue && today == false && before == DateTime.MinValue && after == DateTime.MinValue)
        {
            events.Add((eventDate, category, description));
        }
        if(today == true && eventDate.Date == DateTime.Today)
        {
            events.Add((eventDate, category, description));
        }

        if (date.HasValue && eventDate.Date != date.Value.Date)
        {
            continue;
        }

        if (before != default && eventDate.Date >= before.Date)
        {
            continue;
        }

        if (after != default && eventDate.Date <= after.Date)
        {
            continue;
        }

        events.Add((eventDate, category, description));
    }

    if (date.HasValue && date != DateTime.MinValue)
    {
        Console.WriteLine($"Events on {date.Value.ToString("yyyy-MM-dd")}:");
    }
    else
    {
        Console.WriteLine("All events:");
    }
    foreach (var e in events.OrderBy(x => x.Item1))
    {
        Console.WriteLine($"Date: {e.Item1.ToString("yyyy-MM-dd")}");
        Console.WriteLine($"Category: {e.Item2}");
        Console.WriteLine($"Description: {e.Item3}\n");
    }
}

        internal static void AddEvent(FileInfo file, DateTime date, string category, string description)
        {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
                }

                if (description == null)
                {
                    throw new ArgumentNullException(nameof(description));
                }
                
            Console.WriteLine("Adding to file");
            using StreamWriter writer = file.AppendText();
            writer.Write($"{date.ToString("yyyy-MM-dd")},");
            writer.Write($"{category},");
            writer.WriteLine($"{description}");
            writer.Flush();
        }
}