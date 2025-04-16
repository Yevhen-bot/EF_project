using System.Net.NetworkInformation;
using EF_project;
using EF_project.Entitty;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;

internal partial class Program
{
    private const int VIP_ADDER = 40;

    private static readonly WorkDb db = new WorkDb();

    #region Actions

    public static void FilmAction(User user)
    {
        Menu.PrintFilmsMenu();
        int answer = int.Parse(Console.ReadLine());
        switch (answer)
        {
            case 0:
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
                    if (Console.ReadLine() != "0")
                    {
                        film.Restriction = int.Parse(Console.ReadLine());
                    }
                    Console.Write("Does film have description(y/n): ");
                    if (Console.ReadLine() == "y")
                    {
                        Console.Write("Enter the description: ");
                        film.Description = Console.ReadLine();
                    }
                    db.AddFilm(film, user);
                } break;

            case 1:
                {
                    Console.WriteLine("Enter name of the film you want to delete");
                    string name = Console.ReadLine();
                    db.RemoveFilm(name, user);
                } break;
            case 2:
                {
                    Console.WriteLine("Enter name of the film");
                    string name = Console.ReadLine();
                    Console.WriteLine("And new description");
                    string description = Console.ReadLine();
                    db.UpdateDescription(name, description, user);
                } break;
            default:
                {
                    Console.WriteLine("Sorry, but we don`t have this option yet");
                } break;
        }
    }

    public static void SessionAction(User user)
    {
        Menu.PrintSessionsMenu();
        int answer = int.Parse(Console.ReadLine());
        switch (answer)
        {
            case 0:
                {
                    Console.Write("Enter the datetime of the session: ");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    Console.Write("Enter the hall number it will be set in: ");
                    int hallId = int.Parse(Console.ReadLine());
                    var hall = db.GetHallById(hallId);
                    if (hall == null)
                    {
                        return;
                    }
                    Console.Write("Enter the name of film for the session: ");
                    string filmName = Console.ReadLine();
                    var film = db.GetFilmByName(filmName);
                    if (film == null)
                    {
                        return;
                    }
                    Console.Write("Enter the price for tickets for the session: ");
                    int price = int.Parse(Console.ReadLine());

                    Console.WriteLine("The status of this session?(Planned, Ongoing, Cancelled, Finished)");
                    string status = Console.ReadLine();
                    var st = db.GetStatusSessionByName(status);
                    if (status == null) return;

                    var s = new Session
                    {
                        StartTime = date,
                        Price = price,
                        Hall = hall,
                        Status = st,
                        Film = film
                    };

                    db.AddSession(s, user);
                    for (int j = 1; j <= hall.Seats; j++)
                    {
                        db.AddTicket(new Ticket
                        {
                            SeatNumber = j,
                            Session = s,
                        }, user);
                    }
                } break;
            case 1:
                {
                    Console.WriteLine("Enter name of the film of session you want to delete");
                    string name = Console.ReadLine();
                    Console.WriteLine("And datetime");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    Console.WriteLine("And hall");
                    int hallId = int.Parse(Console.ReadLine());
                    var s = db.FindSession(name, hallId, date);
                    if(s != null) db.RemoveSession(s, user);
                } break;
            case 2:
                {
                    Console.WriteLine("Enter name of the film of session");
                    string name = Console.ReadLine();
                    Console.WriteLine("And datetime");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    Console.WriteLine("And hall");
                    int hallId = int.Parse(Console.ReadLine());
                    var s = db.FindSession(name, hallId, date);
                    Console.WriteLine("And new status of this session?(Planned, Ongoing, Cancelled, Finished)");
                    string status = Console.ReadLine();
                    var st = db.GetStatusSessionByName(status);
                    if (s != null && st != null)
                    {
                        db.ChangeStatus(s, st, user);
                    }
                } break;
            default:
                {
                    Console.WriteLine("Sorry, but we don`t have this option yet");
                }
                break;
        }
    }

    public static void DiscountAction(User user)
    {
        Menu.PrintDiscountsMenu();
        int answer = int.Parse(Console.ReadLine());
        switch (answer)
        {
            case 0:
                {
                    Console.Write("Enter the name of the film: ");
                    string name = Console.ReadLine();
                    var film = db.GetFilmByName(name);
                    if (film == null) return;
                    Console.Write("Enter the discount value: ");
                    int value = int.Parse(Console.ReadLine());
                    Console.WriteLine("And is this discount for regulars(y/n)?");
                    string isRegular = Console.ReadLine();
                    if (isRegular == "y")
                    {
                        db.AddRegularDiscount(value, film, user);
                    }
                    else
                    {
                        db.AddDiscount(value, film, user);
                    }
                }
                break;
            case 1:
                {
                    Console.WriteLine("Enter name of the film of discount you want to delete");
                    string name = Console.ReadLine();
                    var film = db.GetFilmByName(name);
                    if (film == null) return;
                    Console.WriteLine("And is this discount for regulars(y/n)?");
                    string isRegular = Console.ReadLine();
                    if (isRegular == "y")
                    {
                        db.RemoveRegularDiscount(film, user);
                    }
                    else
                    {
                        db.RemoveDiscount(film, user);
                    }
                }
                break;
            default:
                {
                    Console.WriteLine("Sorry, but we don`t have this option yet");
                }
                break;
        }
    }

    public static void HallAction(User user)
    {
        Menu.PrintHallsMenu();
        int answer = int.Parse(Console.ReadLine());
        switch (answer)
        {
            case 0:
                {
                    Console.Write("Enter how many seats does hall have: ");
                    int seatsCount = int.Parse(Console.ReadLine());
                    Console.Write("And is it VIP(y/n): ");
                    string isVIP = Console.ReadLine();
                    db.AddHall(new Hall
                    {
                        Seats = seatsCount,
                        IsVip = isVIP == "y" ? true : false
                    }, user);
                }
                break;
            case 1:
                {
                    Console.WriteLine("Enter id of the hall you want to delete");
                    int id = int.Parse(Console.ReadLine());
                    db.RemoveHall(id, user);
                }
                break;
            case 2:
                {
                    Console.WriteLine("Enter id of the hall you want to update");
                    int id = int.Parse(Console.ReadLine());
                    Console.Write("Enter how many seats does hall have: ");
                    int seatsCount = int.Parse(Console.ReadLine());
                    Console.Write("And is it VIP(y/n): ");
                    string isVIP = Console.ReadLine();
                    db.UpdateHall(id, seatsCount, isVIP == "y" ? true : false, user);
                }
                break;
            default:
                {
                    Console.WriteLine("Sorry, but we don`t have this option yet");
                }
                break;
        }
    }

    public static void OrderAction(User user)
    {
        int ticketsCount = 0;
        int totalPrice = 0;
        for (; ; )
        {
            Menu.PrintOrderMenu();
            int answer = int.Parse(Console.ReadLine());
            switch (answer)
            {
                case 0:
                    {
                        Console.WriteLine("Of course, on what film do you want to go?");
                        string name = Console.ReadLine();
                        var film = db.GetFilmByName(name);
                        if (film == null) break;
                        var ses = db.GetSessionsByFilm(name, user);
                        if (ses == null)
                        {
                            Console.WriteLine("Sorry, but we don`t have availible sessions with this film yet");
                            break;
                        }
                        Console.WriteLine("And what session do you want to go(1, 2, 3, ...)?");
                        foreach (var s in ses)
                        {
                            Console.WriteLine($"{s.Id} - {s.StartTime}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        if(id > ses.Count() || id < 0)
                        {
                            Console.WriteLine("Incorrect");
                            break;
                        }
                        var session = ses[id];

                        int ticketprice = session.Price;
                        Console.WriteLine("Of course!");
                        
                        if(db.IsRegular(user) && film.RegularDiscount != null)
                        {
                            Console.WriteLine($"This film has discount for regulars, and you are one of them! So ticket will be yours for {film.RegularDiscount.DiscountPercent}% cheaper!");
                            ticketprice = (int)(ticketprice * (1 - film.RegularDiscount.DiscountPercent / 100.0));
                        }
                        else if(film.Discount != null)
                        {
                            Console.WriteLine($"This film has discount, so ticket will be yours for {film.Discount.DiscountPercent}% cheaper!");
                            ticketprice = (int)(ticketprice * (1 - film.Discount.DiscountPercent / 100.0)); ;
                        }
                        else
                        {
                            Console.WriteLine($"There is no discounts for this film, sorry");
                        }

                        if(session.Hall.IsVip)
                        {
                            Console.WriteLine($"This session is in VIP hall, so ticket`s value is up for {VIP_ADDER}!");
                            ticketprice += VIP_ADDER;
                        }

                        int bonusesToAdd = ticketprice / 10;
                        Console.WriteLine($"You will get {bonusesToAdd} bonuses for this ticket");

                        Console.WriteLine("Do you want to use your {0} bonuses(y/n)?", user.Bonuses);
                        string answer1 = Console.ReadLine();
                        if (answer1 == "y")
                        {
                            if(user.Bonuses <= ticketprice)
                            {
                                ticketprice -= user.Bonuses;
                                user.Bonuses = 0;
                            }
                            else
                            {
                                user.Bonuses -= ticketprice;
                                ticketprice = 0;
                            }
                        }

                        Console.WriteLine("And what seat do you want to take?");
                        var seats = session.Tickets.Where(t => t.Status.Status == null || t.Status.Status == "Returned").ToList();
                        Console.WriteLine("Availible are: ");
                        foreach (var t in seats)
                        {
                            Console.WriteLine(t.SeatNumber + " ");
                        }
                        int seatNumber = int.Parse(Console.ReadLine());
                        var ticket = seats.FirstOrDefault(t => t.SeatNumber == seatNumber);
                        if (ticket == null)
                        {
                            Console.WriteLine("Incorrect input");
                            break;
                        }
                        db.ChangeStatusTicket(ticket, "Booked");
                        ticketsCount++;
                        totalPrice += ticketprice;
                        db.SetPrice(ticket, ticketprice);
                        Console.WriteLine("Your ticket is booked and it`s id is {0}", ticket.Id);
                    }
                    break;
                case 1:
                    {
                        Console.WriteLine("Sure, what ticket`s id is?");
                        int id = int.Parse(Console.ReadLine());
                        var ticket = db.GetTicketById(id);
                        if (ticket == null)
                        {
                            Console.WriteLine("Sorry, but we can not find ticket with this id in our database");
                            break;
                        }
                        if (ticket.Status.Status == "Booked")
                        {
                            db.ChangeStatusTicket(ticket, "Returned");
                            ticketsCount--;
                            totalPrice -= (int)ticket.Price;
                            Console.WriteLine("Your ticket is returned");
                        }
                        else if (ticket.Status.Status == "Returned")
                        {
                            Console.WriteLine("Sorry, but this ticket is already returned");
                        }
                        else if (ticket.Status.Status == "Bought")
                        {
                            Console.WriteLine("Sorry, but this ticket is already bought and company policy forbids returning bought ticket");
                        }
                        else
                        {
                            Console.WriteLine("Sorry, but this ticket is not booked yet");
                        }
                    }
                    break;
                case 2:
                    {
                        db.AddSale(new Sale
                        {
                            User = user,
                            TicketsCount = ticketsCount,
                            TotalPrice = totalPrice,
                            SaleDate = DateTime.Now
                        });
                        Console.WriteLine("Thank you for your order, it is {0} total", totalPrice);
                    }
                    return;
                case 3:
                    {
                        if(!user.IsAdmin)
                        {
                            Console.WriteLine("You are not admin and can not see profit");
                            break;
                        }
                        Console.WriteLine("Enter start date of the period you want to see profit for");
                        DateTime startDate = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Enter end date of the period you want to see profit for");
                        DateTime endDate = DateTime.Parse(Console.ReadLine());

                        var sales = db.GetSalesByPeriod(startDate, endDate);
                        if(sales != null)
                        {
                            Console.WriteLine("Profit for this period is {0}", sales.Sum(s => s.TotalPrice));
                        }
                    }
                    return;
                default:
                    {
                        Console.WriteLine("Sorry, but we don`t have this option yet");
                    }
                    break;
            }
        }
    }

    #endregion

    // Easier first time initialization of the cinema
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

        Console.Write("Hello dear customer, are you buisness owner and here first time? (y/n): ");
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
                    Console.WriteLine("Do you want to register(y/n)?");
                    string answer1 = Console.ReadLine();
                    if (answer1 == "y")
                    {
                        Console.Write("Please, say your name:");
                        string name = Console.ReadLine();
                        currentUser = new User
                        {
                            Name = name,
                            Email = email,
                            IsAdmin = false
                        };
                        db.AddUser(currentUser);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Try again!");
                    }
                }
                else break;
            }
        }

        for(; ; )
        {
            Console.WriteLine("What do you want to?");
            Menu.PrintMainMenu();
            int answer1 = int.Parse(Console.ReadLine());
            switch (answer1)
            {
                case 0: FilmAction(currentUser); break;
                case 1: SessionAction(currentUser); break;
                case 2: DiscountAction(currentUser); break;
                case 3: HallAction(currentUser); break;
                case 4: OrderAction(currentUser); break;
                case 5: Console.WriteLine("Bye bye!!!"); ; return;
                default: Console.WriteLine("Sorry, but we don`t have this option yet"); break;
            }
        }

    }
}
