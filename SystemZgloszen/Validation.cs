using System.Text.RegularExpressions;

namespace SystemZgloszen;

public static class Validation
{
    
    private static readonly Regex contactNumberRegex = new Regex(@"^(\+?\d{2}\s?)?(\d{3}\s?){3}$");
    private static readonly Regex nameAndSurnameRegex = new Regex(@"^[A-ZŻŹĆĄŚĘŁÓŃ]{1}[a-zżźćńółęąś]{0,20}$");
    private static readonly Regex descriptionRegex = new Regex(@"(.|\n){20,300}");
    private static readonly Regex errorCodeRegex = new Regex(@"(.|\n){1,10}");
    
    public static string ValidateFirstName(string firstName)
    {
        if (!nameAndSurnameRegex.IsMatch(firstName))
        {
            throw new WrongTicketDataFormatException("Invalid first name. First name must start with uppercase letter and has" +
                                                     "to be between 1 and 20 characters.");
        }
        return firstName;
    }

    public static string ValidateLastName(string lastName)
    {
        if (!nameAndSurnameRegex.IsMatch(lastName))
        {
            throw new WrongTicketDataFormatException("Invalid last name. First name is invalid. Last name must start with uppercase letter and has" +
                                                     "to be between 1 and 20 characters. ");
        }
        return lastName;
    }
    
    public static string ValidateDescription(string description)
    {
        if (!descriptionRegex.IsMatch(description))
        {
            throw new WrongTicketDataFormatException("Invalid description. Description has to be at least 20 characters and less than 300.");
        }
        return description;
    }
    
    public static string ValidateContactNumber(string contactNumber)
    {
        if (!contactNumberRegex.IsMatch(contactNumber))
        {
            throw new WrongTicketDataFormatException("Invalid phone number.");
        }
        return contactNumber;
    }
    
    public static string ValidateErrorCode(string errorCode)
    {
        if (!errorCodeRegex.IsMatch(errorCode))
        {
            throw new WrongTicketDataFormatException("Invalid error code.");
        }
        return errorCode;
    }
    
}