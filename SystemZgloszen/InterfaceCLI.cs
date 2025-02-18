using System.Dynamic;
using System.Text.RegularExpressions;

namespace SystemZgloszen
{
    public class InterfaceCLI
    {
        private UserType _userType = UserType.USER;
        private string _userFirstName = string.Empty;
        private string _userLastName = string.Empty;
        private string _password = string.Empty;


        public void Run()
        {
            Ticket ticket = new Ticket("Jan", "Kowalski", "999888771", "bardzo długi opis problemu", DateTime.Now, "1");
            Ticket ticket2 = new Ticket("Jan", "Kowalski", "999888772", "bardzo długi opis problemu", DateTime.Now, "2");
            Ticket ticket3 = new Ticket("Jan", "Kowalski", "999888773", "bardzo długi opis problemu", DateTime.Now, "3");
            ticket.AssignTicket("Przemysław", "Kowalski");
            ticket2.AssignTicket("Przemysław", "Kowalski");
            ticket3.AssignTicket("Przemysław", "Kowalski");
            Ticket.CreateTicket(ticket);
            Ticket.CreateTicket(ticket2);
            Ticket.CreateTicket(ticket3);
            Console.WriteLine(Ticket.GetTickets());
            PromptLogin();
        }
        private void PromptLogin()
        {

                while (true)
                {
                    try
                    {
                    Console.Clear();
                    Console.WriteLine("=== System Zgłoszeń Firmy ===");
                    Console.WriteLine("Zaloguj się:");
                    Console.Write("Imię: ");
                    _userFirstName = Validation.ValidateFirstName(Console.ReadLine());
                    Console.Write("Nazwisko: ");
                    _userLastName = Validation.ValidateLastName(Console.ReadLine());
                    Console.Write("Hasło: ");
                    _password = ReadPassword();

                    if (_userFirstName == "Andrzej" && _userLastName.Equals("Kowalski") && _password == "admin")
                        _userType = UserType.ADMIN;
                    else if (_userFirstName == "Przemysław" && _userLastName.Equals("Kowalski") && _password == "poweruser")
                        _userType = UserType.POWERUSER;
                    else
                        _userType = UserType.USER;

                    break;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Click any key to continue");
                        Console.ReadKey();
                    }

                }

                OpenHomeScreen();

        }

        private string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        private void OpenHomeScreen()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Witaj, {_userFirstName}!");
                Console.WriteLine("Jaką operację chciałbyś wykonać?");

                switch (_userType)
                {
                    case UserType.USER:
                        Console.WriteLine("1) Utwórz ticket");
                        Console.WriteLine("2) Sprawdź utworzone tickety");
                        Console.WriteLine("3) Wyloguj");
                        break;

                    case UserType.POWERUSER:
                        Console.WriteLine("1) Otwórz listę przypisanych Tobie ticketów");
                        Console.WriteLine("2) Wnioskuj o zamknięcie ticketu");
                        Console.WriteLine("3) Wyloguj");
                        break;

                    case UserType.ADMIN:
                        Console.WriteLine("1) Otwórz listę ticketów");
                        Console.WriteLine("2) Przypisz ticket");
                        Console.WriteLine("3) Sprawdź listę żądań");
                        Console.WriteLine("4) Wyloguj");
                        break;
                }

