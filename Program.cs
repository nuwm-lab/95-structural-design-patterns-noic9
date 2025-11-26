using System;
using System.Collections.Generic;

namespace AdapterPatternExample
{
    // Інтерфейс для цифрового сигналу (Target)
    public interface IDigitalSignal
    {
        List<int> GetDigitalData();
        void DisplayDigitalSignal();
    }

    // Клас аналогового сигналу (Adaptee)
    public class AnalogSignal
    {
        private double[] analogData;

        public AnalogSignal(double[] data)
        {
            analogData = data;
        }

        public double[] GetAnalogData()
        {
            return analogData;
        }

        public void DisplayAnalogSignal()
        {
            Console.WriteLine("Аналоговий сигнал:");
            foreach (var value in analogData)
            {
                Console.WriteLine($"  Значення: {value:F2} В");
            }
        }
    }

    // Адаптер (Adapter)
    public class AnalogToDigitalAdapter : IDigitalSignal
    {
        private AnalogSignal analogSignal;
        private int bitResolution;

        public AnalogToDigitalAdapter(AnalogSignal signal, int bitResolution = 8)
        {
            analogSignal = signal;
            this.bitResolution = bitResolution;
        }

        // Перетворення аналогового сигналу на цифровий
        public List<int> GetDigitalData()
        {
            List<int> digitalData = new List<int>();
            double[] analogData = analogSignal.GetAnalogData();
            
            // Максимальне значення для заданої розрядності
            int maxDigitalValue = (int)Math.Pow(2, bitResolution) - 1;

            foreach (var analogValue in analogData)
            {
                // Нормалізація аналогового значення (припускаємо діапазон 0-5В)
                double normalizedValue = Math.Max(0, Math.Min(analogValue, 5.0)) / 5.0;
                
                // Перетворення в цифрове значення
                int digitalValue = (int)(normalizedValue * maxDigitalValue);
                digitalData.Add(digitalValue);
            }

            return digitalData;
        }

        public void DisplayDigitalSignal()
        {
            Console.WriteLine($"\nЦифровий сигнал ({bitResolution}-біт):");
            List<int> digitalData = GetDigitalData();
            
            for (int i = 0; i < digitalData.Count; i++)
            {
                double originalAnalog = analogSignal.GetAnalogData()[i];
                Console.WriteLine($"  Зразок {i + 1}: {digitalData[i]} " +
                    $"(оригінал: {originalAnalog:F2} В, двійковий: {Convert.ToString(digitalData[i], 2).PadLeft(bitResolution, '0')})");
            }
        }

        public int GetBitResolution()
        {
            return bitResolution;
        }

        public void SetBitResolution(int bits)
        {
            bitResolution = bits;
        }
    }

    // Клієнт, який працює з цифровими сигналами
    public class DigitalSignalProcessor
    {
        public void ProcessSignal(IDigitalSignal digitalSignal)
        {
            Console.WriteLine("\n=== Обробка цифрового сигналу ===");
            digitalSignal.DisplayDigitalSignal();
            
            List<int> data = digitalSignal.GetDigitalData();
            double average = 0;
            foreach (var value in data)
            {
                average += value;
            }
            average /= data.Count;
            
            Console.WriteLine($"\nСтатистика:");
            Console.WriteLine($"  Кількість зразків: {data.Count}");
            Console.WriteLine($"  Середнє значення: {average:F2}");
            Console.WriteLine($"  Мінімум: {FindMin(data)}");
            Console.WriteLine($"  Максимум: {FindMax(data)}");
        }

        private int FindMin(List<int> data)
        {
            int min = data[0];
            foreach (var value in data)
            {
                if (value < min) min = value;
            }
            return min;
        }

        private int FindMax(List<int> data)
        {
            int max = data[0];
            foreach (var value in data)
            {
                if (value > max) max = value;
            }
            return max;
        }
    }

    // Демонстрація роботи
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Демонстрація патерну Adapter ===\n");

            // Створюємо аналоговий сигнал (значення в вольтах)
            double[] analogValues = { 0.5, 1.2, 2.8, 3.5, 4.1, 5.0, 2.3, 1.8 };
            AnalogSignal analogSignal = new AnalogSignal(analogValues);

            // Відображаємо аналоговий сигнал
            analogSignal.DisplayAnalogSignal();

            Console.WriteLine("\n--- Використання Адаптера (8-біт) ---");
            // Створюємо адаптер для перетворення на 8-бітний цифровий сигнал
            IDigitalSignal adapter8bit = new AnalogToDigitalAdapter(analogSignal, 8);
            
            // Процесор може працювати тільки з цифровими сигналами
            DigitalSignalProcessor processor = new DigitalSignalProcessor();
            processor.ProcessSignal(adapter8bit);

            Console.WriteLine("\n\n--- Використання Адаптера (12-біт) ---");
            // Створюємо адаптер з вищою розрядністю
            IDigitalSignal adapter12bit = new AnalogToDigitalAdapter(analogSignal, 12);
            processor.ProcessSignal(adapter12bit);

            Console.WriteLine("\n\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
