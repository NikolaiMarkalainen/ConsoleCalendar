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
            description: "Lists all events that are marked today."
            );

        var beforeOption = new Option<DateTime>(
            name: "--before-date",
            description: "Lists all events prior to today."
            );
        
        var afterOption = new Option<DateTime>(
           name: "--after-date",
           description: "List all events in the future." 
           );

        var dateOption = new Option<DateTime>(
            name: "--date",
            description: "An event to set a calendar appointment"
            );
        
        var categoryOption = new Option<String>(
            name: "--category",
            description: "A description field for calendar appointments"
            );

        var descriptionOption = new Option<String>(
            name: "--description",
            description: "A way to add a description to the calendar"
            );
        
        var allOption = new Option <bool>(
            name: "--all",
            description: "Will affect all fields in the calendar"
        );

        //  not implemented
        var testOption = new Option <bool>(
            name: "dry-run",
            description: "For testing purposes to try out deletion"
        );


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

        var DeleteCommand = new Command("delete", "A command for deleting the")
        {
            fileOption,
            todayOption,
            beforeOption,
            afterOption,
            dateOption,
            categoryOption,
            descriptionOption,
            allOption,
            testOption
        };

        var DaysCommand= new Command("days", "Initial command to start the file")
        {
            ListCommand,
            AddCommand,
            DeleteCommand
        };


        var rootCommand = new RootCommand("Calendar management system");
        rootCommand.AddCommand(ListCommand);
        rootCommand.AddCommand(DaysCommand);
        rootCommand.AddCommand(AddCommand);
        rootCommand.AddCommand(DeleteCommand);


        DaysCommand.SetHandler((args) =>
        {
            Console.WriteLine("Use add, list or delete for further usage");
        });

        ListCommand.SetHandler((file, today, before, after, date, category, description) =>
        {
            ReadFile(file!, today!, before, after, date, category, description);
        }, fileOption, todayOption, beforeOption, afterOption, dateOption, categoryOption, descriptionOption);

        AddCommand.SetHandler((file, date, category, description, today) =>
        {
            AddEvent(file!, date, category, description, today);
        }, fileOption, dateOption, categoryOption, descriptionOption, todayOption);

        DeleteCommand.SetHandler((file, date, before, after, today, category, description, all) =>
        {
            DeleteEvent(file!, date, before, after, today, category, description, all);
        }, fileOption, dateOption, beforeOption, afterOption, todayOption, categoryOption, descriptionOption, allOption); 
        
        return await rootCommand.InvokeAsync(args);
    }
    internal static void ReadFile(FileInfo file, bool today, DateTime before, DateTime after, DateTime? date, string cat, string desc)
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

            if (date != DateTime.MinValue)
            {
                if (eventDate.Date == date?.Date)
                {
                    events.Add((eventDate, category, description));
                }
                continue;
            }

            if (before.Date != DateTime.MinValue)
            {
                if(eventDate.Date < before.Date)
                {
                    events.Add((eventDate, category, description));
                }
                continue;
            }

            if (after.Date != DateTime.MinValue)
            {
                if(eventDate.Date > after.Date)
                {
                    events.Add((eventDate, category, description));
                }
                continue;
            }
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


    internal static void AddEvent(FileInfo file, DateTime date, string category, string description, bool today)
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
            
            if(date == DateTime.MinValue && today == false || today == true)
            {
                date = DateTime.Today;
            }
                
            Console.WriteLine("Adding to file");
            using StreamWriter writer = file.AppendText();
            writer.Write($"{date.ToString("yyyy-MM-dd")},");
            writer.Write($"{category},");
            writer.WriteLine($"{description}");
            writer.Flush();
    }
    internal static void DeleteEvent(FileInfo file, DateTime? date, DateTime before, DateTime after, bool today, string cat, string desc, bool all)
    {
        var lines = File.ReadAllLines(file.FullName);
        var deletedEvents = new List<(DateTime, string, string)>();

        if (all)
            {
                deletedEvents.AddRange(lines.Select(line =>
                {
                    string[] fields = line.Split(',');
                    DateTime eventDate = DateTime.Parse(fields[0]);
                    string category = fields[1];
                    string description = fields[2];
                    return (eventDate, category, description);
                }));
            }
        foreach (string line in lines)
        {
            string[] fields = line.Split(',');
            DateTime eventDate = DateTime.Parse(fields[0]);
            string category = fields[1];
            string description = fields[2];
            bool shouldDelete = false;

            if (today && eventDate.Date == DateTime.Today)
            {
                shouldDelete = true;
            }
            else if (date.HasValue && date.Value.Date == eventDate.Date)
            {
                shouldDelete = true;
            }
            else if (before != DateTime.MinValue && eventDate.Date < before.Date)
            {
                shouldDelete = true;
            }
            else if (after != DateTime.MinValue && eventDate.Date > after.Date)
            {
                shouldDelete = true;
            }
            else if (!string.IsNullOrEmpty(cat) && category == cat)
            {
                shouldDelete = true;
            }
            else if (!string.IsNullOrEmpty(desc) && description == desc)
            {
                shouldDelete = true;
            }

            if (shouldDelete)
            {
                deletedEvents.Add((eventDate, category, description));
            }
        }
        var updatedLines = lines.Where(line =>
        {
            string[] fields = line.Split(',');
            DateTime eventDate = DateTime.Parse(fields[0]);
            string category = fields[1];
            string description = fields[2];
            return !deletedEvents.Contains((eventDate, category, description));
            }).ToList();

            File.WriteAllLines(file.FullName, updatedLines);
            if (deletedEvents.Count > 0)
            {
                Console.WriteLine($"Deleted {deletedEvents.Count} events:");
                foreach (var e in deletedEvents.OrderBy(x => x.Item1))
                {
                    Console.WriteLine($"Date: {e.Item1.ToString("yyyy-MM-dd")}");
                    Console.WriteLine($"Category: {e.Item2}");
                    Console.WriteLine($"Description: {e.Item3}\n");
                }
            }
            else
            {
                Console.WriteLine("No events were deleted.");
            }
    } 
}   