using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF_project.Data;
using EF_project.Entitty;
using Org.BouncyCastle.Asn1.Mozilla;

namespace EF_project
{
    internal class WorkDb
    {
        #region Private methods

        private bool EnsureUserIsAdmin(User user)
        {
            if (user.IsAdmin)
            {
                return true;
            }
            Console.WriteLine("You are not admin, so you cannot do this");
            return false;
        }

        private void CreateHelpTables()
        {
            using (var context = new AppDbContext())
            {
                // Status-session

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Ongoing"
                });

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Canceled"
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

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Booked"
                });

                context.StatusSessions.Add(new StatusSession
                {
                    Status = "Returned"
                });

                context.SaveChanges();
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

        public void RemoveFilm(Film film, User user)
        {
            using (var context = new AppDbContext())
            {
                if(EnsureUserIsAdmin(user))
                {
                    var filmToRemove = context.Films.FirstOrDefault(f => f.Name == film.Name);
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

        public void UpdateDescription(string ds, Film film, User user)
        {
            using (var context = new AppDbContext())
            {
                if (EnsureUserIsAdmin(user))
                {
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
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
                    var filmToUpdate = context.Films.FirstOrDefault(f => f.Name == film.Name);
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
    }
}
