void setup() {
    Serial.begin(115200);
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
}
