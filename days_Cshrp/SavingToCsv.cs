using System;
using System.IO;


namespace SaveDataToCSV
{
    public class Save
    {
        private string _firstname = "";
        private string _lastname = "";
        private int _age;
        public Save(string firstname, string lastname, int age)
        {
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
        }

        public string Firstname
        {
            get
            {
                return _firstname.ToUpper();
            }
            set
            {
                if(string.IsNullOrEmpty(value) || value.Length < 2)
                {
                    throw new ArgumentException("Firstname must have a value and longer than 2 characters");
                }
                    _firstname = value.ToUpper();
            }
        }
        public string Lastname
        {
            get
            {
                return _lastname.ToUpper();
            }
            set
            {   
                if(string.IsNullOrEmpty(value) || value.Length < 2)
                {
                    throw new ArgumentException("Lastname must have a value and longer than 2 characters");
                }
                    _lastname = value.ToUpper();
            }
        }
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if(value < 0 || value > 120)
                {
                    throw new ArgumentException("Age must be between 0 and 120");
                }
                    _age = value;
            }
        }

        public override string ToString()
        {
            return $"Your firstname is: {Firstname} and you are: {Age} old and your lastname is: {Lastname}";
        }

        public static void PromptToSaveToCSV(Save save)
        {
            Console.WriteLine("Do you want to save this data to CSV? (Y/N): ");
            bool prompt = false;
            while(prompt != true)
            {
                string? input = Console.ReadLine();
                if(input?.ToLower() == "y")
                {
                    prompt = true;
                    WriteToCsv(save.Firstname, save.Lastname, save.Age);
                    Console.WriteLine("Data saved to CSV");
                }
                else if (input?.ToLower() == "n")
                {
                    prompt = true;
                    Console.WriteLine("Data not saved to CSV");
                }
                else
                {
                    Console.WriteLine("Enter y/n");
                }
            }
            
        }

        public static void PromptToReadFromCsv()
        {
                Console.WriteLine("Do you wish to read from the file ? (Y/N)");
                bool prompt = false;
                while(prompt != true)
                {
                    string? input = Console.ReadLine();
                    if(input?.ToLower() == "y")
                    {
                        prompt = true;
                        ReadFromCsv();
                        
                    }
                    else if(input?.ToLower() == "n")
                    {
                        prompt = true;
                        Console.WriteLine("Shutting down...");
                    }
                    else {
                        Console.WriteLine("Enter Y/N");
                    }
                }
        }
        public static void WriteToCsv(string firstname, string lastname, int age)
        {
            string filePath = "users.csv";
            bool fileExsists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if(!fileExsists)
                {
                    writer.WriteLine(string.Format("{0,-15},{1,-15},{2}", "Firstname", "Lastname", "Age"));
                }

                writer.WriteLine(string.Format("{0,-15},{1,-15},{2}", firstname, lastname, age));
            }
        }
        public static void ReadFromCsv()
        {
            string filePath = "users.csv";
            bool fileExsists = File.Exists(filePath);
            using (StreamReader reader = new StreamReader(filePath, true))
            {
                if(!fileExsists)
                {
                    Console.WriteLine("File doesn't have any data currently");
                    return;
                }
                reader.ReadLine();
                while(!reader.EndOfStream)
                {
                        string? line = reader.ReadLine();
                        if(line != null)
                        {
                            string[] fields = line.Split(',');
                            string firstname = fields[0].Trim();
                            string lastname = fields[1].Trim();
                            int age = int.Parse(fields[2].Trim());
                            Console.WriteLine($"Firstname: {firstname}, Lastname: {lastname}, Age: {age}");
                        }
                }
            }
        }
    }
}