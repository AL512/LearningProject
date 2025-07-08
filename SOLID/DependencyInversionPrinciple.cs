using SOLID.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace SOLID;

public class DependencyInversionPrinciple
{
    /*
    Суть принципа:
    1) Модули верхнего уровня не должны зависеть от модулей нижнего уровня. Оба должны зависеть от абстракций
    2) Абстракции не должны зависеть от деталей. Детали должны зависеть от абстракций

    Ключевая идея: Создание гибкой архитектуры через "переворачивание" традиционных зависимостей

    Почему DIP критически важен
    1) Снижение связанности: Компоненты зависят от абстракций, а не конкретных реализаций
    2) Упрощение тестирования: Легкая замена реальных зависимостей mock-объектами
    3) Гибкость системы: Замена реализаций без изменения основного кода
    4) Улучшение сопровождаемости: Изолированные изменения
    5) Повышение переиспользуемости: Независимые компоненты

    Пример 1: Система обработки заказов
    Нарушение DIP:
     */

    public class OrderProcessor1
    {
        private readonly SqlServerDatabase _database;
        private readonly SmtpEmailService _emailService;

        public OrderProcessor1()
        {
            _database = new SqlServerDatabase();
            _emailService = new SmtpEmailService();
        }

        public void Process(Order order)
        {
            _database.Save(order);
            _emailService.SendEmail(order.CustomerEmail, "Order processed");
        }
    }

    /*
    Проблемы:
    - Жесткая привязка к SQL Server и SMTP
    - Невозможно протестировать без реальных сервисов

    Соблюдение DIP:
    */

    // Абстракции
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    public interface INotificationService
    {
        void SendNotification(string recipient, string message);
    }

    // Реализации
    public class SqlOrderRepository : IOrderRepository
    {
        public void Save(Order order) => Console.WriteLine("Saving to SQL...");
    }

    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string message)
            => Console.WriteLine($"Sending email to {recipient}");
    }

    // Основная логика
    public class OrderProcessor
    {
        private readonly IOrderRepository _repository;
        private readonly INotificationService _notification;

        public OrderProcessor(
            IOrderRepository repository,
            INotificationService notification)
        {
            _repository = repository;
            _notification = notification;
        }

        public void Process(Order order)
        {
            _repository.Save(order);
            _notification.SendNotification(order.CustomerEmail, "Order processed");
        }
    }

    // Конфигурация в .NET 8
    /*

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
    builder.Services.AddScoped<INotificationService, EmailNotificationService>();

    Пример 2: Кэширование данных с DIP
    */

    // Абстракция
    public interface ICacheProvider
    {
        void Set(string key, object value, TimeSpan expiry);
        T Get<T>(string key);
    }

    // Реализации
    public class MemoryCacheProvider : ICacheProvider
    {
        public void Set(string key, object value, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }
    }

    public class RedisCacheProvider : ICacheProvider
    {
        public void Set(string key, object value, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }
    }

    public class DistributedCacheProvider : ICacheProvider
    {
        public void Set(string key, object value, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDataRepository
    {
        Data GetData(string key);
    }

    // Сервис с инверсией зависимости
    public class DataService
    {
        private readonly ICacheProvider _cache;
        private readonly IDataRepository _repository;

        public DataService(ICacheProvider cache, IDataRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public Data GetData(string key)
        {
            var data = _cache.Get<Data>(key);
            if (data == null)
            {
                data = _repository.GetData(key);
                _cache.Set(key, data, TimeSpan.FromMinutes(30));
            }

            return data;
        }
    }

    /*
    Ключевые техники реализации DIP
    1. Конструкторная инъекция (наиболее распространенная)
    */

    public class ReportGenerator
    {
        private readonly IDataRepository _dataProvider;

        public ReportGenerator(IDataRepository dataProvider)
            => _dataProvider = dataProvider;

        public Report Generate()
            => new Report(_dataProvider.GetData("key1"));
    }

    /*
    2. Инъекция через свойства
    */

    public class ApiClient
    {
        public ILogger Logger { get; set; } = NullLogger.Instance;

        public void Execute()
        {
            try
            {
                /* ... */
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }

    /*
    3. Инъекция через методы
    */

    public interface IImageFilter
    {
        void Apply();
    }

    public class ImageProcessor
    {
        public void Process(IImageFilter filter)
            => filter.Apply();
    }

    /*
    4. Контейнер внедрения зависимостей
    */

    /*
    var builder = WebApplication.CreateBuilder(args);

    // Регистрация зависимостей
    builder.Services.AddSingleton<ILogger, FileLogger>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IEmailService, SendGridEmailService>();

    // Автоматическая регистрация сборки
    builder.Services.AddImplementationsOf(typeof(IDomainService));

    var app = builder.Build();
    */

    /*
    Паттерны проектирования для реализации DIP

    1) Адаптер (Adapter):
    */

    public interface IModernInterface
    {
    }

    public class LegacySystemAdapter : IModernInterface
    {
        private readonly LegacySystem _legacy;

        public LegacySystemAdapter(LegacySystem legacy)
            => _legacy = legacy;

        public void NewMethod()
            => _legacy.OldMethod();
    }

    /*
    2) Фабрика (Factory):
    */

    public interface IServiceFactory
    {
        OpenClosedPrinciple.IDataService Create();
    }

    public class DataServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _provider;

        public DataServiceFactory(IServiceProvider provider)
            => _provider = provider;

        public OpenClosedPrinciple.IDataService Create()
            => _provider.GetRequiredService<OpenClosedPrinciple.IDataService>();
    }

    /*
    3) Стратегия (Strategy):
    */

    public interface IPaymentStrategy
    {
        void Process(decimal amount);
    }

    public class PaymentProcessor
    {
        private readonly IPaymentStrategy _strategy;

        public PaymentProcessor(IPaymentStrategy strategy)
            => _strategy = strategy;

        public void ExecutePayment(decimal amount)
            => _strategy.Process(amount);
    }

    /*
    Ошибки при реализации DIP
    1) Инжектирование конкретных классов:
    */

    // Антипаттерн!
    public class UserService
    {
        private readonly SqlUserRepository _repository; // Должен быть интерфейс

        public UserService(SqlUserRepository repository)
            => _repository = repository;
    }

    /*
    2) Сервис локатор (Service Locator):
    */

    // Нежелательный подход
    public class OrderService
    {
        private readonly IRepository _repository;

        public OrderService()
        {
            _repository = ServiceLocator.Resolve<IRepository>();
        }
    }

    /*
    3) Чрезмерное дробление интерфейсов:
    */

    // Избыточное разделение
    public interface IIdGetter
    {
        int GetId();
    }

    public interface INameGetter
    {
        string GetName();
    }

    // Лучше:
    public interface IBasicInfo
    {
        int Id { get; }
        string Name { get; }
    }

    /*
    DIP — фундаментальный принцип для создания гибких, тестируемых и поддерживаемых приложений.
    Его правильное применение позволяет строить системы, которые могут эволюционировать годами без значительных переделок.
    */
}