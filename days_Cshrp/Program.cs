using System;
namespace SaveDataToCSV;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter your firstname: ");
        string? fname = Console.ReadLine();
        Console.WriteLine("Enter your lastname");
        string? lname = Console.ReadLine();
        Console.WriteLine("Enter your age");
        string? ageInput = Console.ReadLine();
        int age;    
        
        if(!int.TryParse(ageInput, out age))
        {
            throw new ArgumentException("Invalid input. Please enter a valid number");
        }
        if(string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname))
        {
            throw new ArgumentException("Name is not in correct format");
        }

        Save test = new Save(fname, lname, age);
        Console.WriteLine(test);
        Save.PromptToSaveToCSV(test);
        Save.PromptToReadFromCsv();
    }
}

