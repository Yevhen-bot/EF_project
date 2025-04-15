using EF_project;
using EF_project.Entitty;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Security;

internal class Program
{
    private static readonly WorkDb db = new WorkDb();

    public static void PrintMainMenu()
    {
        Console.WriteLine("0 - films, 1 - sessions, 2 - discounts, 3 - halls, 4 - order, 5 - quit");
    }

    public static User FirstTime()
    {
        Console.WriteLine("Welcome to managing your cinema!");
        Console.Write("Please, enter your name: ");
        string name = Console.ReadLine();
        Console.Write("And you email: ");
        string email = Console.ReadLine();
        User owner = new User
        {
            Name = name,
            Email = email,
            IsAdmin = true
        };
        db.AddUser(owner);
        Console.WriteLine($"Hello {name}, we are glad that you have chosen us!");
        Console.WriteLine("Since it is your first time here, you should tell us basics about ur cinema");

        Console.WriteLine("Please enter how many halls do you have?");
        int hallsCount = int.Parse(Console.ReadLine());
        for (int i = 1; i <= hallsCount; i++)
        {
            Console.Write("Enter how many seats does " + i + " hall have: ");
            int seatsCount = int.Parse(Console.ReadLine());
            Console.Write("And is it VIP(y/n): ");
            string isVIP = Console.ReadLine();
            db.AddHall(new Hall
            {
                Seats = seatsCount,
                IsVip = isVIP == "y" ? true : false
            }, owner);
        }

        Console.Write("Please enter how many films do you have?");
        int filmsCount = int.Parse(Console.ReadLine());
        for (int i = 1; i <= filmsCount; i++)
        {
            Film film = new();
            Console.Write("Enter the name of the film: ");
            film.Name = Console.ReadLine();
            Console.Write("Enter the genre of the film: ");
            film.Genre = Console.ReadLine();
            Console.Write("Enter the duration of the film(in seconds): ");
            film.Duration = int.Parse(Console.ReadLine());
            Console.Write("Enter the director name: ");
            film.Director = Console.ReadLine();
            Console.Write("Enter the release date of the film: ");
            film.ReleaseDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter the restriction of the film(0 if don`t): ");
            if(Console.ReadLine() != "0")
            {
                film.Restriction = int.Parse(Console.ReadLine());
            }
            Console.Write("Does film have description(y/n): ");
            if(Console.ReadLine() == "y")
            {
                Console.Write("Enter the description: ");
                film.Description = Console.ReadLine();
            }
            db.AddFilm(film, owner);
        }

        Console.WriteLine("Please enter how many planned sessions do you have?");
        int sessionsCount = int.Parse(Console.ReadLine());
        for (int i = 1; i <= sessionsCount; i++)
        {
            Console.Write("Enter the datetime of the session: ");
            DateTime date = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter the hall number it will be set in: ");
            int hallId = int.Parse(Console.ReadLine());
            var hall = db.GetHallById(hallId);
            if(hall == null)
            {
                continue;
            }
            Console.Write("Enter the name of film for the session: ");
            string filmName = Console.ReadLine();
            var film = db.GetFilmByName(filmName);
            if (film == null)
            {
                continue;
            }
            Console.Write("Enter the price for tickets for the session: ");
            int price = int.Parse(Console.ReadLine());

            var st = db.GetStatusSessionByName("Planned");

            var s = new Session
            {
                StartTime = date,
                Price = price,
                Hall = hall,
                Status = st,
                Film = film
            };

            db.AddSession(s, owner);
            for(int j = 1; j <= hall.Seats; j++)
            {
                db.AddTicket(new Ticket
                {
                    SeatNumber = j,
                    Session = s,
                }, owner);
            }
        }

        Console.WriteLine("Enter how many workers do you have?");
        int workersCount = int.Parse(Console.ReadLine());
        for (int i = 1; i <= workersCount; i++)
        {
            Console.Write("Enter the name of the worker: ");
            string workerName = Console.ReadLine();
            Console.Write("Enter the email of the worker: ");
            string workerEmail = Console.ReadLine();
            db.AddUser(new User
            {
                Name = workerName,
                Email = workerEmail,
                IsAdmin = true
            });
        }

        Console.WriteLine("All done! Now you can manage your cinema!");
        return owner;
    }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.Write("Hello dear customer, are you here first time? (y/n): ");
        string answer = Console.ReadLine();
        User currentUser;
        if (answer == "y")
        {
            currentUser = FirstTime();
        }
        else
        {
            Console.WriteLine("Welcome back!");

            for(; ; )
            {
                Console.Write("Please, enter your email: ");
                string email = Console.ReadLine();
                currentUser = db.FindUserByEmail(email);
                if (currentUser == null)
                {
                    Console.WriteLine("Sorry, but we can not find you in our database");
                    return;
                }
                else break;
            }
        }

        for(; ; )
        {
            Console.WriteLine("What do you want to?");
            int answer1 = int.Parse(Console.ReadLine());
        }

    }
}
