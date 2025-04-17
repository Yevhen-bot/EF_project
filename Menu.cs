internal partial class Program
{
    public static class Menu
    {
        public static void PrintMainMenu()
        {
            Console.WriteLine("0 - films, 1 - sessions, 2 - discounts, 3 - halls, 4 - order, 5 - quit");
        }

        public static void PrintFilmsMenu()
        {
            Console.WriteLine("0 - add film, 1 - delete film, 2 - update film description");
        }

        public static void PrintSessionsMenu()
        {
            Console.WriteLine("0 - add session, 1 - delete session, 2 - change status");
        }

        public static void PrintDiscountsMenu()
        {
            Console.WriteLine("0 - add discount, 1 - delete discount");
        }

        public static void PrintHallsMenu()
        {
            Console.WriteLine("0 - add hall, 1 - delete hall, 2 - update hall");
        }

        public static void PrintOrderMenu()
        {
            Console.WriteLine("0 - add ticket to item list, 1 - return ticket, 2 - make an order, 3 - see profit, 4 - bye bye");
        }
    }
}
