using EF_project.Data;
using EF_project.Entitty;
using Microsoft.EntityFrameworkCore;

namespace EF_project
{
    internal class WorkDb
    {

        private bool EnsureUserIsAdmin(User user)
        {
            if (user.IsAdmin)
            {
                return true;
            }
            Console.WriteLine("You are not admin, so you cannot do this");
            return false;
        }

        #region Status

        private void CreateHelpTables()
        {
            using (var context = new AppDbContext())
            {
                // Status-session

                if(context.StatusSessions.Any() || context.StatusTickets.Any())
                {
                    return;
                }

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Ongoing"
                });

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Cancelled"
                });

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Planned"
                });

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Finished"
                });

                // Status-ticket

                context.StatusTickets.Add(new StatusTicket
                {
                    Status = "Bought"
                });

                context.StatusTickets.Add(new StatusTicket
                {
                    Status = "Booked"
                });

                context.StatusTickets.Add(new StatusTicket
                {
                    Status = "Returned"
                });

                context.SaveChanges();
            }
        }

        public StatusSession GetStatusSessionByName(string name)
        {
            using (var context = new AppDbContext())
            {
                var statusSession = context.StatusSessions.FirstOrDefault(s => s.Status == name);
                if (statusSession != null)
                {
                    return statusSession;
                }
                else
                {
                    Console.WriteLine("Status not found");
                    return null;
                }
            }
        }

        public StatusTicket GetStatusTicketByName(string name)
        {
            using (var context = new AppDbContext())
            {
                var statusSession = context.StatusTickets.FirstOrDefault(s => s.Status == name);
                if (statusSession != null)
                {
                    return statusSession;
                }
                else
                {
                    Console.WriteLine("Status not found");
                    return null;
                }
            }
        }

        #endregion

        public WorkDb()
        {
            Console.WriteLine("Log started:");
            CreateHelpTables();
            Console.WriteLine("Help-tables filled");
        }

        #region Add/Remove/Update Users

        public void AddUser(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public void RemoveUser(User user, User admin)
        {
            using (var context = new AppDbContext())
            {
                var userToRemove = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userToRemove != null)
                {
                    if(EnsureUserIsAdmin(admin))
                    {
                        if (!userToRemove.IsAdmin)
                        {
                            Console.WriteLine("There is no point in removing standart user");
                            return;
                        }

                        context.Users.Remove(userToRemove);
                        context.SaveChanges();
                    }
                }
                else
                {
                    Console.WriteLine("User not found");
                }
            }
        }

        public void UpdateUser(User user)
        {
            using (var context = new AppDbContext())
            {
                var userToUpdate = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.Name = user.Name;
                    userToUpdate.Bonuses = user.Bonuses;
                    userToUpdate.Email = user.Email;
                    userToUpdate.IsAdmin = user.IsAdmin;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("User not found");
                }
            }
        }

        public User FindUserByEmail(string email)
        {
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    Console.WriteLine("User not found");
                    return null;
                }
            }
        }

        public bool IsRegular(User user)
        {
            if (user.Sales == null) return false;
            if (user.Sales.Count() > 3)
            {
                return true;
            }
            return false;
        }

        public void AddBonuses(User user, int bonuses)
        {
            using (var context = new AppDbContext())
            {
                var userToUpdate = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.Bonuses += bonuses;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("User not found");
                }
            }
        }

        #endregion

        #region Add/Remove/Update Films

        public void AddFilm(Film film, User user)
        {
            using (var context = new AppDbContext())
            {
                if(EnsureUserIsAdmin(user))
                {
                    context.Films.Add(film);
                    context.SaveChanges();
                }
            }
        }

        public void RemoveFilm(string name, User user)
        {
            using (var context = new AppDbContext())
            {
                if(EnsureUserIsAdmin(user))
                {
                    var filmToRemove = context.Films.FirstOrDefault(f => f.Name == name);
                    if (filmToRemove != null)
                    {
                        context.Films.Remove(filmToRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void UpdateDescription(string ds, string name, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == name);
                    if (filmToUpdate != null)
                    {
                        filmToUpdate.Description = ds;
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public Film GetFilmByName(string name)
        {
            using (var context = new AppDbContext())
            {
                var film = context.Films.FirstOrDefault(f => f.Name == name);
                if (film != null)
                {
                    return film;
                }
                else
                {
                    Console.WriteLine("Film not found");
                    return null;
                }
            }
        }

        #endregion

        #region Add/Remove/Update Discouts

        public void AddDiscount(int discount, Film film, User user)
        {
            if(discount < 0 || discount > 100)
            {
                Console.WriteLine("Discount must be between 0 and 100");
                return;
            }

            using (var context = new AppDbContext())
            {
                if(EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
                    if (filmToUpdate == null)
                    {
                        Console.WriteLine("Film not found");
                        return;
                    }
                    context.Attach(filmToUpdate);
                    if (filmToUpdate != null)
                    {
                        if(filmToUpdate.Discount != null)
                        {
                            Console.WriteLine("Film already has discount");
                            return;
                        }

                        var d = new Discount
                        {
                            DiscountPercent = discount,
                            Film = filmToUpdate
                        };

                        context.Add(d);

                        filmToUpdate.Discount = d;
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void AddRegularDiscount(int discount, Film film, User user)
        {
            if (discount < 0 || discount > 100)
            {
                Console.WriteLine("Discount must be between 0 and 100");
                return;
            }

            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
                    if (filmToUpdate == null)
                    {
                        Console.WriteLine("Film not found");
                        return;
                    }
                    context.Attach(filmToUpdate);
                    if (filmToUpdate != null)
                    {
                        if (filmToUpdate.RegularDiscountId != null)
                        {
                            Console.WriteLine("Film already has discount for regular visitors");
                            return;
                        }

                        if (filmToUpdate.Discount != null && filmToUpdate.Discount.DiscountPercent >= discount)
                        {
                            Console.WriteLine("Standart discount is bigger than one for regular visitors!");
                            return;
                        }

                        var d = new RegularDiscount
                        {
                            DiscountPercent = discount,
                            Film = filmToUpdate
                        };

                        context.Add(d);

                        filmToUpdate.RegularDiscount = d;
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void RemoveDiscount(Film film, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
                    if (filmToUpdate != null)
                    {
                        if (filmToUpdate.Discount != null)
                        {
                            context.Discounts.Remove(filmToUpdate.Discount);
                            filmToUpdate.Discount = null;
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Film does not have discount");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void RemoveRegularDiscount(Film film, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films
                        .Include(f => f.RegularDiscount)
                        .FirstOrDefault(f => f.Id == film.Id);
                    if (filmToUpdate != null)
                    {
                        if (filmToUpdate.RegularDiscount != null)
                        {
                            context.RegularDiscounts.Remove(filmToUpdate.RegularDiscount);
                            filmToUpdate.RegularDiscount = null;
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Film does not have discount for regular visitors");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void ChangeDiscount(Film film, User user, int newDiscount)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
                    if (filmToUpdate != null)
                    {
                        if (filmToUpdate.Discount != null)
                        {
                            if (newDiscount < 0 || newDiscount > 100)
                            {
                                Console.WriteLine("Discount must be between 0 and 100");
                                return;
                            }
                            filmToUpdate.Discount.DiscountPercent = newDiscount;
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Film does not have discount");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        public void ChangeRegularDiscount(Film film, User user, int newDiscount)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
                    if (filmToUpdate != null)
                    {
                        if (filmToUpdate.RegularDiscount != null)
                        {
                            if (newDiscount < 0 || newDiscount > 100)
                            {
                                Console.WriteLine("Discount must be between 0 and 100");
                                return;
                            }
                            filmToUpdate.RegularDiscount.DiscountPercent = newDiscount;
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("Film does not have discount");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Film not found");
                    }
                }
            }
        }

        #endregion

        #region Add/Remove/Update Halls

        public void AddHall(Hall hall, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    context.Halls.Add(hall);
                    context.SaveChanges();
                }
            }
        }

        public void RemoveHall(int id, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var hallToRemove = context.Halls.FirstOrDefault(h => h.Id == id);
                    if (hallToRemove != null)
                    {
                        context.Halls.Remove(hallToRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Hall not found");
                    }
                }
            }
        }

        public void UpdateHall(int id, int seats, bool isV, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var hallToUpdate = context.Halls.FirstOrDefault(h => h.Id == id);
                    if (hallToUpdate != null)
                    {
                        hallToUpdate.IsVip = isV;
                        if(seats <= 0)
                        {
                            Console.WriteLine("Seats must be more than 0");
                            return;
                        }
                        if(seats < hallToUpdate.Seats)
                        {
                            RemoveSeats(id, seats, hallToUpdate.Seats, user);
                        } else
                        {
                            AddSeats(id, seats, hallToUpdate.Seats, user);
                        }

                            hallToUpdate.Seats = seats;
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Hall not found");
                    }
                }
            }
        }

        public void AddSeats(int Hallid, int newAofseats, int prevAsofseats, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var ses = context.Sessions
                        .Include(s => s.Hall)
                        .Include(s => s.Tickets)
                        .Include(s => s.Status)
                        .Include(s => s.Film)
                        .Where(s => s.Hall.Id == Hallid)
                        .ToList();

                    foreach (var s in ses)
                    {
                        for(int i = prevAsofseats+1; i <= newAofseats; i++)
                        {
                            AddTicket(new Ticket
                            {
                                Session = s,
                                SeatNumber = i,
                            }, user);
                        }
                    }
                }
            }
        }

        public void RemoveSeats(int Hallid, int newAofseats, int prevAsofseats, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var ses = context.Sessions
                        .Include(s => s.Hall)
                        .Include(s => s.Tickets)
                        .Where(s => s.Hall.Id == Hallid)
                        .ToList();

                    foreach (var s in ses)
                    {
                        var ticketsToRemove = s.Tickets
                            .Where(t => t.SeatNumber > newAofseats && t.SeatNumber <= prevAsofseats)
                            .ToList();

                        foreach (var ticket in ticketsToRemove)
                        {
                            context.Tickets.Remove(ticket);
                        }
                    }
                }
            }
        }

        public Hall GetHallById(int id)
        {
            using (var context = new AppDbContext())
            {
                var hall = context.Halls.FirstOrDefault(h => h.Id == id);
                if (hall != null)
                {
                    return hall;
                }
                else
                {
                    Console.WriteLine("Hall not found");
                    return null;
                }
            }
        }

        #endregion

        #region Add/Remove/Update Sessions

        public void AddSession(Session session, User user)
        {
            using (var context = new AppDbContext())
            {
                var film = context.Films.FirstOrDefault(f => f.Name == session.Film.Name);
                var hall = context.Halls.FirstOrDefault(h => h.Id == session.Hall.Id);
                var status = context.StatusSessions.FirstOrDefault(s => s.Status == session.Status.Status);

                if (film == null || hall == null || status == null)
                {
                    Console.WriteLine("Something went wrong");
                    return;
                }

                if (EnsureUserIsAdmin(user))
                {
                    var newSession = new Session
                    {
                        Film = film,
                        Hall = hall,
                        Status = status,
                        StartTime = session.StartTime,
                        Price = session.Price
                    };

                    context.Sessions.Add(newSession);
                    context.SaveChanges();
                }
            }
        }


        public void RemoveSession(Session session, User user)
        {
            using (var context = new AppDbContext())
            {
                if(EnsureUserIsAdmin(user))
                {
                    var sessionToRemove = context.Sessions.FirstOrDefault(s => s.Id == session.Id);
                    if (sessionToRemove != null)
                    {
                        context.Sessions.Remove(sessionToRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Session not found");
                    }
                }
            }
        }

        public Session FindSession(string filmname, int hallid, DateTime date)
        {
            using (var context = new AppDbContext())
            {
                var s = context.Sessions.Include(s => s.Film)
                    .Include(s => s.Hall)
                    .FirstOrDefault(s => s.Film.Name == filmname && s.Hall.Id == hallid && s.StartTime.Date == date.Date);

                if (s == null)
                {
                    Console.WriteLine("Session not found");
                    return null;
                }
                else
                {
                    return s;
                }
            }
        }

        public Session[] GetSessionsByFilm(string filmname, User user)
        {
            using (var context = new AppDbContext())
            {
                var sessions = context.Sessions
                    .Include(s => s.Film)
                    .Include(s => s.Hall)
                    .Include(s => s.Status)
                    .ToArray();
                //.Where(s => s.Film.Name == filmname && s.Status.Status == "Planed")
                //.ToArray();
                var ses = sessions.Where(s => s.Film.Name == filmname && s.Status.Status == "Planned").ToArray();

                if (ses.Length == 0)
                {
                    Console.WriteLine("No sessions found");
                    return null;
                }
                else
                {
                    return ses;
                }
            }
        }

        public void ChangeStatus(Session s, StatusSession status, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var st = context.StatusSessions.FirstOrDefault(s => s.Status == status.Status);
                    var session = context.Sessions.FirstOrDefault(ses => ses.Id == s.Id);

                    if(st != null)
                        session.Status = st;
                    context.SaveChanges();
                }
            }
        }

        public Session GetSessionById(int id)
        {
            using (var context = new AppDbContext())
            {
                var session = context.Sessions.FirstOrDefault(s => s.Id == id);
                if (session != null)
                {
                    return session;
                }
                else
                {
                    Console.WriteLine("Session not found");
                    return null;
                }
            }
        }

        #endregion

        #region Add/Remove/Update Tickets

        public void AddTicket(Ticket ticket, User user)
        {
            using (var context = new AppDbContext())
            {
                context.Attach(ticket.Session);
                context.Attach(ticket.Session.Film);
                context.Attach(ticket.Session.Hall);
                context.Attach(ticket.Session.Status);
                if(ticket.Status != null)
                    context.Attach(ticket.Status);

                if (EnsureUserIsAdmin(user))
                {
                    context.Tickets.Add(ticket);
                    context.SaveChanges();
                }
            }
        }

        public void RemoveTicket(Ticket ticket, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var ticketToRemove = context.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                    if (ticketToRemove != null)
                    {
                        context.Tickets.Remove(ticketToRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Ticket not found");
                    }
                }
            }
        }

        public void ChangeStatusTicket(Ticket ticket, string status)
        {
            using (var context = new AppDbContext())
            {
                var ticketToUpdate = context.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                var statusTicket = context.StatusTickets.FirstOrDefault(s => s.Status == status);
                if (ticketToUpdate != null)
                {
                    ticketToUpdate.Status = statusTicket;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Ticket not found");
                }
            }
        }

        public Ticket GetTicketById(int id)
        {
            using (var context = new AppDbContext())
            {
                var ticket = context.Tickets.Include(t => t.Status).FirstOrDefault(t => t.Id == id);
                if (ticket != null)
                {
                    return ticket;
                }
                else
                {
                    Console.WriteLine("Ticket not found");
                    return null;
                }
            }
        }

        public void SetPrice(Ticket ticket, int price)
        {
            using (var context = new AppDbContext())
            {
                var ticketToUpdate = context.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                if (ticketToUpdate != null)
                {
                    ticketToUpdate.Price = price;
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Ticket not found");
                }
            }
        }

        public List<Ticket> GetTicketsBySession(Session session)
        {
            using (var context = new AppDbContext())
            {
                var tick = context.Tickets
                    .Include(t => t.Session)
                    .Include(t => t.Status)
                    //.Where(t => t.Session.Id == session.Id && (t.Status == null || t.Status.Status == "Returned"))
                    .ToList();

                var tickets = tick.Where(t => t.Session.Id == session.Id && (t.Status == null || t.Status.Status == "Returned")).ToList();

                if (tickets.Count == 0)
                {
                    Console.WriteLine("No tickets found");
                    return null;
                }
                else
                {
                    return tickets;
                }
            }
        }

        #endregion

        #region Add/Remove/Update Sales

        public void AddSale(Sale sale)
        {
            using (var context = new AppDbContext())
            {
                context.Attach(sale.User);
                context.Sales.Add(sale);
                context.SaveChanges();
            }
        }

        public void RemoveSale(Sale sale, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var saleToRemove = context.Sales.FirstOrDefault(s => s.Id == sale.Id);
                    if (saleToRemove != null)
                    {
                        context.Sales.Remove(saleToRemove);
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Sale not found");
                    }
                }
            }
        }

        public Sale[] GetSalesByPeriod(DateTime start, DateTime end)
        {
            using (var context = new AppDbContext())
            {
                var sales = context.Sales
                    .Where(s => s.SaleDate >= start && s.SaleDate <= end)
                    .ToArray();
                if (sales.Length == 0)
                {
                    Console.WriteLine("No sales found");
                    return null;
                }
                else
                {
                    return sales;
                }
            }
        }

        #endregion
    }
}
