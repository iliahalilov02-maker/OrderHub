OrderHub — описание проекта и структура
1. Что это за проект

OrderHub — учебный консольный проект на C# (.NET), демонстрирующий девять этапов изучения ключевых механизмов языка и базовых структур данных:
Этап	Тема	Что демонстрируется
1–2	Dictionary<TKey,TValue>	Склад: TryGetValue, индексатор, Remove, KeyNotFoundException
3	Queue<T> / Stack<T>	Заказы: FIFO-очередь, LIFO-история, Enqueue/Dequeue/Push/Pop
4	IComparable<T> / IComparer<T>	Три порядка сортировки каталога: естественный, альтернативный, лямбда
5	LINQ: базовые запросы	Where → OrderBy → Select, агрегаты Count/Any/All/First
6	LINQ: группировка	GroupBy, агрегаты внутри групп, проекция в анонимный тип
7	LINQ: Join	Соединение клиентов с заказами, Take (топ-N), агрегаты после Join
8	Итераторы	yield return, ленивость, бесконечный генератор + Take(N)
9	Пользовательские итераторы	CircularBuffer<T>: IEnumerable<T>, прямой и обратный обход

Идея: не девять разрозненных задач, а один связный проект, где каждый этап опирается на предыдущий. Склад → заказы → каталог → отчёты → поток заказов → кольцевой буфер.

Тип приложения: консольное (OutputType: Exe), без БД, без сети. Все данные — в памяти, создаются в SeedData и Program.cs.

## Структура решения

