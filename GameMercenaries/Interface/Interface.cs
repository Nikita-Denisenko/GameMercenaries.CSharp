using GameMercenaries.models.items;

namespace GameMercenaries.Interface;

public static class Interface
{
    public static void PrintItems(List<Item> items)
    {
        var currentItemNumber = 0;
        
        foreach (var item in items)
        {
            currentItemNumber++;
            Console.WriteLine($"{currentItemNumber}. {item.Name}");
        }    
    }

    public static int GetNumberOfAction(int actionsQuantity)
    {
        while (true)
        {
            // Номер чего(действия, игрока, предмета) вводить, логичнее записать в вышестоящий
            // метод, так как это зависит от контекста.
                
            var input = Console.ReadLine();
            
            if (String.IsNullOrEmpty(input) || !int.TryParse(input, out var number))
            {
                Console.WriteLine("Некорректный ввод! попробуйте ещё раз.");
                Console.WriteLine("Введите номер:");
                continue;
            }

            if (number < 1 || number > actionsQuantity)
            {
                Console.WriteLine("Некорректный ввод! попробуйте ещё раз.");
                Console.WriteLine("Введите номер:");
                continue;
            }

            return number;
        }
    }
}