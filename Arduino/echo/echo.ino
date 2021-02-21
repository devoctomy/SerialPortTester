unsigned long startedAt;

void setup() {
    Serial.begin(115200);
    startedAt = millis();
    Serial.print("Begin echo...\n");
}

void loop() {
  String buffer = "";
  while (Serial.available() > 0)
  {
    int incomingByte = Serial.read();
    buffer += (char)incomingByte;
  }
  if(buffer.length() > 0)
  {
    Serial.print(buffer);  
  }

  if(millis() - startedAt >= 5000)
  {
    Serial.print("Waiting for input...");  
    startedAt = millis();
  }
}
