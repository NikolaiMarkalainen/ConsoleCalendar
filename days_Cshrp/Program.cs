using System.CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "list",
            description: "Lists the contents of the csv.",
            isDefault: true,
            parseArgument: result =>
            {
                if(result.Tokens.Count == 0)
                {
                    return new FileInfo("Events.csv");
                }
                string? filePath = result.Tokens.Single().Value;
                if(!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exsist";
                    return null;
                }
                else
                {
                    return new FileInfo(filePath);
                }
            });

        var todayOption = new Option<DateTime>(
            name: "--today",
            description: "Lists all events that are marked today.");
        
        var beforeOption = new Option<DateTime>(
            name: "--before",
            description: "Lists all events prior to today.");
        
        var afterOption = new Option<DateTime>(
           name: "--after",
           description: "List all events in the future." );

        var dateArgument = new Argument<DateTime>(
            name: "--date",
            description: "An event to set a calendar appointment");
        
        var categoryArgument = new Argument<String>(
            name: "--category",
            description: "A description field for calendar appointments");

        var descriptionArgument = new Argument<String>(
            name: "--description",
            description: "A way to add a description to the calendar");


        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddGlobalOption(fileOption);

        var DaysCommand = new Command("days", "Initial command to start file")
        {
            fileOption,
            todayOption,
            beforeOption,
            afterOption
        };

        var AddCommand = new Command("add", "Add an entry to the file");
        AddCommand.AddArgument(dateArgument);
        AddCommand.AddArgument(categoryArgument);
        AddCommand.AddArgument(descriptionArgument);

        DaysCommand.SetHandler((file, today, before, after) =>
        {
            ReadFile(file!, today, before, after);
        }, fileOption, todayOption, beforeOption, afterOption);

        AddCommand.SetHandler((file, date, category, description) =>
        {
            AddEvent(file!, date, category, description);
        }, fileOption, dateArgument, categoryArgument, descriptionArgument);

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
            Console.WriteLine("Adding to file");
            using StreamWriter? writer = file.AppendText();
            writer.WriteLine($"{Environment.NewLine}{Environment.NewLine}{date}");
            writer.WriteLine($"{Environment.NewLine}{category}");
            writer.WriteLine($"{Environment.NewLine}{description}");
            writer.Flush();
        }
}