                Console.Write("Twój wybór: ");
                var userInput = Console.ReadLine();
                if (HandleHomeScreenInput(userInput)) break;
            }
        }

        private bool HandleHomeScreenInput(string userInput)
        {
            switch (_userType)
            {
                case UserType.USER:
                    if (userInput == "1")
                        CreateTicket();
                    else if (userInput == "2")
                        OpenTicketList();
                    else if (userInput == "3")
                        PromptLogin();
                    break;

                case UserType.POWERUSER:
                    if (userInput == "1")
                        OpenTicketList();
                    else if (userInput == "2")
                        RequestTicketClosure();
                    else if (userInput == "3")
                        PromptLogin();
                    break;

                case UserType.ADMIN:
                    if (userInput == "1")
                        OpenTicketList();
                    else if (userInput == "2")
                        AssignTicket();
                    else if (userInput == "3")
                        CheckRequests();
                    else if (userInput == "4")
                        PromptLogin();
                    break;
            }

            return false;
        }

        private void CreateTicket()
        {
                try
                {
                    Console.Clear();
                    Console.WriteLine("=== Utwórz Ticket ===");

                    Console.Write("Podaj opis problemu (od 20 do 300 znaków) : ");
                    string description = Validation.ValidateDescription(Console.ReadLine());

                    Console.Write("Podaj otrzymany kod błędu (1 do 10 znaków): ");
                    string errorCode = Validation.ValidateErrorCode(Console.ReadLine());

                    Console.Write("Podaj numer kontaktowy: ");
                    string contactNumber = Validation.ValidateContactNumber(Console.ReadLine());

                    Ticket.CreateTicket(new Ticket(_userFirstName, _userLastName, contactNumber, description,
                        DateTime.Today, errorCode));
                    Console.WriteLine("Ticket został utworzony.");
                }
                catch (WrongTicketDataFormatException)
                {
                    Console.WriteLine("Błąd, ticket nie został utworzony");
                }
                finally
                {
                    Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić...");
                    Console.ReadKey();
                    OpenHomeScreen();
                }

        }

        private void OpenTicketList()
        {
            Console.Clear();
            Console.WriteLine("=== Lista Ticketów ===");
            var tickets = Ticket.GetTickets();
            if (_userType == UserType.USER)
            {
                foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                {
                    PrintTicket(ticket.value, ticket.index);
                }
            }
            if (_userType == UserType.POWERUSER)
            {
                foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                {
                    if (ticket.value.AssigneeFirstName?.Equals(_userFirstName) == true &&
                        ticket.value.AssigneeLastName?.Equals(_userLastName) == true && !ticket.value.State.Equals(TicketState.CLOSED))
                    {
                        PrintTicket(ticket.value, ticket.index);
                    }
                }
            }
            if (_userType == UserType.ADMIN)
            {
                foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                {
                    PrintTicket(ticket.value, ticket.index);
                }
            }

            Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić...");
            Console.ReadKey();
        }

        private void PrintTicket(Ticket ticket, int ticketIndex)
        {
            Console.ForegroundColor = ticketIndex % 2 == 0 ? ConsoleColor.Blue : ConsoleColor.Red;
            Console.WriteLine("TICKET NR " + (ticketIndex + 1));
            Console.WriteLine($"UTWORZONO: {ticket.IssueRegistrationDateTime.ToString("dd-MM-yyyy")} PRZEZ: {ticket.IssuerFirstName} {ticket.IssuerLastName} (nr: {ticket.IssuerContactNumber})");
            Console.WriteLine($"OPIS: {ticket.IssueDescription.Substring(0, 20)}...");
            Console.WriteLine($"KOD BŁĘDU: {ticket.ErrorCode}");
            if (_userType.Equals(UserType.ADMIN) || _userType.Equals(UserType.POWERUSER))
            {
                Console.WriteLine($"PRZYPISANY DO: {ticket.AssigneeFirstName} {ticket.AssigneeLastName}");
                Console.WriteLine($"PRIORYTET: {ticket.Priority}");
            }

            Console.WriteLine("-------------------------------------------");
            Console.ResetColor();
        }

        private void RequestTicketClosure()
        {

            while (true){
            Console.WriteLine("=== Wnioskuj o Zamknięcie Ticketu ===");
            var tickets = Ticket.GetTickets();
            List<Ticket> workersTickets = new List<Ticket>();
            int i = 0;
                foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                {
                    if (ticket.value.AssigneeFirstName?.Equals(_userFirstName) == true &&
                        ticket.value.AssigneeLastName?.Equals(_userLastName) == true && !ticket.value.State.Equals(TicketState.CLOSED))
                    {
                        workersTickets.Add(ticket.value);
                        PrintTicket(ticket.value, i);
                        i++;
                    }
                }

                Console.WriteLine("Wobec którego ticketu chciałbyś wnioskować o zamknięcie?");
                string ticketStringNumber = Console.ReadLine();
                int ticketNumber;
                if (Int32.TryParse(ticketStringNumber, out ticketNumber))
                {
                    if (ticketNumber <= workersTickets.Count)
                    {

                        workersTickets[ticketNumber - 1].RequestClosure();
                        Console.WriteLine("Wysłano zapytanie");
                        Console.ReadKey();
                        break;
                    }
                }

                break;
            }
        }

        private void AssignTicket()
        {
            while (true){

                try
                {
                    Console.WriteLine("=== Przypisz Ticket ===");
                    var tickets = Ticket.GetTickets();
                    List<Ticket> workersTickets = new List<Ticket>();
                    foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                    {
                        PrintTicket(ticket.value, ticket.index);
                        workersTickets.Add(ticket.value);
                    }

                    Console.WriteLine("Który ticket chcesz przypisać?");
                    string ticketStringNumber = Console.ReadLine();
                    int ticketNumber;
                    if (Int32.TryParse(ticketStringNumber, out ticketNumber))
                    {
                        if (ticketNumber <= workersTickets.Count)
                        {
                            Console.WriteLine("Komu chcesz przypisać ticket?");
                            Console.Write("Imię pracownika:");
                            string workerFirstName = Validation.ValidateFirstName(Console.ReadLine());
                            Console.Write("Nazwisko pracownika:");
                            string workerLastName = Validation.ValidateLastName(Console.ReadLine());
                            workersTickets[ticketNumber - 1].AssignTicket(workerFirstName, workerLastName);
                            break;
                        }
                    }

                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }

            }
        }

        private void CheckRequests()
        {
            while (true){
                Console.WriteLine("=== Zarządzaj statusem ticketu ===");
                var tickets = Ticket.GetTickets();
                List<Ticket> workersTickets = new List<Ticket>();
                int i = 0;
                foreach (var ticket in tickets.Select((value, index) => new { value, index }))
                {
                    if (ticket.value.IsClosureRequested)
                    {
                        PrintTicket(ticket.value, i);
                        i++;
                        workersTickets.Add(ticket.value);
                    }
                }

                Console.WriteLine("W którym tickecie chcesz wykonać akcję?");
                string ticketStringNumber = Console.ReadLine();
                int ticketNumber;
                if (Int32.TryParse(ticketStringNumber, out ticketNumber))
                {

                    Console.WriteLine("1) Potwierdź zamknięcie");
                    Console.WriteLine("2) Odrzuć zamknięcie");
                    ticketStringNumber = Console.ReadLine();
                    if (ticketStringNumber == "1")
                    {

                        if (ticketNumber <= workersTickets.Count)
                        {
                            workersTickets[ticketNumber - 1].CloseTicket();
                            Console.WriteLine("Zamknięto ticket");
                            break;

                        }
                    }

                    if (ticketStringNumber == "2")
                    {

                        if (ticketNumber <= workersTickets.Count)
                        {
                            workersTickets[ticketNumber - 1].RejectTicketClosure();
                            Console.WriteLine("Odrzucono żądanie");
                            break;

                        }
                    }


                }

                break;

            }

        }
    }
}
