using System.CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The CSV file to read",
            getDefaultValue: () => new FileInfo("Events.csv"));
        var todayOption = new Option<DateTime>(
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

        ListCommand.SetHandler((file, today, before, after) =>
        {
            ReadFile(file!, today, before, after);
        }, fileOption, todayOption, beforeOption, afterOption);

        AddCommand.SetHandler((file, date, category, description) =>
        {
            AddEvent(file!, date, category, description);
        }, fileOption, dateOption, categoryOption, descriptionOption);

        return await rootCommand.InvokeAsync(args);
    }
        internal static void ReadFile(FileInfo file, DateTime today, DateTime before, DateTime after)
        {
            var lines = File.ReadLines(file.FullName).ToList();
            foreach (string line in lines)
            {
                Console.WriteLine(line);
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
            writer.WriteLine($"{Environment.NewLine}{Environment.NewLine}{date}");
            writer.WriteLine($"{Environment.NewLine}{category}");
            writer.WriteLine($"{Environment.NewLine}{description}");
            writer.Flush();
        }
}