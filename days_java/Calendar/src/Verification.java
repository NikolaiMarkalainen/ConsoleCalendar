
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;

public class Verification {
    
    public static LocalDate DateVerification (String dateString) {    
        try{
            LocalDate date = null;
            DateTimeFormatter dateFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd");
            date = LocalDate.parse(dateString, dateFormatter);
            return date;
        } catch (DateTimeParseException e) {
            System.err.println("Failed to parse the date: " + e.getMessage());
        }
        return null;
    }

    public static String StringVerification (String input) {
        if(input != null && !input.isEmpty()){
            return input;
        } else {
            return null;
        }
    }
}
