using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KOVI_D_Cinema
{
    class Program
    {
        static List<Vetítés> Vetítések = new List<Vetítés>();
        static List<Film> Filmek = new List<Film>();
        static List<Szék> Székek = new List<Szék>();
        static List<Felhasználó> Userek = new List<Felhasználó>();
        public enum User_Státusz { Admin, Regisztrált_Látogató, Nem_Regisztrált }
        public enum Jegy_Státusz { Jegy_Állapot }

        static double ÁR = 1000;
        static StreamReader Olvasó;
        static StreamWriter Író;

        static int User_Last_ID;
        static int Film_Last_ID;
        static int Vetítés_Last_ID;

        static bool Logged_In = false;
        static int LOGGED_USER_ID;
        static string LOGGED_USER_NAME;


        #region Kinézet
        static int tableWidth = 75;
        static void PrintHeader(string title)
        {
            Console.WriteLine(new string('~', tableWidth));
            PrintRow(title);
            Console.WriteLine(new string('~', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        #endregion
        #region Adat_betölt

        static void User_Betölt()
        {
            Userek.Clear();
            Felhasználó User;
            if (File.Exists("Userek.txt"))
            {
                Olvasó = new StreamReader("Userek.txt");
                string line;
                while ((line = Olvasó.ReadLine()) != null)
                {
                    User = new Felhasználó(line);
                    Userek.Add(User);
                    User_Last_ID = User.ID;
                }
            }
            else
            {
                Console.WriteLine("Nem található a Userek.txt");
            }
            Felhasználó admin = new Felhasználó();
            Userek.Add(admin);
            Olvasó.Close();
        }
        static void Film_Betölt()
        {
            Filmek.Clear();
            Film Adat_Film;
            if (File.Exists("Filmek.txt"))
            {
                Olvasó = new StreamReader("Filmek.txt");
                string line;
                while ((line = Olvasó.ReadLine()) != null)
                {
                    Adat_Film = new Film(line);
                    Filmek.Add(Adat_Film);
                    Film_Last_ID = Adat_Film.Film_ID;
                }
            }
            else
            {
                Console.WriteLine("Nem található a Filmek.txt");
            }
            Olvasó.Close();
        }
        static void Vetítés_Betölt()
        {
            Vetítések.Clear();
            Vetítés Adat_Vetítés;
            if (File.Exists("Vetítések.txt"))
            {
                Olvasó = new StreamReader("Vetítések.txt");
                string line;
                while ((line = Olvasó.ReadLine()) != null)
                {
                    Adat_Vetítés = new Vetítés(line);
                    Vetítések.Add(Adat_Vetítés);
                    Vetítés_Last_ID = Adat_Vetítés.ID;
                }
            }
            else
            {
                Console.WriteLine("Nem található a Vetítések.txt");
            }
            Olvasó.Close();
        }
        static void Szék_Betölt()
        {
            Székek.Clear();
            Szék Adat_Szék;
            if (File.Exists("Székek.txt"))
            {
                Olvasó = new StreamReader("Székek.txt");
                string line;
                while ((line = Olvasó.ReadLine()) != null)
                {
                    Adat_Szék = new Szék(line);
                    Székek.Add(Adat_Szék);
                }
            }
            else
            {
                Console.WriteLine("Nem található a Székek.txt");
            }
            Olvasó.Close();
        }
        public static void Feltölt()
        {
            Film_Betölt();
            Vetítés_Betölt();
            Szék_Betölt();
            User_Betölt();
            Olvasó.Close();
        }
        #endregion
        #region Menu
        private static bool MainMenu()
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI");
            Console.WriteLine("Válassz egy menüpontot:");
            Console.WriteLine("\t1) Bejelentkezés");
            // felvitel
            Console.WriteLine("\t2) Regisztráció");
            Console.WriteLine("\t3) Vetítés Keresés Film Alapján");
            Console.WriteLine("\t4) Összes Vetítés Listázása");
            Console.WriteLine("\t5) Kilépés");
            Console.Write("\r\nKérlek válassz: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Bejelentkezés();
                    break;
                case "2":
                    Regisztráció();
                    break;
                case "3":
                    Keresés();
                    break;
                case "4":
                    Listázás();
                    break;
                case "5":
                    return false;
                default:
                    return true;
            }
            return true;
        }

        static void Bejelentkezés()
        {
            Console.Clear();
            PrintHeader("KOVI - D MOZI");
            Console.Write("Kérlek add meg az e-mail címedet: ");
            string email = Console.ReadLine();
            Console.Write("Kérlek add meg a jelszót a fiókodhoz: ");
            string jelszó = Console.ReadLine();

            var query = from user in Userek
                        where email.Equals(user.Email)
                        where jelszó.Equals(user.Jelszó)
                        select new { user.ID, user.Admin, user.Email, user.Jelszó, user.User_nev };

            if (!query.Any())
            {
                Console.WriteLine("Nem sikerült bejelentkezni\n\t 1) Újra próbál\n\t 2) Vissza a főmenübe");
                switch (Console.ReadLine())
                {
                    case "1":
                        Bejelentkezés();
                        break;
                    default:
                        Console.WriteLine("Nyomj egy gombot a továbblépéshez");
                        Console.ReadKey();
                        break;
                }

            }
            else
            {
                bool showMenu = true;
                foreach (var i in query)
                {
                    Console.WriteLine(i.Email + " bejelentkezett");
                    if (i.Admin == true)
                    {
                        while (showMenu)
                        {
                            showMenu = Admin_Menu();
                        }

                    }
                    else if (i.Admin == false)
                    {
                        while (showMenu)
                        {
                            Logged_In = true;
                            LOGGED_USER_ID = i.ID;
                            LOGGED_USER_NAME = i.User_nev;
                            showMenu = User_Menu();
                        }
                    }
                }
            }
        }

        private static bool User_Menu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            string welcome = "USER MENU - ÜDVÖZLET: " + LOGGED_USER_NAME;
            PrintHeader(welcome);
            Console.WriteLine("Foglalni / vásárolni a vetítés kiválasztása után tudsz.");
            Console.WriteLine("Válassz egy menüpontot:");
            Console.WriteLine("\t1) Vetítés Keresés Film Alapján");
            Console.WriteLine("\t2) Összes Vetítés Listázása");
            Console.WriteLine("\t3) Kijelentkezés");
            Console.Write("\r\nKérlek válassz: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Keresés();
                    break;
                case "2":
                    Listázás();
                    break;
                case "3":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Logged_In = false;
                    return false;
                default:
                    return true;
            }
            return true;
        }

        private static bool Admin_Menu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            PrintHeader("ADMIN MENU");
            //Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Válassz egy menüpontot:");
            Console.WriteLine("\t1) Vetítés felvitele");
            Console.WriteLine("\t2) Film felvitele");
            Console.WriteLine("\t3) Kijelentkezés");

            Console.Write("\r\nKérlek válassz: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Új_Vetítés();
                    break;
                case "2":
                    Új_Film();
                    break;
                case "3":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return false;
                default:
                    return true;
            }
            return true;
        }
        #endregion
        #region Adatfelvitel
        private static void Új_Film()
        {
            Console.Clear();
            PrintHeader("ADMIN - ÚJ FILM");
            Console.Write("Kérlek add meg a film címét: ");
            string film_cím = Console.ReadLine();
            Író = new StreamWriter("Filmek.txt", true);
            try
            {
                Író.WriteLine("{0};{1}", Film_Last_ID + 1, film_cím);
                Film_Last_ID++;

            }
            catch (Exception e)
            {
                Console.WriteLine("Nem sikerült a Film felvétele");
                Console.WriteLine("Hiba: {0}", e); throw;
            }
            Console.WriteLine("Sikeresen felvitted : {0}\n Nyomj meg egy gombot, hogy továbblépj", film_cím);
            Console.ReadKey();
            Író.Flush();
            Író.Close();
            Film_Betölt(); // Újra betölti,hogy benne legyen az amit most vitt fel
        }

        private static void Új_Vetítés()
        {
            Console.Clear();
            PrintHeader("ADMIN - ÚJ VETÍTÉS");
            Console.WriteLine("FILMEK:");
            var query = from film in Filmek
                        select new { film.Film_ID, film.Név };

            foreach (var i in query)
            {
                Console.WriteLine("\t" + i.Film_ID + ") " + i.Név);
            }
            Console.Write("\nKérlek add meg a Film ID-jét: ");
            string film_id = Console.ReadLine();
            Console.Write("Kérlek add meg a termet: ");
            string terem = Console.ReadLine();
            Console.Write("Kérlek add meg a dátumot: YYYY-MM-DD-HH ");
            string datum = Console.ReadLine();

            Író = new StreamWriter("Vetítések.txt", true);

            try
            {
                Író.WriteLine("{0};{1};{2};{3}", Vetítés_Last_ID + 1, film_id, terem, datum);
                Vetítés_Last_ID++;

            }
            catch (Exception e)
            {
                Console.WriteLine("Nem sikerült a Film felvétele");
                Console.WriteLine("Hiba: {0}", e); throw;
            }
            Console.WriteLine("Sikeresen felvitted a vetítést!\n Nyomj meg egy gombot, hogy továbblépj");
            Console.ReadKey();
            Író.Flush();
            Író.Close();
            Vetítés_Betölt();
        }

        static void Regisztráció()
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI - REGISZTRÁCIÓ");
            string email = "", jelszó = "", telefon = "", nev = ""; int a = 0;
            do
            {
                if (a == 1)
                {
                    Console.WriteLine("Hibás bevitel: Próbáld újra");
                }
                Console.Write("Kérlek add meg az e-mail címedet: ");
                email = Console.ReadLine(); a++;
            } while (string.IsNullOrEmpty(email));
            a = 0;
            do
            {
                if (a == 1)
                {
                    Console.WriteLine("Hibás bevitel: Próbáld újra");
                }
                Console.Write("Kérlek add meg a jelszót a fiókodhoz: ");
                jelszó = Console.ReadLine(); a++;
            } while (string.IsNullOrEmpty(jelszó));
            a = 0;
            do
            {
                if (a == 1)
                {
                    Console.WriteLine("Hibás bevitel: Próbáld újra");
                }
                Console.Write("Kérlek add meg a telefonszámod: ");
                telefon = Console.ReadLine(); a++;
            } while (string.IsNullOrEmpty(telefon));

            do
            {
                if (a == 1)
                {
                    Console.WriteLine("Hibás bevitel: Próbáld újra");
                }
                Console.Write("Kérlek add meg a teljes neved: ");
                nev = Console.ReadLine(); a++;
            } while (string.IsNullOrEmpty(nev));

            Író = new StreamWriter("Userek.txt", true);
            try
            {
                Író.WriteLine("{0};{1};{2};{3};{4}", User_Last_ID + 1, email, jelszó, telefon, nev);
                User_Last_ID++;
            }
            catch (Exception e)
            {

                Console.WriteLine("Nem sikerült a regisztráció");
                Console.WriteLine("Hiba: {0}", e); throw;
            }
            Console.WriteLine("Sikerült a regisztráció, köszönjuk : {0}", nev);
            Író.Flush();
            Író.Close();
            User_Betölt(); // újratölti,hogy benne legyen az újonnan felvitt
            // Felhasználó user = new Felhasználó(email,jelszo,telefon);
        }
        static void Listázás()
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI - VETÍTÉSEK");

            var query = from vetites in Vetítések
                        join film in Filmek
                        on vetites.Film_ID equals film.Film_ID
                        select new { vetites.ID, film.Név, vetites.Datum.S_date };
            Console.WriteLine("ID: | Név:");
            int last = 0;
            foreach (var i in query)
            {
                Console.WriteLine(i.ID + ")  | " + i.Név + " : " + i.S_date + " óra");
                last++;
            }
            Console.Write("\r\nKérlek válaszd ki az ID-jét a vetítésnek: ");
            string választ = Console.ReadLine();
            switch (választ)
            {
                case null:
                    Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                    Console.ReadKey();
                    Listázás();
                break;
                case "":
                    Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                    Console.ReadKey();
                    Listázás();
                    break;
                default:
                    if (Check_if_szám(választ) && Convert.ToInt32(választ) <= last)
                    {
                        Foglalás(választ);
                    }
                    else
                    {
                        Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                        Console.ReadKey();
                        Listázás();
                    }
                    break;
            }
        }
        static void Keresés()
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI - VETÍTÉS KERESÉS");
            Console.Write("Írd be a film címét, amit keresel ?: ");
            string keresett_film = Console.ReadLine();
            var query = from vetites in Vetítések
                        join film in Filmek
                        on vetites.Film_ID equals film.Film_ID
                        where film.Név.Equals(keresett_film)
                        select new { vetites.ID, film.Név, vetites.Datum.S_date };
            Console.WriteLine("ID: | Név:");
            List<int> IDk = new List<int>();
            foreach (var i in query)
            {
                Console.WriteLine(i.ID + ")  | " + i.Név + " : " + i.S_date + " óra");
                IDk.Add(i.ID);
            }
            Console.Write("\r\n Hogy visszalépj a főmenübe, írd : Exit");
            Console.Write("\r\nKérlek válaszd ki az ID-jét a vetítésnek a foglaláshoz: ");
            string választ = Console.ReadLine();
            switch (választ)
            {
                case null:
                    Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                    Console.ReadKey();
                    Keresés();
                    break;
                case "":
                    Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                    Console.ReadKey();
                    Listázás();
                    break;
                case "Exit": break;
                default:
                    if (Check_if_szám(választ) && IDk.Contains(Convert.ToInt32(választ)))
                    {
                        Foglalás(választ);
                    }
                    else
                    {
                        Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                        Console.ReadKey();
                        Keresés();
                    }
                    break;
            }
        }
    
        #endregion
        #region Foglalás
        static void Foglalás(string vetítés_id)
        {
            Console.Clear();
            int ID = Convert.ToInt32(vetítés_id);
            PrintHeader("KOVI-D MOZI - FOGLALÁS");
            if (!Logged_In)
            {
                Console.WriteLine("Előszőr be kell jelentkezned vagy regiszrálnod!");
                Console.WriteLine("Válassz egy menüpontot:");
                Console.WriteLine("\t1) Bejelentkezés");
                Console.WriteLine("\t2) Regisztráció");
                Console.WriteLine("\t3) Visszalépés");
                Console.Write("\r\nKérlek válassz: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Bejelentkezés();
                        break;
                    case "2":
                        Regisztráció();
                        break;
                    case "3":
                        Console.WriteLine("Nyomj bármilyen gombot a folytatáshoz");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var query = from vetites in Vetítések
                            join film in Filmek
                            on vetites.Film_ID equals film.Film_ID
                            where vetites.ID.Equals(ID)
                            select new { vetites.ID, film.Név, vetites.Datum.S_date };
                string Film_Név = " ";
                string Vetítés_Dátum = " ";
                int Vetítés_ID = 0;
                var item = query.First();
                Film_Név = item.Név;
                Vetítés_Dátum = item.S_date;
                Vetítés_ID = item.ID;
                Console.WriteLine("Foglalás a következőre:\n\t" + Film_Név + " \n\tekkor: " + Vetítés_Dátum + " órakor");
                int[,] Foglalt = FoglaltSzékek(Vetítés_ID); //foglalt
                Táblázat_Rajz();
                Székrajz(Foglalt);
                Console.Write("\nHány jegyet szeretnél venni/foglalni?: ");
                int db = Convert.ToInt32(Console.ReadLine());
                if (db == 0 || db > 100)
                {
                    Console.WriteLine("Befejezted a székválasztást. Nyomj egy gombot a továbblépéshez");
                }
                else
                {
                    Író = new StreamWriter("Székek.txt", true);
                    int sor = 0;
                    int oszlop = 0;
                    for (int i = 0; i < db; i++)
                    {
                        Console.Write("Az {0}. jegy hova szóljon?: \nSor:", i + 1);
                        sor = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Oszlop:");
                        oszlop = Convert.ToInt32(Console.ReadLine());
                        if (Keres(Foglalt, sor, oszlop) == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ez a hely már foglalt! Vagy nem tudsz ide foglalni \nKérlek válassz másikat");
                            i--;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else
                        {
                            try
                            {
                                Író.WriteLine("{0};{1};{2}", Vetítés_ID, sor, oszlop);
                                Foglalt[sor-1, oszlop-1] = 1;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Nem sikerült a felvétel");
                                Console.WriteLine("Hiba: {0}", e); throw;
                            }
                        }
                       
                    }
                    Console.WriteLine("Sikeresen foglaltál!\nNyomj meg egy gombot, hogy továbblépj");
                    Console.ReadKey();
                    Író.Flush();
                    Író.Close();
                    Szék_Betölt(); // újra betölt,hogy legyenek benne az adatok
                    Foglalás_Menu(db);

                    //Console.WriteLine("A jegyeid ára: {0}", db * ÁR);
                    Console.WriteLine("Nyomj meg egy gombot, hogy továbblépj");
                }
               
               
            }
            Console.ReadKey();
        }
        static bool Keres(int[,] matrix, int sor, int oszlop)
        {
            if (sor > 8 || oszlop > 8)
            {
                return true;
            }
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++) //
                {
                    if (matrix[sor - 1, oszlop - 1] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static void Táblázat_Rajz()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("     ");
            for (int i = 0; i < 8; i++)
            {
                Console.Write("{0}  ", i + 1);
            }
            Console.WriteLine();
        }
        private static void Foglalás_Menu(int db)
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI - FOGLALÁS / VÁSÁRLÁS");
            Console.WriteLine("Válassz egy menüpontot:");
            Console.WriteLine("1) Helyszínen vásárolok! Foglalás véglegesítése");
            Console.WriteLine("2) Most vásárolok");
            Console.WriteLine("3) Foglalás törlése / Vissza a főmenübe");
            Console.Write("\r\nKérlek válassz: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("A jegyeid ára: {0}\nKöszönjök a foglalást! A helyszínen várunk!", db * ÁR); //return false; // nincs több dolga, lefoglalta ajegyeket
                    break;
                case "2":
                    Vásárlás(db);
                    break;
                case "3":
                    Foglalás_Törlés("Vetítések.txt"); //TODO
                    break;
                default:
                    break;
            }

        }
        private static int[,] FoglaltSzékek(int vetítés_ID)
        {
            int[,] matrix = new int[8, 8];
            foreach (var item in Székek)
            {
                if (item.Vetítés_ID == vetítés_ID)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        for (int y = 0; y < matrix.GetLength(1); y++) //
                        {
                            if (item.Sor == x + 1)
                            {
                                if (item.Oszlop == y + 1)
                                {
                                    //Console.WriteLine("Foglalt: {0} {1}",x+1,y+1);
                                    matrix[x, y] = 1;
                                }
                            }
                        }
                    }
                }
            }

            return matrix;
        }
        private static void Székrajz(int[,] Foglalt)
        {
            for (int x = 0; x < Foglalt.GetLength(0); x++) //10 sor
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("{0} | ", x + 1);
                for (int y = 0; y < Foglalt.GetLength(1); y++) // 20 oszlop
                {
                    if (Foglalt[x, y] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" {0} ", (char)9632);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" {0} ", (char)9632);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
        }
        #endregion
        static int Kupon_felvitel()
        {
            Console.WriteLine("TESZT kupon: SWT-gyakorlat");
            Console.WriteLine("Kilépéshez írd: exit");
            Console.Write("Add meg a kuponod:");
            switch (Console.ReadLine())
            {
                case "SWT-gyakorlat":
                    Console.WriteLine("\n20% kedvezményre jogosító kupont adtál meg");
                    ÁR = ÁR * 0.8;
                    return 1;
                case "I_AM_THE_MESSIAH":
                    Console.WriteLine("\n100% kedvezményre jogosító kupont adtál meg");
                    ÁR = 0;
                    return 1;
                case "Exit": return 0;
                case "exit": return 0;
                case "EXIT": return 0;
                case "0": return 0;
                default:
                    Console.WriteLine("\nNem érvényes kupont adtál meg");
                    Console.WriteLine("Nyomj bármilyen gombot a továbblépéshez !");
                    Console.ReadKey();
                    return 1;
            }
        }
        static void Kupon() 
        {
            Console.WriteLine("Kérlek válaszd ki a kedvezményeid: ");
            Console.WriteLine("\t1) Kuponkód felvitele: ");
            Console.WriteLine("\t2) Nincs kuponkódom");
            Console.Write("\r\nKérlek válassz: ");
            switch (Console.ReadLine())
            {
                case "1":
                    int ciklus_változó = 1;
                    while (ciklus_változó == 1)
                    {
                        ciklus_változó = Kupon_felvitel();
                    }
                    break;
                case "2":
                    Console.WriteLine("Nyomj bármilyen gombot a továbblépéshez !");
                    Console.ReadKey();
                    break;
                default: 
                    Console.WriteLine("Hibás bevitel");
                    Console.WriteLine("Nyomj bármilyen gombot a továbblépéshez !");
                    Console.ReadKey();
                    break;
            }
        }
       
        static void Vásárlás(int db)
        {
            Console.Clear();
            PrintHeader("KOVI-D MOZI - VÁSÁRLÁS");
            Console.WriteLine("Az általad fizetendő összeg: {0}", db * ÁR);
            Console.WriteLine("Diák / Nyugdíjas kedvezményre jogosult vagy ? ");
            Console.WriteLine("\t1) Igen");
            Console.WriteLine("\t2) Nem ");
            Console.Write("\r\nKérlek válassz: ");
            if (Console.ReadLine() == "1") { ÁR = 750; }
            else{  ÁR = 1000;}
            Console.WriteLine("Az általad fizetendő összeg mostmár: {0}", db * ÁR);
            Kupon();
            Console.WriteLine("Az általad fizetendő összeg mostmár: {0}\n", db * ÁR);

            Console.WriteLine("Kérlek add meg a bankkártya adataid:\n");
            string számlaszám = Adatbe("Számlaszám: ",0);
            string lejárat = Adatbe("Lejárati-dátum: ",0);
            string fura_szám_a_hátulján = Adatbe("CVC kód: ",0);
            string  nev = Adatbe("Számlán szereplő név:  ",1);

            Console.WriteLine("Fizetés a következő bankkártyával:\n" +
                "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
                "| Kártyaszám: {0}\n" +
                "| Lejárat:    {1}\n" +
                "| CVC:        {2}\n" +
                "| Név:        {3}\n" +
                "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", számlaszám,lejárat,fura_szám_a_hátulján,nev);
            Console.WriteLine("A bankkártyadataid helyesek!\n");

            Console.WriteLine("Kérlek add meg az ellenőrzéshez szükséges adatokat:\n");
            string user_nev = Adatbe("Regisztrációkor megadott név: ",2);
            Console.WriteLine("Az utalás készen van! A jegye(ke)t elküldtük az e-mail címére.");

        }
        static string Adatbe(string szöveg, int kód) {
            int fiszfasz = 1; string adat = "";
            while (fiszfasz == 1)
            {
                Console.Write(szöveg); 
                adat = Console.ReadLine();
                fiszfasz = Adat_csekk(adat, kód);
            }
            return adat;
        }
        static bool Check_if_szám(string adatsor)
        {
            for (int i = 0; i < adatsor.Length; i++)
            {
                if (char.IsNumber(adatsor, i) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private static int Adat_csekk(string adatsor, int switch_válasz)
        {
            if (adatsor == null || adatsor == "")
            {
                Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                Console.ReadKey();
                return 1;
            }
            else 
            {
                switch (switch_válasz) // ha return / 0 akkor jó
                {
                    case 0: // számlaszám, lejárati dáum, cvc kód
                        if (Check_if_szám(adatsor) == true) { return 0; }
                        else { Console.WriteLine("Hibás bevitel"); return 1;}
                    case 1: //nev
                        if (Regex.IsMatch(adatsor, @"^[a-zA-Z]+$") == true) {return 0;}
                        else { Console.WriteLine("Hibás bevitel");  return 1; }
                    case 2: //nev
                        if (LOGGED_USER_NAME == adatsor) { return 0; }
                        else { Console.WriteLine("Hibás bevitel"); return 1; }
                    default:
                        Console.WriteLine("Hibás bevitel! Nyomj egy gombot a továbblépéshez");
                        Console.ReadKey();
                        return 1;
                }
            }
            
        }
       
        static void Foglalás_Törlés(string filepath)
        {
            List<string> lines = File.ReadAllLines(filepath).ToList();

            File.WriteAllLines(filepath, lines.GetRange(0, lines.Count - 1).ToArray());
        }
        public void Futtat() 
        {
            bool showMenu = true;
            Feltölt();
            while (showMenu)
            {
                showMenu = MainMenu();
            }
            Olvasó.Close();
        }
        static void Main(string[] args)
        {

            Program Application = new Program();
            Application.Futtat();
            Environment.Exit(0);
        }
    }
}