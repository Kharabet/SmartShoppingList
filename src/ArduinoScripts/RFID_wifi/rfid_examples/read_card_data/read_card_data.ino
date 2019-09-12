#include "MFRC522.h"

#define RST_PIN  5 
#define SS_PIN  4

MFRC522 mfrc522(SS_PIN, RST_PIN); // Create MFRC522 instance

void setup() {
 Serial.begin(74880);  // Инициализируем монитор последовательного порта
 while (!Serial);  // Ничего не делаем пока он не открыт (для Arduino на чипе ATMEGA32U4)
 SPI.begin();   // Инициализируем SPI шину
 mfrc522.PCD_Init();  // Инициализируем RFID модуль
 ShowReaderDetails(); // Выводим данные о модуле MFRC522
 Serial.println(F("Scan PICC to see UID, type, and data blocks..."));
}

void loop() {
 // Ищем новую карту
 if (!mfrc522.PICC_IsNewCardPresent()) {
  return;
 }

 // Выбираем одну из карт
 if (!mfrc522.PICC_ReadCardSerial()) {
  return;
 }

 // Выводим данные с карты
 mfrc522.PICC_DumpToSerial(&(mfrc522.uid));
}

void ShowReaderDetails() {
 // Получаем номер версии модуля
 byte v = mfrc522.PCD_ReadRegister(mfrc522.VersionReg);
 Serial.print(F("MFRC522 Software Version: 0x"));
 Serial.print(v, HEX);
 if (v == 0x91)
  Serial.print(F(" = v1.0"));
 else if (v == 0x92)
  Serial.print(F(" = v2.0"));
 else
  Serial.print(F(" (unknown)"));
 Serial.println("");
 // Когда получаем 0x00 или 0xFF, передача данных нарушена
 if ((v == 0x00) || (v == 0xFF)) {
  Serial.println(F("WARNING: Communication failure, is the MFRC522 properly connected?"));
 }
}
