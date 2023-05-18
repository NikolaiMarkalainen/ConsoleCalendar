
import org.apache.commons.cli.*;
import java.util.Date;
import java.time.LocalDate;

//>     javac -cp ../lib/commons-cli-1.5.0/commons-cli-1.5.0.jar App.java
//>     java -cp ../lib/commons-cli-1.5.0/commons-cli-1.5.0.jar App.java -date

// java -cp .:../lib/commons-cli-1.5.0/commons-cli-1.5.0.jar App


// compile file > javac -cp ../lib/commons-cli-1.5.0/commons-cli-1.5.0.jar *.java                  

// test runs > java -cp .:../lib/commons-cli-1.5.0/commons-cli-1.5.0.jar App -days -list --today

public class App {
    public static void main(String[] args) {   
        Options options = new Options();  

        CalendarManager calendarManager = new CalendarManager();
        Verification Verification = new Verification();

        Option daysOption = new Option("days", false, "Initiate program execution");
        Option addOption = new Option("add", false, "Use add to create new Calendar points");
        Option listOption = new Option("list", false, "Use List to display contents of the file");
        Option removeOption = new Option("delete", false, "Use delete to remove contents from calendar");
        daysOption.setRequired(true);

        options.addOption(daysOption);
        options.addOption(addOption);
        options.addOption(listOption);
        options.addOption(removeOption);

        Option fileOption = Option.builder()
        .longOpt("file")
        .type(String.class)
        .desc("file location")
        .build();

        Option todayOption = Option.builder()
        .longOpt("today")
        .type(Boolean.class)
        .desc("Check for today")
        .build();

        Option noCategory = Option.builder()
        .longOpt("no-category")
        .type(Boolean.class)
        .desc("Filter out categories")
        .build();
        
        Option allOption = Option.builder()
        .longOpt("all")
        .type(Boolean.class)
        .desc("Affect all fields in data")
        .build();
        
        Option testOption = Option.builder()
        .longOpt("dry-run")
        .type(Boolean.class)
        .desc("For testing purposes")
        .build();

        Option afterOption = Option.builder()
        .longOpt("after-date")
        .hasArg()
        .type(Date.class)
        .desc("Check for date after")
        .build();

        Option beforeOption = Option.builder()
        .longOpt("before-date")
        .hasArg()
        .type(Date.class)
        .desc("Check for date before")
        .build();

        Option dateOption = Option.builder()
        .longOpt("date")
        .hasArg()
        .type(Date.class)
        .desc("Check for specific date")
        .build();

        Option categoryOption = Option.builder()
        .longOpt("category")
        .hasArg()
        .type(String.class)
        .desc("Category field")
        .build();

        Option descriptionOption = Option.builder()
        .longOpt("description")
        .hasArg()
        .type(String.class)
        .desc("Description field")
        .build();

        options.addOption(descriptionOption);
        options.addOption(categoryOption);
        options.addOption(dateOption);
        options.addOption(beforeOption);
        options.addOption(afterOption);
        options.addOption(testOption);
        options.addOption(allOption);
        options.addOption(noCategory);
        options.addOption(todayOption);
        options.addOption(fileOption);
        
        CommandLineParser parser = new DefaultParser();

        String header = "Calendar application";
        String footer = "Issues";
        HelpFormatter formatter = new HelpFormatter();
        
        try{
            CommandLine cmd = parser.parse(options, args);   
            String fileName;

            if(cmd.hasOption("file")){
                fileName = cmd.getOptionValue("file");
            } else {
                fileName = "Events.csv";
            }

            boolean hasRemoveOption = cmd.hasOption("delete");
            boolean hasAddOption = cmd.hasOption("add");
            boolean hasListOption = cmd.hasOption("list");

            if (!hasRemoveOption && !hasAddOption && !hasListOption) {
                System.err.println("\n At least one of -remove, -add, or -list options is required. \n");
                formatter.printHelp("Calendar application", options);
                System.exit(1);
            }
            if(hasAddOption){
                
                String dateString = cmd.getOptionValue("date");
                LocalDate date = null;

                if(dateString != null){
                    date = Verification.DateVerification(dateString);
                }

                String category = cmd.getOptionValue("category");
                if(category != null){
                    category = Verification.StringVerification(category);
                }

                String description = cmd.getOptionValue("description");
                if(description != null){
                    description = Verification.StringVerification(description);
                }

                calendarManager.addToCalendar(date, fileName, category, description);
            }

            else if(hasRemoveOption){
                String dateString = cmd.getOptionValue("date");
                LocalDate date = null;
                
                if(dateString != null){
                    date = Verification.DateVerification(dateString);
                }

                String afterString = cmd.getOptionValue("after-date");
                LocalDate after = null;

                if(afterString != null){
                    after = Verification.DateVerification(afterString);
                }

                String beforeString = cmd.getOptionValue("before-date");
                LocalDate before = null;

                if(beforeString != null){
                    before = Verification.DateVerification(beforeString);
                }
                
                String category = cmd.getOptionValue("category");
                if(category != null){
                    category = Verification.StringVerification(category);
                }

                String description = cmd.getOptionValue("description");
                if(description != null){
                    description = Verification.StringVerification(description);
                }
                
                Boolean today = cmd.hasOption("today");
                Boolean all = cmd.hasOption("all");
                Boolean noneCategory = cmd.hasOption("no-category");
                Boolean dryRun = cmd.hasOption("dry-run");

                calendarManager.removeFromCalendar(date, fileName, category, description, after, before, today, all, noneCategory, dryRun);
            }
            else if(hasListOption){
                String dateString = cmd.getOptionValue("date");
                LocalDate date = null;
                
                if(dateString != null){
                    date = Verification.DateVerification(dateString);
                }

                String afterString = cmd.getOptionValue("after-date");
                LocalDate after = null;

                if(afterString != null){
                    after = Verification.DateVerification(afterString);
                }

                String beforeString = cmd.getOptionValue("before-date");
                LocalDate before = null;

                if(beforeString != null){
                    before = Verification.DateVerification(beforeString);
                }
                
                String category = cmd.getOptionValue("category");
                if(category != null){
                    category = Verification.StringVerification(category);
                }

                String description = cmd.getOptionValue("description");
                if(description != null){
                    description = Verification.StringVerification(description);
                }
                

                Boolean today = cmd.hasOption("today");
                Boolean noneCategory = cmd.hasOption("no-category");

                calendarManager.readFromCalendar(date, fileName, category, description, after, before, today, noneCategory);
            }
        }
        catch(Exception e){
            formatter.printHelp("Calendar application \n" ,header, options, footer + "\n" + e.getMessage(), true);
        }
    }

}
