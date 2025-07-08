using System.Collections.ObjectModel;
using SOLID.Support;

namespace SOLID;

public class LiskovSubstitutionPrinciple
{
    /*
    Формальное определение:
    "Если для каждого объекта o1 типа S существует объект o2 типа T, такой, что для всех программ P, 
    определенных в терминах T, поведение P не изменяется при подстановке o1 вместо o2, тогда S является подтипом T." 
    — Барбара Лисков, 1987
    
    Проще говоря:
    Объекты производных классов должны быть способны заменять объекты базовых классов без нарушения работы программы. 
    Наследники должны дополнять, а не изменять поведение родителя.
    
    Ключевые аспекты LSP
    1) Контрактное поведение:
    - Предусловия не могут быть усилены в подклассе
    - Постусловия не могут быть ослаблены в подклассе
    - Инварианты базового класса должны сохраняться

    2) Типичные нарушения:
    - Выбрасывание новых исключений в наследниках
    - Возврат значений другого типа
    - Изменение состояния базового класса недопустимым образом
    - Требование дополнительных условий для работы
    
    Примеры
    Пример 1: Классический пример с прямоугольником и квадратом
    
    Нарушение LSP:
    */
    
    public class Rectangle1
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public int Area => Width * Height;
    }

    public class Square1 : Rectangle1
    {
        public override int Width
        {
            set { base.Width = value; base.Height = value; }
        }
    
        public override int Height
        {
            set { base.Width = value; base.Height = value; }
        }
    }

    // Тест, демонстрирующий проблему
    public void TestRectangleArea(Rectangle rect)
    {
        rect.Width = 5;
        rect.Height = 4;
        Console.WriteLine(rect.Area); // Ожидается 20, но для Square будет 16
    }

    /*
    Проблема: Квадрат изменяет ожидаемое поведение сеттеров, нарушая контракт базового класса.
    Решение через LSP:
    */
    
    public interface IShape
    {
        int Area { get; }
    }

    public class Rectangle : IShape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Area => Width * Height;
    }

    public class Square : IShape
    {
        public int Side { get; set; }
        public int Area => Side * Side;
    }
    
    /*
    Пример 2: Система обработки заказов
    
    Нарушение LSP:
    */
    
    public abstract class Order1
    {
        public virtual void Process()
        {
            // Базовая обработка
        }
    
        public virtual bool Validate()
        {
            return true; // Базовая валидация
        }
    }

    public class DiscountOrder1 : Order1
    {
        public override void Process()
        {
            if (!ValidateDiscount())
                throw new InvalidOperationException("Discount not valid");
        
            base.Process();
        }

        private bool ValidateDiscount()
        {
            /* ... */
            return true;
        }
    }

    // Клиентский код
    public void ProcessOrder(Order1 order)
    {
        order.Validate();
        order.Process(); // Может выбросить исключение для DiscountOrder
    }
    
    /*
    Проблема: Клиентский код не ожидает исключений от базового метода Process().

    Решение через LSP:
    */
    
    public abstract class Order
    {
        protected abstract void ValidateCore();
    
        public void Validate()
        {
            // Общая логика валидации
            ValidateCore();
        }
    
        public abstract void Process();
    }

    public class DiscountOrder : Order
    {
        protected override void ValidateCore()
        {
            // Специфичная валидация скидки
        }
    
        public override void Process()
        {
            // Обработка со скидкой
        }
    }
    
    /*
    Пример 3: Коллекции в .NET (реальный пример)
    Соблюдение LSP:
    */

    static IList<int> list = new List<int> { 1, 2, 3 };
    IList<int> readOnly = new ReadOnlyCollection<int>(list);

    // Клиентский код работает с любым IList
    void PrintCollection(IList<int> collection)
    {
        foreach (var item in collection)
        {
            Console.WriteLine(item);
        }
    
        // Проверка на возможность модификации
        if (!collection.IsReadOnly)
        {
            collection.Add(4);
        }
    }
    
    /*
    Правила проектирования по LSP
    
    1) Не изменяйте ожидаемое поведение:
    - Переопределенные методы должны выполнять ту же логическую функцию
    - Результаты должны быть совместимы по типам
    
    2) Не вводите новые исключения:
    - Подклассы не должны генерировать исключения, неизвестные базовому классу
    
    3) Соблюдайте инварианты:
    - Состояние объекта должно оставаться валидным после любого метода

    4) Поддерживайте исторические ограничения:
    - Новые правила не должны запрещать то, что разрешалось базовым классом
    
    Паттерны проектирования для соблюдения LSP

    1) Шаблонный метод (Template Method):
    */
      
    public abstract class DataProcessor
    {
        // Неизменяемый алгоритм
        public void Process()
        {
            LoadData();
            TransformData();
            SaveResult();
        }
    
        protected abstract void LoadData();
        protected abstract void TransformData();
        protected virtual void SaveResult() 
            => Console.WriteLine("Default saving");
    }

    public class CsvProcessor : DataProcessor
    {
        protected override void LoadData() { /* ... */ }
        protected override void TransformData() { /* ... */ }
    }
    
    /*
    2) Стратегия (Strategy):
    */
    
    public interface IExportStrategy
    {
        void Export(Data data);
    }

    public class PdfExport : IExportStrategy {
        public void Export(Data data)
        {
            
        }
    }
    public class ExcelExport : IExportStrategy {
        public void Export(Data data)
        {
            
        }
    }

    public class ReportGenerator
    {
        private readonly IExportStrategy _exporter;
    
        public ReportGenerator(IExportStrategy exporter) 
            => _exporter = exporter;
    
        public void Generate(Data data) 
            => _exporter.Export(data);
    }
    
    /*
    3) Декоратор (Decorator):
    */
        
    public interface INotifier
    {
        void Send(string message);
    }

    public class EmailNotifier : INotifier {
        public void Send(string message)
        {
            
        }
    }

    public class SmsDecorator : INotifier
    {
        private readonly INotifier _inner;
    
        public SmsDecorator(INotifier inner) 
            => _inner = inner;
    
        public void Send(string message)
        {
            _inner.Send(message);
            Console.WriteLine($"SMS: {message}");
        }
    }
    
    /*
    Последствия нарушения LSP
    - Хрупкость кода: Изменения в подклассах ломают клиентский код
    - Непредсказуемость: Поведение системы становится неочевидным
    - Сложность тестирования: Требуются специальные тесты для каждого подкласса

    Нарушение OCP: Приходится модифицировать существующий код для поддержки новых подклассов
    */
    
}