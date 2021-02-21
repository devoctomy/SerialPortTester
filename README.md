# SerialPortTester

Extremely basic tool for testing coms to and from a serial port using .net core.

Usage:

Run the application, and you will be given a selection of serial ports that have been enumerated from your system.

Once you have made a choice you will be given the option to open the port as-is, or reconfigure.

Once the port is open, strings can be sent by typing and pressing return.  A new line is appended to the string that is sent to the serial port.  This is not configurable yet so if you want to change this you will need to edit the source.