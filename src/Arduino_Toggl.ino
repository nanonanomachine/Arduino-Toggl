#include <LiquidCrystal.h>

LiquidCrystal lcd(4, 3, 14, 15, 16, 17);
int maxCharacterPerLine = 16;
int nodataCount = 0;

void setup() {
  // LCD initialize
  lcd.begin(16, 2);
  lcd.clear();
  lcd.setCursor(maxCharacterPerLine - 9,0);
  lcd.print("Toggl.com");
  lcd.setCursor(maxCharacterPerLine - 12,1); 
  lcd.print("Time Checker");
  
  // Serial initialize
  Serial.begin(9600);
  delay(5000) ;
}

void loop() {
  int count = 0;
  
  // Check data received 
  if(Serial.available() > 0){
    lcd.setCursor(0, 0);
    nodataCount = 0;
  }
  else if(nodataCount < 6){
    nodataCount++;
  }
  else{
    lcd.clear();
    lcd.setCursor(maxCharacterPerLine - 16,0);
    lcd.print("No data received");
    lcd.setCursor(maxCharacterPerLine - 10,1);
    lcd.print("Waiting...");
  }
  
  // Write data to LCD
  while (Serial.available()) {
    // Line Break
    if(count == maxCharacterPerLine * 2){
      lcd.setCursor(0,0);
    }
    else if(count == maxCharacterPerLine){
      lcd.setCursor(0,1);
    }
    lcd.write(Serial.read());
    count++;
  }
  
  delay(1000);
}

