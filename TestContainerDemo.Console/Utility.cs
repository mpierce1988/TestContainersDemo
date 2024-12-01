namespace TestContainerDemo.Console;

public static class Utility
{
    public static bool AskYesNoQuestion(string question)
    {
        bool validResponse = false;
        bool result = false;
        
        while (!validResponse)
        {
            System.Console.WriteLine(question);
            System.Console.WriteLine("Please enter 'y' for Yes or 'n' for No:");
            var response = System.Console.ReadLine();
            
            if (response is not null && (response.ToLower() == "y" || response.ToLower() == "n"))
            {
                validResponse = true;
                
                result = response?.ToLower() == "y";
            }
            else
            {
                System.Console.WriteLine("Invalid response. Please enter 'y' for Yes or 'n' for No:");
            }
        }


        return result;
    }
    
}