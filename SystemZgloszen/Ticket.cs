using System.Text.RegularExpressions;

namespace SystemZgloszen;

public class Ticket
{
    private string _id;
    private TicketState _state = TicketState.UNASSIGNED;
    private bool _isClosureRequested;
    private DateTime _issueRegistrationDateTime;
    private string _issueDescription;
    private string _errorCode;
    private string _priority;
    private string _assigneeFirstName;
    private string _assigneeLastName;
    private string _issuerFirstName;
    private string _issuerLastName;
    private string _issuerContactNumber;
    private static readonly List<Ticket> _tickets = new List<Ticket>();
  
    
    public string IssuerFirstName => _issuerFirstName;
    public string IssuerLastName => _issuerLastName;
    public string IssuerContactNumber => _issuerContactNumber;
    public string IssueDescription => _issueDescription;
    
    public bool IsClosureRequested => _isClosureRequested;
    public string ErrorCode => _errorCode;
    public string Priority => _priority;
    public DateTime IssueRegistrationDateTime => _issueRegistrationDateTime;
    
    public TicketState State { get; private set; }
    public string AssigneeFirstName => _assigneeFirstName;
    public string AssigneeLastName => _assigneeLastName;

    public void RequestClosure()
    {
        _isClosureRequested = true;
    }

    public void AssignTicket(string firstName, string lastName)
    {
        _assigneeFirstName = firstName;
        _assigneeLastName = lastName;
        _state = TicketState.ASSIGNED;
    }

    public void CloseTicket()
    {
        State = TicketState.CLOSED;
        _isClosureRequested = false;
    }

    public void RejectTicketClosure()
    {
        State = TicketState.ASSIGNED;
        _isClosureRequested = false;
    }
    
    public override string ToString()
    {
        return $"{IssueRegistrationDateTime:dd-MM-yyyy} | {IssuerFirstName} {IssuerLastName} | {IssueDescription.Substring(0, 20)}...";
    }
    public Ticket(string issuerFirstName, string issuerSurname, string issuerContactNumber, string issueDescription,
    DateTime issueRegistrationDateTime, string errorCode)
    {
        _issuerFirstName = issuerFirstName;
        _issuerLastName = issuerSurname;
        _issuerContactNumber = issuerContactNumber;
        _issueDescription = issueDescription;
        _issueRegistrationDateTime = issueRegistrationDateTime;
        _errorCode = errorCode;
        _id = Guid.NewGuid().ToString();
        
    }
    
    public static void CreateTicket(Ticket ticket)
    {
        _tickets.Add(ticket);
    }
    
    public static List<Ticket> GetTickets()
    {
        return _tickets;
    }
    
}