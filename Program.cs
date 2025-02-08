namespace Game
{
    abstract class GameObject
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameObject(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }

    class Hero : GameObject
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Stamina { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int Wood { get; set; }
        public List<string> Inventory { get; set; }
        private bool _hasStaminaWarning; // Чи було попередження про низьку витривалість

        public Hero(string name, int x, int y) : base(name, x, y)
        {
            Strength = 10;
            Agility = 10;
            Stamina = 10;
            Health = 100;
            Gold = 0;
            Wood = 0;
            Inventory = new List<string>();
            _hasStaminaWarning = false;
        }

        public void CollectResource(Resource resource)
        {
            if (resource is Gold)
            {
                Gold += resource.Amount;
                Console.WriteLine($"Ви знайшли {resource.Amount} золота!");
            }
            else if (resource is Wood)
            {
                Wood += resource.Amount;
                Inventory.Add($"Дерево ({resource.Amount})");
                Console.WriteLine($"Ви знайшли {resource.Amount} дерева!");
            }
        }

        public void Fight(Enemy enemy)
        {
            if (enemy is Boss && Stamina < 5)
            {
                Console.WriteLine("У вас недостатньо витривалості для бою з босом! Ви помираєте...");
                Die();
                return;
            }

            Console.WriteLine($"Бій з {enemy.Name}!");
            if (Strength > enemy.Strength)
            {
                Console.WriteLine($"Ви перемогли {enemy.Name}!");
                Gold += enemy.Reward;
                if (enemy is Goblin)
                {
                    if (new Random().Next(0, 2) == 0)
                    {
                        Health -= 1;
                        Console.WriteLine("Гоблін забрав 1 здоров'я!");
                    }
                }
                else if (enemy is Boss)
                {
                    Stamina -= 5; // Витрата витривалості за бій з босом
                    if (new Random().Next(0, 4) < 3) // 75% шанс
                    {
                        AddToInventory("Проклятий шлем");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Ви програли {enemy.Name}... Гра закінчена.");
                Die();
            }
        }

        public void AddToInventory(string item)
        {
            Inventory.Add(item);
            Console.WriteLine($"{item} додано до інвентаря.");
        }

        public void ShowInventory()
        {
            Console.WriteLine("Ваш інвентар:");
            foreach (var item in Inventory)
            {
                Console.WriteLine($"- {item}");
            }
        }

        public void Rest()
        {
            Stamina += 3;
            Console.WriteLine($"Ви перепочили. Витривалість відновлено на 2. Поточна витривалість: {Stamina}");
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Здоров'я: {Health}, Витривалість: {Stamina}");
        }

        public void CheckStamina()
        {
            if (Stamina <= 0)
            {
                if (_hasStaminaWarning)
                {
                    Console.WriteLine("Ваша витривалість впала до 0 вдруге. Ви помираєте...");
                    Die();
                }
                else
                {
                    Console.WriteLine("Ви впали без сил! Наступного разу це призведе до смерті.");
                    _hasStaminaWarning = true;
                    Stamina = 1; // Відновлюємо витривалість до 1, щоб уникнути миттєвої смерті
                }
            }
        }

        public void Die()
        {
            Console.WriteLine("Ви померли. Введіть 'return', щоб перезапустити гру, або будь-що інше для виходу.");
            string input = Console.ReadLine();
            if (input.ToLower() == "return")
            {
                Program.RestartGame();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }

    // вороги
    class Enemy : GameObject
    {
        public int Strength { get; set; }
        public int Reward { get; set; }

        public Enemy(string name, int x, int y, int strength, int reward) : base(name, x, y)
        {
            Strength = strength;
            Reward = reward;
        }
    }

    // Клас для гобліна
    class Goblin : Enemy
    {
        public Goblin(int x, int y) : base("Goblin", x, y, 5, 10) { }
    }

    // Клас для боса
    class Boss : Enemy
    {
        public int BossFightCount { get; set; }

        public Boss(int x, int y) : base("Boss", x, y, 30, 100)
        {
            BossFightCount = 0;
        }

        public void FightHero(Hero hero)
        {
            BossFightCount++;
            if (BossFightCount == 1)
            {
                hero.Health /= 2;
                Console.WriteLine($"Бос забрав половину вашого здоров'я! Здоров'я: {hero.Health}");
            }
            else if (BossFightCount == 2)
            {
                if (new Random().Next(0, 2) == 0)
                {
                    Console.WriteLine("Ви померли після другої спроби вбити боса!");
                    hero.Die();
                }
                else
                {
                    Console.WriteLine($"Ви перемогли {Name}!");
                    hero.Gold += Reward;
                }
            }
        }
    }

    // Клас для Вбивці
    class Assassin : Enemy
    {
        public Assassin(int x, int y) : base("Assassin", x, y, 15, 20) { }

        public void AttackHero(Hero hero)
        {
            Console.WriteLine($"{Name} атакує вас!");
            if (new Random().Next(0, 2) == 0) // 50% шанс вбити героя
            {
                Console.WriteLine($"{Name} вбив вас!");
                hero.Die();
            }
            else
            {
                Console.WriteLine($"{Name} промахнувся!");
            }
        }
    }

    abstract class Resource : GameObject
    {
        public int Amount { get; set; }

        public Resource(string name, int x, int y, int amount) : base(name, x, y)
        {
            Amount = amount;
        }
    }

    //золотo
    class Gold : Resource
    {
        public Gold(int x, int y, int amount) : base("Gold", x, y, amount) { }
    }

    //деревo
    class Wood : Resource
    {
        public Wood(int x, int y, int amount) : base("Wood", x, y, amount) { }
    }
//магазин
    class Shop : GameObject
    {
        public Shop(int x, int y) : base("Shop", x, y) { }

        public void OpenShop(Hero hero)
        {
            Console.WriteLine("Ласкаво просимо до магазину!");
            Console.WriteLine("1. Зілля зцілення (2 золота)");
            Console.WriteLine("2. Вийти");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (hero.Gold >= 2)
                    {
                        hero.Gold -= 2;
                        hero.Health = 100;
                        Console.WriteLine("Ви купили зілля зцілення! Ваше здоров'я повністю відновлено.");
                    }
                    else
                    {
                        Console.WriteLine("У вас недостатньо золота.");
                    }
                    break;
                case "2":
                    Console.WriteLine("Дякуємо за візит!");
                    break;
                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
        }
    }

    //грa
    class Game
    {
        private Hero hero;
        private List<GameObject> gameObjects;
        private char[,] map;

        public Game()
        {
            hero = new Hero("Hero", 0, 0);
            gameObjects = new List<GameObject>
            {
                new Gold(1, 1, 5),
                new Wood(2, 2, 3),
                new Goblin(3, 3),
                new Boss(5, 5),
                new Goblin(7, 7),
                new Goblin(10, 10),
                new Boss(15, 15),
                new Shop(12, 12),
                new Assassin(18, 18) 
            };

            map = new char[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    map[i, j] = '█';
                }
            }

            // Розміщення об'єктів
            map[hero.X, hero.Y] = 'Г';
            map[1, 1] = 'C'; // Gold
            map[2, 2] = 'C'; // Wood
            map[3, 3] = 'M'; // Goblin
            map[5, 5] = 'B'; // Boss
            map[7, 7] = 'M';
            map[10, 10] = 'M';
            map[15, 15] = 'B';
            map[12, 12] = 'S'; // Shop
            map[18, 18] = 'A'; // Assassin
        }

        public void Start()
        {
            while (true)
            {
                hero.Stamina -= 1;
                hero.CheckStamina();
                hero.ShowStatus();
                PrintMap();
                Console.WriteLine($"Поточна позиція: ({hero.X}, {hero.Y})");
                Console.WriteLine("Доступні дії:");
                Console.WriteLine("1. Рух");
                Console.WriteLine("2. Видобути ресурс");
                Console.WriteLine("3. Бій з ворогом");
                Console.WriteLine("4. Показати інвентар");
                Console.WriteLine("5. Перепочити");
                Console.WriteLine("6. Відвідати магазин");
                Console.WriteLine("7. Показати золото");
                Console.WriteLine("8. показати правила і умовності гри");
                Console.WriteLine("9. Вийти");
                Console.WriteLine("Для комфортної гри відкрийте консоль на весь екран.");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        MoveHero();
                        break;
                    case "2":
                        CollectResource();
                        break;
                    case "3":
                        FightEnemy();
                        break;
                    case "4":
                        hero.ShowInventory();
                        break;
                    case "5":
                        hero.Rest();
                        break;
                    case "6":
                        VisitShop();
                        break;

                    case "7":
                        Console.WriteLine($"Золото: {hero.Gold}");
                        break;
                    case "8":
                        Console.WriteLine($"Герой має параметри: сила, спритність, витривалість, здоров’я, золото, дерево та інвентар.\r\nГерой може збирати ресурси, боротися з ворогами, перепочивати, відвідувати магазин та помирати.\r\nПри падінні витривалості до 0 спрацьовує попередження; якщо це повторюється, герой помирає.\r\nРесурси:\r\n\r\nДоступні типи ресурсів: золото та дерево.\r\nГерой може збирати ресурси, перебуваючи на одній позиції з ними на карті.\r\nВороги:\r\n\r\nЄ кілька типів ворогів: гоблін, бос та вбивця.\r\nГобліни можуть забирати 1 здоров'я після поразки.\r\nБій із босом вимагає витрати витривалості (мінімум 5). Якщо витривалості недостатньо, герой помирає.\r\nПеремога над босом має шанс (75%) додати до інвентаря \"Проклятий шлем\".\r\nБій із босом другого рівня може завершитися миттєвою смертю.\r\nВбивця має 50% шанс вбити героя.\r\nМагазин:\r\n\r\nГерой може відвідувати магазин для купівлі зілля зцілення за 2 золота, яке повністю відновлює здоров’я.\r\nКарта:\r\n\r\nПредставлена у вигляді матриці 20x20.\r\nНа карті розташовані герої, ресурси, вороги та магазин.\r\nПересування:\r\n\r\nГерой може рухатися вліво (a), вправо (d), вгору (w), вниз (s).\r\nРух за межі карти неможливий.\r\nІнвентар:\r\n\r\nГерой може переглядати та додавати предмети до інвентаря.\r\nСистема бою:\r\n\r\nЯкщо сила героя перевищує силу ворога, він перемагає.\r\nПеремога над ворогом приносить нагороду у вигляді золота.\r\nПоразка завершує гру.\r\nВідпочинок:\r\n\r\nВідновлює витривалість героя на 2 одиниці.\r\nСтатус:\r\n\r\nГерой може переглядати поточні показники здоров’я та витривалості.\r\nЗакінчення гри:\r\nГра завершується після смерті героя або за вибором користувача.\r\nПісля смерті гравець може перезапустити гру або вийти.");
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }
            }
        }

        private void PrintMap()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Console.Write(map[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        private void MoveHero()
        {
            Console.WriteLine("Виберіть напрямок (a - вліво, w - вгору, s - вниз, d - вправо):");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            int newX = hero.X;
            int newY = hero.Y;

            switch (keyInfo.Key)
            {
                case ConsoleKey.A:
                    newX--;
                    break;
                case ConsoleKey.W:
                    newY++;
                    break;
                case ConsoleKey.S:
                    newY--;
                    break;
                case ConsoleKey.D:
                    newX++;
                    break;
                default:
                    Console.WriteLine("Невірна клавіша.");
                    return;
            }

            if (newX >= 0 && newX < 20 && newY >= 0 && newY < 20)
            {
                map[hero.X, hero.Y] = '█';  // Очищаємо стару позицію героя
                hero.X = newX;
                hero.Y = newY;
                map[hero.X, hero.Y] = 'Г';  // Оновлюємо нову позицію героя
            }
            else
            {
                Console.WriteLine("Рух за межі карти неможливий.");
            }
        }

        private void CollectResource()
        {
            foreach (var obj in gameObjects)
            {
                if (obj is Resource resource && hero.X == resource.X && hero.Y == resource.Y)
                {
                    hero.CollectResource(resource);
                    map[hero.X, hero.Y] = '█';
                    return;
                }
            }
            Console.WriteLine("Ресурсів немає на цій позиції.");
        }

        private void FightEnemy()
        {
            foreach (var obj in gameObjects)
            {
                if (obj is Enemy enemy && hero.X == enemy.X && hero.Y == enemy.Y)
                {
                    if (enemy is Boss boss)
                    {
                        boss.FightHero(hero);
                    }
                    else if (enemy is Assassin assassin)
                    {
                        assassin.AttackHero(hero);
                    }
                    else
                    {
                        hero.Fight(enemy);
                    }
                    map[hero.X, hero.Y] = '#';
                    return;
                }
            }
            Console.WriteLine("Ворогів немає на цій позиціі.");
        }

        private void VisitShop()
        {
            foreach (var obj in gameObjects)
            {
                if (obj is Shop shop && hero.X == shop.X && hero.Y == shop.Y)
                {
                    shop.OpenShop(hero);
                    return;
                }
            }
            Console.WriteLine("Магазину немає на цій позиції.");
        }
    }

    // Точка входу
    class Program
    {
        public static void RestartGame()
        {
            Console.Clear();
            Console.WriteLine("Перезапуск гри...");
            Game game = new Game();
            game.Start();
        }

        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}



