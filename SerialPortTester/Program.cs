using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace hello_serialport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Yield();
            var ports = new List<string>(SerialPort.GetPortNames());
            var selectedPort = InputSelection("Please enter a serial port to use", ports, null);

            var port = default(SerialPort);
            var baudRate = InputInteger("Please enter the baud rate to use (Default = 9600)", 9600);
            var dataBits = InputInteger("Please enter the data bits to use (Default = 8)", 8);
            var parity = InputSelection("Please enter the parity to use (Default = None)", new List<string>(Enum.GetNames(typeof(Parity))), Parity.None.ToString());
            var stopBits = InputSelection("Please enter the parity to use (Default = One)", new List<string>(Enum.GetNames(typeof(StopBits))), StopBits.One.ToString());
            var dtrEnable = InputSelection("Enable dtr y/n? (Default = y)", new List<string>(new[] { "Yes", "No" }), "Yes");
            port = new SerialPort(
                selectedPort,
                baudRate,
                Enum.Parse<Parity>(parity, true),
                dataBits,
                Enum.Parse<StopBits>(stopBits, true));

            port.DtrEnable = dtrEnable == "Yes";
            port.Open();
            var cancellationTokenSource = new CancellationTokenSource();
            var readingTask = BeginReadingAsync(port);

            Console.WriteLine("Port opened, type to send data (sent on return)");
            while (true)
            {
                var input = Console.ReadLine();
                if(input == "quit")
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
                var txBuffer = System.Text.Encoding.ASCII.GetBytes($"{input}{GetEndPacket()}");
                await port.BaseStream.WriteAsync(
                    txBuffer,
                    0,
                    txBuffer.Length,
                    CancellationToken.None);
                Console.WriteLine($"TX :: {input}");
            }

            port.Close();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static async Task BeginReadingAsync(SerialPort port)
        {
            await Task.Yield();
            Console.WriteLine("Started async reading from serial port.");

            var rxBuffer = new byte[1024];
            while(true)
            {
                var bytesRead = await port.BaseStream.ReadAsync(
                    rxBuffer,
                    0,
                    rxBuffer.Length,
                    CancellationToken.None);
                if (bytesRead > 0)
                {
                    Console.WriteLine("RX :: " + System.Text.Encoding.ASCII.GetString(rxBuffer, 0, bytesRead));
                }
            }
        }

        private static string GetEndPacket()
        {
            return "\n";
        }

        private static string InputSelection(
            string message,
            List<string> options,
            string defaultValue)
        {
            Console.WriteLine(message);
            foreach (string curOption in options)
            {
                Console.WriteLine($"{options.IndexOf(curOption) + 1} - {curOption}");
            }
            var optionChoiceLine = Console.ReadLine();
            if(!String.IsNullOrEmpty(defaultValue) && string.IsNullOrEmpty(optionChoiceLine))
            {
                return defaultValue;
            }

            int optionChoice = -1;
            while (!int.TryParse(optionChoiceLine, out optionChoice) || (optionChoice < 0 || optionChoice > options.Count))
            {
                foreach (string curOption in options)
                {
                    Console.WriteLine($"{options.IndexOf(curOption) + 1} - {curOption}");
                }
                optionChoiceLine = Console.ReadLine();
                if (!String.IsNullOrEmpty(defaultValue) && string.IsNullOrEmpty(optionChoiceLine))
                {
                    return defaultValue;
                }
            }

            return options[optionChoice - 1];
        }

        private static int InputInteger(
            string message,
            int defaultValue)
        {
            Console.WriteLine(message);
            var choice = Console.ReadLine();
            if(string.IsNullOrEmpty(choice))
            {
                return defaultValue;
            }

            int value = -1;
            while (!int.TryParse(choice, out value))
            {
                Console.Clear();
                Console.WriteLine(message);
                choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice))
                {
                    return defaultValue;
                }
            }

            return value;
        }
    }
}
