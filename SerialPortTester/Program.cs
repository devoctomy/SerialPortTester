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
            var baudRate = InputInteger("Please enter the baud rate to use");
            var dataBits = InputInteger("Please enter the data bits to use");
            var parity = InputSelection("Please enter the parity to use", new List<string>( Enum.GetNames(typeof(Parity)) ));
            var stopBits = InputSelection("Please enter the parity to use", new List<string>(Enum.GetNames(typeof(StopBits))));

            var port = new SerialPort(
                selectedPort,
                baudRate,
                Enum.Parse<Parity>(parity, true),
                dataBits,
                Enum.Parse<StopBits>(stopBits, true));
            port.Handshake = Handshake.None;

            port.Open();
            if(port.IsOpen)
            {
                Console.WriteLine("Port opened");
            }
            else
            {
                Console.WriteLine("Failed to open port");
                return;
            }

            port.DataReceived += Port_DataReceived;

            while(true)
            {
                var input = Console.ReadLine();
                if(input == "quit")
                {
                    break;
                }
                port.Write($"{input}\n");
                Console.WriteLine($"TX :: {input}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            Console.WriteLine($"RX :: {port.ReadExisting()}");
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
