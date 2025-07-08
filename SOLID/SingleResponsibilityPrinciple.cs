using SOLID.Support;

namespace SOLID;

public class SingleResponsibilityPrinciple
{
    /*
    Суть принципа:
    Класс должен иметь только одну причину для изменения — то есть отвечать за одну конкретную задачу или функциональность.
    Каждый компонент системы должен фокусироваться на выполнении единственной цели.

    Почему это важно:
    - Упрощение тестирования
    - Повышение читаемости кода
    - Снижение связанности компонентов
    - Упрощение рефакторинга
    - Улучшение повторного использования кода
    */

    public class ProductServiceGlobal
    {
        public void AddProduct(Product product)
        {
            // Проверка бизнес-правил
            if (product.Price <= 0)
            {
                throw new ArgumentException("Invalid price");
            }

            // Логика сохранения в БД
            using var context = new AppDbContext();
            context.Products.Add(product);
            context.SaveChanges();

            // Отправка уведомления
            EmailService.SendEmail("admin@example.com", "New product added");
        }
    }

    /*
    Проблемы:
    - Изменение правил валидации потребует правки класса
    - Смена способа сохранения (например, на файловое хранилище) затронет этот же класс
    - Обновление механизма нотификации повлияет на тот же код
    */

    // Отвечает только за бизнес-логику
    public class ProductValidator
    {
        public void Validate(Product product)
        {
            if (product.Price <= 0)
                throw new ArgumentException("Invalid price");

            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("Name required");
        }
    }

    // Отвечает только за работу с хранилищем
    public class ProductRepository
    {
        public void Save(Product product)
        {
            using var context = new AppDbContext();
            context.Products.Add(product);
            context.SaveChanges();
        }
    }

    // Отвечает только за коммуникации
    public class NotificationService
    {
        public void SendProductAddedNotification(Product product)
        {
            EmailService.SendEmail("admin@example.com", $"New product: {product.Name}");
        }
    }

    // Координирует процессы
    public class ProductService
    {
        private readonly ProductValidator _validator;
        private readonly ProductRepository _repository;
        private readonly NotificationService _notification;

        public ProductService(
            ProductValidator validator,
            ProductRepository repository,
            NotificationService notification)
        {
            _validator = validator;
            _repository = repository;
            _notification = notification;
        }

        public void AddProduct(Product product)
        {
            _validator.Validate(product);
            _repository.Save(product);
            _notification.SendProductAddedNotification(product);
        }
    }
    /*
    Ключевые индикаторы нарушения SRP (условно)
    - Класс имеет более 5-7 методов
    - Методы класса выполняют несвязанные операции
    - При изменении требований приходится менять один класс в разных местах
    - Класс имеет зависимости от множества внешних систем
    - Тесты для класса требуют сложных setup-ов
    */
    
    /*
    Преимущества соблюдения SRP
    - Упрощение отладки: Ошибки локализуются в конкретном компоненте
    - Гибкость: Замена реализации не затрагивает другие компоненты
    - Тестируемость: Компоненты тестируются изолированно
    - Читаемость: Каждый класс имеет четкое назначение
    - Безопасность изменений: Риск сломать существующую функциональность минимален
    */
    
    /*Важно:
     SRP не означает, что каждый класс должен иметь только один метод. 
     Речь идет о единой ответственности, которая может реализовываться несколькими связанными методами. 
     Например, класс CustomerValidator может содержать методы 
     ValidateEmail, ValidatePhone, ValidateAddress — все относятся к единой ответственности "валидация данных".
     */
    
}