import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.io.FileWriter;
import java.io.FileReader;
import java.nio.file.Files;
import java.nio.file.Path;

public class CalendarManager {

    public void addToCalendar(LocalDate date, String fileName,String category, String description){
        if (fileName == null) {
            throw new IllegalArgumentException("File name cannot be null");
        }

        if (category == null) {
            throw new IllegalArgumentException("Category cannot be null");
        }

        if (description == null) {
            throw new IllegalArgumentException("Description cannot be null");
        }

        try (BufferedWriter writer = new BufferedWriter(new FileWriter(fileName, true))) {
            String formattedDate = date.format(DateTimeFormatter.ofPattern("yyyy-MM-dd"));
            writer.write(formattedDate + ",");
            writer.write(category + ",");
            writer.write(description);
            writer.newLine();
        } catch (IOException e) {
            System.err.println("Failed to write to file: " + e.getMessage());
        }
    }


    public void removeFromCalendar(LocalDate date, String fileName, String category, String description, LocalDate after,
    LocalDate before, Boolean today, Boolean all, Boolean noCategory, Boolean dryRun){
        if (fileName == null) {
            throw new IllegalArgumentException("File name cannot be null");
        }

        List<String> lines;
        try {
            lines = Files.readAllLines(Path.of(fileName));
        } catch (IOException e) {
            System.err.println("Failed to read from file: " + e.getMessage());
            return;
        }

        List<String> updatedLines = new ArrayList<>();
        List<String> deletedEvents = new ArrayList<>();

        for (String line : lines) {
            String[] fields = line.split(",");
            LocalDate eventDate = LocalDate.parse(fields[0], DateTimeFormatter.ofPattern("yyyy-MM-dd"));
            String eventCategory = fields[1];
            String eventDescription = fields[2];

            boolean shouldDelete = false;

            if (today && eventDate.isEqual(LocalDate.now())) {
                shouldDelete = true;
            } else if (date != null && eventDate.isEqual(date)) {
                shouldDelete = true;
            } else if (before != null && eventDate.isBefore(before)) {
                shouldDelete = true;
            } else if (after != null && eventDate.isAfter(after)) {
                shouldDelete = true;
            } else if (category != null && eventCategory.equals(category)) {
                shouldDelete = true;
            } else if (description != null && eventDescription.equals(description)) {
                shouldDelete = true;
            } else if (all) {
                shouldDelete = true;
            } else if (noCategory && eventCategory.isEmpty()) {
                shouldDelete = true;
            }

            if (shouldDelete) {
                deletedEvents.add(line);
            } else {
                updatedLines.add(line);
            }
        }

        if (!deletedEvents.isEmpty()) {
            System.out.println("Deleted " + deletedEvents.size() + " events:");
            for (String event : deletedEvents) {
                String[] fields = event.split(",");
                LocalDate eventDate = LocalDate.parse(fields[0], DateTimeFormatter.ofPattern("yyyy-MM-dd"));
                String eventCategory = fields[1];
                String eventDescription = fields[2];

                System.out.println("Date: " + eventDate.format(DateTimeFormatter.ofPattern("yyyy-MM-dd")));
                System.out.println("Category: " + eventCategory);
                System.out.println("Description: " + eventDescription);
                System.out.println();
            }
        } else {
            System.out.println("No events were deleted.");
        }
    }

    public void readFromCalendar(LocalDate date, String fileName, String category, String description, LocalDate after,
    LocalDate before, Boolean today, Boolean noCategory){
        if (fileName == null) {
            throw new IllegalArgumentException("File name cannot be null");
        }

        List<String> events = new ArrayList<>();

        try (BufferedReader reader = new BufferedReader(new FileReader(fileName))) {
            String line;
            while ((line = reader.readLine()) != null) {
                String[] fields = line.split(",");
                LocalDate eventDate = LocalDate.parse(fields[0], DateTimeFormatter.ofPattern("yyyy-MM-dd"));
                String eventCategory = fields[1];
                String eventDescription = fields[2];

                if (date != null && !eventDate.isEqual(date)) {
                    continue;
                }

                if (today && !eventDate.isEqual(LocalDate.now())) {
                    continue;
                }

                if (before != null && eventDate.isAfter(before)) {
                    continue;
                }

                if (after != null && eventDate.isBefore(after)) {
                    continue;
                }

                if (category != null && !eventCategory.equals(category)) {
                    continue;
                }

                if (description != null && !eventDescription.equals(description)) {
                    continue;
                }

                if (noCategory && eventCategory != null) {
                    continue;
                }

                events.add(line);
            }
        } catch (IOException e) {
            System.err.println("Failed to read from file: " + e.getMessage());
        }

        if (date != null) {
            System.out.println("Events on " + date.format(DateTimeFormatter.ofPattern("yyyy-MM-dd")) + ":");
        } else {
            System.out.println(LocalDate.now());
            System.out.println(today);
            System.out.println("All events:");
        }

        for (String event : events) {
            System.out.println(event);
        }
    }
}