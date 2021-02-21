using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace hello_serialport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Yield();
            var ports = new List<string>(SerialPort.GetPortNames());
            var selectedPort = InputSelection("Please enter a serial port to use", ports);
            var changePortSettings = InputSelection("Do you want co change the port settings?", new List<string>(new[] { "Yes", "No" }));

            var port = default(SerialPort);
            if(changePortSettings == "Yes")
            {
                var baudRate = InputInteger("Please enter the baud rate to use");
                var dataBits = InputInteger("Please enter the data bits to use");
                var parity = InputSelection("Please enter the parity to use", new List<string>(Enum.GetNames(typeof(Parity))));
                var stopBits = InputSelection("Please enter the parity to use", new List<string>(Enum.GetNames(typeof(StopBits))));
                port = new SerialPort(
                    selectedPort,
                    baudRate,
                    Enum.Parse<Parity>(parity, true),
                    dataBits,
                    Enum.Parse<StopBits>(stopBits, true));
            }
            else
            {
                port = new SerialPort(selectedPort);
            }

            port.Open();

            AsyncCallback callback = null;
            var rxBuffer = new byte[1024];
            callback = ar => {
                int bytesRead = port.BaseStream.EndRead(ar);
                Console.WriteLine("RX :: " + System.Text.Encoding.ASCII.GetString(rxBuffer, 0, bytesRead));
                port.BaseStream.BeginRead(rxBuffer, 0, rxBuffer.Length, callback, null);
            };
            port.BaseStream.BeginRead(rxBuffer, 0, rxBuffer.Length, callback, null);
            Console.WriteLine("Port opened, type to send data (sent on return)");

            while (true)
            {
                var input = Console.ReadLine();
                if(input == "quit")
                {
                    break;
                }
                var txBuffer = System.Text.Encoding.ASCII.GetBytes($"{input}\n");
                await port.BaseStream.WriteAsync(txBuffer, 0, txBuffer.Length);
                Console.WriteLine($"TX :: {input}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static string InputSelection(
            string message,
            List<string> options)
        {
            Console.WriteLine(message);
            foreach (string curOption in options)
            {
                Console.WriteLine($"{options.IndexOf(curOption) + 1} - {curOption}");
            }
            var optionChoiceLine = Console.ReadLine();
            int optionChoice = -1;
            while (!int.TryParse(optionChoiceLine, out optionChoice) || (optionChoice < 0 || optionChoice > options.Count))
            {
                foreach (string curOption in options)
                {
                    Console.WriteLine($"{options.IndexOf(curOption) + 1} - {curOption}");
                }
                optionChoiceLine = Console.ReadLine();
            }

            return options[optionChoice - 1];
        }

        private static int InputInteger(string message)
        {
            Console.WriteLine(message);
            var choice = Console.ReadLine();
            int value = -1;
            while (!int.TryParse(choice, out value))
            {
                Console.Clear();
                Console.WriteLine(message);
                choice = Console.ReadLine();
            }

            return value;
        }
    }
}
