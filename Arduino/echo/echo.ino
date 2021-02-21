unsigned long startedAt;
bool received;
String buffer;
bool recogniseMarlin = true;

void setup() {
    Serial.begin(115200);
    startedAt = millis();
    buffer = "";
}

void loop() {
  if(Serial.available() > 0)
  {
    int incomingByte = Serial.read();
    if(incomingByte == 10 || incomingByte == 13)
    {
      received = true;
    }
    else
    {
      buffer += (char)incomingByte;
    }
  }
  if(received && buffer.length() > 0)
  { 
    if(recogniseMarlin)
    {
      if(buffer == "M105")
      {
        Serial.print("ok T:-15.00 /0.00 B:-15.00 /0.00 T0:-15.00 /0.00 T1:-15.00 /0.00 @:0 B@:0 @0:0 @1:0\n");
      }
      else
      {
        Serial.print("Unknown command: \"" + buffer + "\"\n");  //Echo back
      }
    }
    else
    {
      Serial.print(buffer + "\n");  //Echo back
    }

    buffer = "";
    received = false;
  }

  if(millis() - startedAt >= 5000)
  {
    //Serial.print("Waiting for input...");  
    startedAt = millis();
  }
}