```text
OrderHub/
├── .gitignore
├── OrderHub.sln
├── README.md
├── docs/
│   └── EADME.md                      # Конспект теории
├── src/
│   ├── Collections/
│   │   └── CircularBuffer.cs         # Этап 9: кольцевой буфер
│   ├── Data/
│   │   └── SeedData.cs               # Демо-данные
│   ├── Domain/
│   │   ├── Product.cs                # Товар + компараторы
│   │   ├── Customer.cs               # Клиент
│   │   └── Order.cs                  # Заказ
│   ├── Services/
│   │   ├── WarehouseService.cs       # Склад
│   │   ├── OrderService.cs           # Заказы (Queue/Stack)
│   │   ├── CatalogService.cs         # Каталог (Сортировки)
│   │   ├── ReportService.cs          # LINQ-отчёты
│   │   └── OrderStream.cs            # Итераторы
│   └── Program.cs                    # Точка входа
└── tests/                            # Тесты

    Замечание по текущему состоянию: в src/Domain/Customer.cs по ошибке лежит два класса — Customer и OrderService. Их нужно разделить: Customer → Domain/Customer.cs, OrderService → Services/OrderService.cs. Сейчас OrderService недописан (нет CreateOrder, ProcessNext, PrintAllOrders и т.д.), а Program.cs их вызывает — проект не компилируется.

3. Назначение каждого слоя
Domain/ — доменные сущности

Что: чистые модели данных, без зависимостей от инфраструктуры. Не знают ни про консоль, ни про LINQ-сервисы.
Файл	Класс	Роль
Product.cs	Product	Товар: Name, Price, Category. IComparable<Product> — естественный порядок по цене
Product.cs	NameComparer	IComparer<Product> — порядок по алфавиту
Product.cs	PriceDescComparer	IComparer<Product> — по убыванию цены
Customer.cs	Customer	Клиент: Id, Name, City
Order.cs	Order	Заказ: Id, CustomerId, CustomerName, ProductName, Price, CreatedAt

Принципы:

    Свойства Product — private set (неизменяемы после создания), у Order/Customer — get; set; (могут обновляться).

    У Product — конструктор с параметрами (единственный способ создать).

    У Order — два конструктора: без параметров (для инициализатора объекта) и с параметрами (для сервисов).

    ToString() переопределён у всех — для читаемого вывода.

Services/ — бизнес-логика

Что: операции над доменными объектами. Каждый сервис отвечает за свою область.
Файл	Класс	Ответственность	Структуры данных
WarehouseService.cs	WarehouseService	Приём/продажа товара, остатки	Dictionary<string, int>
OrderService.cs	OrderService	Создание/обработка заказов, история	List<Order>, Queue<Order>, Stack<string>, LinkedList<Order>
CatalogService.cs	CatalogService	Каталог товаров, три сортировки	List<Product>
ReportService.cs	ReportService	LINQ-отчёты: фильтры, группировки, join	(не хранит, принимает IEnumerable)
OrderStream.cs	OrderStream	Генераторы последовательностей заказов	yield return

Принципы:

    Хранилища — private readonly (инкапсуляция: снаружи нельзя Clear()).

    Публичные методы — единственный способ изменить состояние.

    ReportService — stateless (не хранит данные, принимает IEnumerable параметром) — легко тестировать.

    OrderStream — static методы, потому что состояние не нужно.

Collections/ — пользовательские структуры данных
Файл	Класс	Роль
CircularBuffer.cs	CircularBuffer<T>	Кольцевой буфер фиксированной ёмкости. IEnumerable<T>, прямой и обратный обход через yield return

Ключевые формулы:

    Куда писать: next

    Сдвиг: (next + 1) % capacity

    Самый старый: (next - size + capacity) % capacity

    Самый новый: (next - 1 + capacity) % capacity

Data/ — демо-данные

Что: статические методы/константы, возвращающие готовые наборы товаров, клиентов, заказов для Program.cs. Отделяет «данные» от «логики» — если завтра данные придут из БД, меняется только этот слой.
Program.cs — точка входа

Что: последовательная демонстрация всех девяти этапов, разбитая на части:
Часть	Этап	Что вызывает
1	Склад	WarehouseService.Receive/Sell/PrintStock
2	Заказы	OrderService.CreateOrder/ProcessNext/Print*
3	Каталог	CatalogService.PrintAll/PrintSortedBy*
4	Отчёты	ReportService.ExpensiveProducts/SummarizeByCategory/...
5	Join	ReportService.TopCustomers/RevenueForYear/TopCustomerByOrderCount/AboveAverageOrders
6	Итераторы	OrderStream.Orders().Take(5), OrderStream.Expensive(...)
7	Кольцевой буфер	CircularBuffer<string>.Add/foreach/Reverse()

Принцип: Program.cs — только демонстрация. Никакой бизнес-логики в нём нет: всё в сервисах. Если убрать Program.cs, проект всё равно соберётся как библиотека.
docs/ — конспект

EADME.md — 11 вопросов-ответов по темам этапов: Queue vs Stack, AddLast в LinkedList, KeyNotFoundException, IComparable vs IComparer, ленивость LINQ, IGrouping, Join, бесконечные итераторы, явная реализация IEnumerable.GetEnumerator, формула кольцевого буфера, преимущества единого проекта.
tests/ — заготовка

Пока пусто. Планируется: юнит-тесты на WarehouseService.Sell (граничные случаи), CircularBuffer (перезапись, обход), ReportService (пустые коллекции, границы threshold).
4. Поток данных между слоями
text

Program.cs
   │
   ├── new WarehouseService() ──► Receive/Sell ──► Dictionary<string,int> (внутри)
   │
   ├── new OrderService() ─────► CreateOrder ──► List + Queue + Stack (внутри)
   │
   ├── new CatalogService() ───► AddProduct ──► List<Product> (внутри)
   │                              PrintSortedBy* ──► Sort (IComparable / IComparer / лямбда)
   │
   ├── new ReportService() ────► принимает IEnumerable<Product>/<Order>/<Customer>
   │                              возвращает List<string>/<CategorySummary>/<Order>
   │
   ├── OrderStream.Orders() ───► IEnumerable<Order> (бесконечный, ленивый)
   │
   └── new CircularBuffer<T>() ► Add / foreach / Reverse()

Зависимости:
text

Program.cs ──► Services ──► Domain
Program.cs ──► Collections (независимо)
Services ──► Domain (не наоборот!)
Domain ──► ничего (кроме System)
Collections ──► ничего (кроме System.Collections)

Правило: Domain не знает про Services. Services не знают про Program. Collections — самостоятельны. Это слоистая архитектура с однонаправленными зависимостями.
5. Что демонстрирует каждый сервис — по этапам
WarehouseService (этапы 1–2)
csharp

private Dictionary<string, int> stock;
public void Receive(string name, int qty)   // TryGetValue + индексатор
public bool Sell(string name, int qty)      // проверки + Remove при 0
public bool TryGetStock(string name, out int qty)  // out во всех ветках
public void PrintStock()                    // foreach + OrderBy

Ключевые идеи: TryGetValue вместо ContainsKey + индексатор (один поиск), Remove при нулевом остатке, out-параметр инициализируется всегда.
OrderService (этап 3)
csharp

private List<Order> allOrders;
private Queue<Order> pendingQueue;   // FIFO — обработка в порядке поступления
private Stack<string> undoHistory;   // LIFO — последнее действие первым
private int nextOrderId;             // счётчик, не Count+1 (иначе повторы при удалении)

public Order CreateOrder(...)        // Add + Enqueue + Push
public Order? ProcessNext()          // Dequeue + Push в историю
public bool UndoLastAction()         // Pop из истории
public void PrintAllOrders()         // foreach по List
public void PrintPendingQueue()      // Peek — без извлечения
public void PrintRecentActions()     // foreach по Stack — от вершины

Ключевые идеи: Queue — FIFO, Stack — LIFO, Peek vs Dequeue/Pop, отдельный счётчик ID.
CatalogService (этап 4)
csharp

private List<Product> products;

public void PrintSortedByNaturalOrder()   // products.Sort() — IComparable
public void PrintSortedByName()           // products.Sort(new NameComparer()) — IComparer
public void PrintSortedByNameLength()     // products.Sort((a,b) => ...) — Comparison<T>
public void PrintAll()                    // исходный порядок

Ключевые идеи: три механизма сортировки, Sort мутирует список, IComparable vs IComparer vs лямбда.
ReportService (этапы 5–7)
Метод	Этап	Операторы
ExpensiveProducts	5	Where → OrderBy → Select → ToList
CountExpensive / HasVeryExpensive / AllInStock	5	Count / Any / All (немедленные)
Cheapest	5	OrderBy → FirstOrDefault
SummarizeByCategory / MostExpensiveCategory / CategoriesWithAtLeast	6	GroupBy → Select с агрегатами → OrderByDescending
TopCustomers / TopCustomerByOrderCount	7	Join → GroupBy → Sum/Count → OrderByDescending → Take/FirstOrDefault
RevenueForYear / AboveAverageOrders	7	Where + Sum / Average

Ключевые идеи: ленивые vs немедленные операторы, GroupBy как IEnumerable<IGrouping>, Join — inner join, Take после OrderBy.
OrderStream (этап 8)
csharp

public static IEnumerable<Order> Orders()                    // бесконечный while(true) + yield
public static IEnumerable<Order> Expensive(int limit, decimal threshold)  // for + continue + yield

Ключевые идеи: ленивость (yield приостанавливает выполнение), бесконечные последовательности + Take(N), ToList() на бесконечном — зависание.
CircularBuffer<T> (этап 9)
csharp

private T[] buffer;
private int next, size;
private readonly int capacity;

public void Add(T item)                    // запись + сдвиг + size++
public IEnumerator<T> GetEnumerator()      // прямой обход через yield
public IEnumerable<T> Reverse()            // обратный обход через yield
IEnumerator IEnumerable.GetEnumerator()    // явная реализация — для foreach

Ключевые идеи: кольцевые индексы (x + capacity) % capacity, foreach требует public GetEnumerator, явная реализация для неуниверсального IEnumerable.
6. Что нужно исправить (текущее состояние)
Файл	Проблема	Как исправить
ReportService.cs	class ReportServices;, list<string>, .where(...), лишняя }	Исправить на class ReportService, List<string>, .Where(...), убрать лишнюю }
Domain/Customer.cs	Содержит два класса: Customer и OrderService	Разделить: Customer → Domain/Customer.cs, OrderService → Services/OrderService.cs
Services/OrderService.cs	Нет CreateOrder, ProcessNext, PrintAllOrders, PrintPendingQueue, PrintRecentActions, InitHistory	Дописать (см. предыдущий ответ)
Domain/Order.cs	Нет конструктора без параметров — new Order { ... } в Program.cs не скомпилируется	Добавить public Order() { }
Domain/Order.cs	CreatedAt = DateTime.Now игнорирует переданный createdAt	CreatedAt = createdAt
Domain/Product.cs	Нет using System;	Добавить
Services/WarehouseService.cs	Нет namespace OrderHub.Services;	Добавить
Program.cs	service.InitHistory(...) — метода нет	Реализовать или удалить вызов
Services/OrderService.cs	console.WriteLine (маленькая c), orderHistory не заполняется	Console.WriteLine, orderHistory.AddLast(order) в CreateOrder
7. Как запустить
bash

# из корня решения
dotnet build        # собрать
dotnet run          # запустить демонстрацию

Ожидаемый порядок вывода (после исправлений):

    Остатки на складе (отсортированы по алфавиту)

    Все заказы, первый в очереди, история действий

    Каталог в исходном порядке, три отсортированных варианта

    Отчёты: дорогие товары, сводка по категориям, топ-2 клиента, выручка по годам

    Итераторы: «Готовлю заказ №1…5», дорогие заказы (пусто при 10, 10000m)

    Кольцевой буфер: прямой 3 4 5, обратный 5 4 3, Count = 3, IsFull = True, LINQ: [3], [4], [5]

8. Ключевые принципы проекта

    Слоистость: Domain → Services → Program. Зависимости — только вниз.

    Инкапсуляция: все хранилища private, доступ только через методы.

    Один класс — один файл (кроме Product.cs с компараторами — они тесно связаны с Product).

    Program.cs — только демонстрация, вся логика — в сервисах.

    Доменные модели не зависят от инфраструктуры — Product не знает про CatalogService.

    ReportService — stateless (принимает IEnumerable параметром) — легко тестировать и переиспользовать.

    Каждый этап опирается на предыдущий — не изолированные задачи, а эволюция одного проекта.

