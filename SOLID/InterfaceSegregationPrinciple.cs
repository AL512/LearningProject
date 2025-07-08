using SOLID.Support;

namespace SOLID;

public class InterfaceSegregationPrinciple
{
    /*
    Суть принципа:
    Клиенты не должны зависеть от интерфейсов, которые они не используют.
    Вместо создания "толстых" интерфейсов, следует проектировать множество специализированных интерфейсов.

    Почему это важно:
    - Избегает принудительной реализации ненужных методов
    - Уменьшает связность компонентов
    - Предотвращает загрязнение кода пустыми реализациями
    - Повышает гибкость системы
    - Упрощает тестирование

    Глубокий анализ проблемы "толстых интерфейсов"

    Типичные симптомы нарушения ISP:
    - Пустые реализации методов (throw new NotImplementedException)
    - Интерфейсы с >5-7 методами
    - Клиенты используют только подмножество методов интерфейса
    - Изменения в интерфейсе затрагивают не связанные классы
    - Тесты требуют заглушек для неиспользуемых методов

    Последствия нарушений:
    - Хрупкие архитектуры
    - Трудности при рефакторинге
    - Ненужные зависимости
    - Нарушение принципа единственной ответственности

    Подробные примеры с решениями
    Пример 1: Система управления устройствами (нарушение ISP)
    */

    public interface IMultiFunctionDevice
    {
        void Print(Document document);
        void Scan(Document document);
        void Fax(Document document);
        void Email(Document document);
    }

    public class BasicPrinter1 : IMultiFunctionDevice
    {
        public void Print(Document document)
        {
            /* Реализация */
        }

        // Пустые реализации - нарушение ISP
        public void Scan(Document document)
            => throw new NotImplementedException();

        public void Fax(Document document)
            => throw new NotImplementedException();

        public void Email(Document document)
            => throw new NotImplementedException();
    }

    /*
    Решение через ISP:
    */

    public interface IPrinter
    {
        void Print(Document document);
    }

    public interface IScanner
    {
        void Scan(Document document);
    }

    public interface IFax
    {
        void Fax(Document document);
    }

    public interface IEmailSender
    {
        void Email(Document document);
    }

    // Для базового принтера
    public class BasicPrinter : IPrinter
    {
        public void Print(Document document)
        {
            /* ... */
        }
    }

    // Для многофункционального устройства
    public class OfficeMachine : IPrinter, IScanner, IFax, IEmailSender
    {
        public void Print(Document document)
        {
            /* ... */
        }

        public void Scan(Document document)
        {
            /* ... */
        }

        public void Fax(Document document)
        {
            /* ... */
        }

        public void Email(Document document)
        {
            /* ... */
        }
    }

    // Клиенты используют только нужные интерфейсы
    public class PrintService
    {
        private readonly IPrinter _printer;

        public PrintService(IPrinter printer) => _printer = printer;

        public void Execute(Document doc) => _printer.Print(doc);
    }

    /*
    Пример 2: Система управления пользователями (реальный кейс)
    Проблемный интерфейс:
    */

    public interface IUserService
    {
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        void LockUser(int id);
        void UnlockUser(int id);
        void ResetPassword(int id);
        void AssignRole(int userId, string role);
        void GenerateReport(UserFilter filter);
    }

    /*
    Решение через ISP:
    */

    public interface IUserCRUD
    {
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
    }

    public interface IUserSecurity
    {
        void LockUser(int id);
        void UnlockUser(int id);
        void ResetPassword(int id);
    }

    public interface IUserRoles
    {
        void AssignRole(int userId, string role);
    }

    public interface IUserReporting
    {
        void GenerateReport(UserFilter filter);
    }

    // Реализация для разных клиентов
    public class AdminPanelService : IUserCRUD, IUserSecurity, IUserRoles, IUserReporting
    {
        // Все методы
        public void CreateUser(User user)
        {
        }

        public void UpdateUser(User user)
        {
        }

        public void DeleteUser(int id)
        {
        }

        public void LockUser(int id)
        {
        }

        public void UnlockUser(int id)
        {
        }

        public void ResetPassword(int id)
        {
        }

        public void AssignRole(int userId, string role)
        {
        }

        public void GenerateReport(UserFilter filter)
        {
        }
    }

    public class MobileAppService : IUserCRUD, IUserSecurity
    {
        // Только CRUD + Security
        public void CreateUser(User user)
        {
        }

        public void UpdateUser(User user)
        {
        }

        public void DeleteUser(int id)
        {
        }

        public void LockUser(int id)
        {
        }

        public void UnlockUser(int id)
        {
        }

        public void ResetPassword(int id)
        {
        }
    }
    
    /*
    Реальный пример ISP
    
    IAsyncDisposable и IDisposable
    */
        
    public class ResourceManager : IAsyncDisposable, IDisposable
    {
        public void Dispose() { /* Синхронное освобождение */ }
    
        public ValueTask DisposeAsync() 
        {
            /* Асинхронное освобождение */
            return ValueTask.CompletedTask;
        }
    }

    // Клиенты используют только нужный интерфейс
    public class SyncClient : IDisposable
    {
        private readonly IDisposable _resource;
        public SyncClient(IDisposable resource) => _resource = resource;

        public void Dispose()
        {
            _resource.Dispose();
        }
    }

    public class AsyncClient : IAsyncDisposable
    {
        private readonly IAsyncDisposable _resource;
        public AsyncClient(IAsyncDisposable resource) => _resource = resource;

        public async ValueTask DisposeAsync()
        {
            await _resource.DisposeAsync();
        }
    }
    
    /*
    Ошибки при применении ISP
    Чрезмерное разделение:
    */
    
    // Антипаттерн: слишком мелкие интерфейсы
    public interface IIdSetter { void SetId(int id); }
    public interface IIdGetter { int GetId(); }
    public interface INameSetter { void SetName(string name); }
    
    /*
    Решение: Группировать связанные методы
    */
        
    public interface IIdentifiable
    {
        int Id { get; set; }
        string Name { get; set; }
    }
    
    /*
    Проверка соблюдения ISP
    
    Методика тестирования:
    1) Для каждого класса-клиента определите:
    - Какие методы интерфейса он вызывает
    - Какие методы никогда не используются

    2) Проверьте:
    - Нет ли пустых реализаций
    - Нет ли выброшенных исключений в реализациях
    - Соответствует ли интерфейс реальным потребностям клиента

    Соблюдение ISP приводит к созданию чистых, фокусированных контрактов, которые делают систему более гибкой, 
    тестируемой и устойчивой к изменениям.
    */
    
}