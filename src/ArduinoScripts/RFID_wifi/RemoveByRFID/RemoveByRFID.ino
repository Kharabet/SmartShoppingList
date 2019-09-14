// Скетч для отправки запросов на /api/add-unassigned-rfid. Для того чтобы добавить в кеш серверя "висящий" RFID. 
// В последствии его должен привязать к товару пользователь через веб сайт.

#include "MFRC522.h"

#include <ESP8266HTTPClient.h>

#include <ESP8266WiFi.h>

#define RST_PIN 5 // RST-PIN for RC522 - RFID - SPI - Modul GPIO5 
#define SS_PIN 4 // SDA-PIN for RC522 - RFID - SPI - Modul GPIO4 
MFRC522 mfrc522(SS_PIN, RST_PIN); // Create MFRC522 instance

char ssid[] = "Tenda_wifi"; // your network SSID (name)
char password[] = "kukuruza"; // your network password
int status = WL_IDLE_STATUS; // the Wifi radio's status

char server[] = "http://2d91f44c.ngrok.io/api/user-product-to-bin-by-rfid";
int port = 80;
char userId[] = "6fdf3b68-bb2d-4f4b-bddd-765553db3e06";

void setup() {
  Serial.begin(74880); // Initialize serial communications
  SPI.begin(); // Init SPI bus
  mfrc522.PCD_Init(); // Init MFRC522

  ShowReaderDetails(); // Выводим данные о модуле MFRC522

  WiFi.mode(WIFI_OFF); //Prevents reconnection issue (taking too long to connect)
  delay(1000);
  WiFi.mode(WIFI_STA); //This line hides the viewing of ESP as wifi hotspot

  WiFi.begin(ssid, password); //Connect to your WiFi router
  Serial.println("");

  Serial.print("Connecting");
  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  //If connection successful show IP address in serial monitor
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP()); //IP address assigned to your ESP

}

void loop() {
  // Look for new cards
  if (!mfrc522.PICC_IsNewCardPresent()) {
    return;
  }
  // Select one of the cards
  if (!mfrc522.PICC_ReadCardSerial()) {
    return;
  }

  // Выводим данные с карты
  mfrc522.PICC_DumpToSerial( & (mfrc522.uid));

  // Show some details of the PICC (that is: the tag/card)
  Serial.print(F("Card UID:"));
  dump_byte_array(mfrc522.uid.uidByte, mfrc522.uid.size);
  Serial.println();

  if (WiFi.status() == WL_CONNECTED) { //Check WiFi connection status

    HTTPClient http; //Declare object of class HTTPClient

    http.begin(server); //Specify request destination
    http.addHeader("Content-Type", "application/x-www-form-urlencoded"); //Specify content-type header

    unsigned long UID_unsigned;
    UID_unsigned = mfrc522.uid.uidByte[0] << 24;
    UID_unsigned += mfrc522.uid.uidByte[1] << 16;
    UID_unsigned += mfrc522.uid.uidByte[2] << 8;
    UID_unsigned += mfrc522.uid.uidByte[3];

    Serial.println();
    Serial.println("UID Unsigned int");
    Serial.println(UID_unsigned);

    String UID_string = (String) UID_unsigned;

    //Post Data
    String postData = "rfid=" + UID_string;
    Serial.print("POST data: ");
    Serial.println(postData);

    int httpCode = http.POST(postData); //Send the request
    String payload = http.getString(); //Get the response payload

    Serial.print("httpCode: ");
    Serial.println(httpCode); //Print HTTP return code

    if (httpCode > 0) {
      Serial.printf("HTTP POST ... code: %d\n", httpCode);
      Serial.print("payload: ");
      Serial.println(payload); //Print request response payload

    } else {
      Serial.printf("HTTP POST failed, error: %s\n", http.errorToString(httpCode).c_str());
    }

    http.end(); //Close connection

  } else {

    Serial.println("Error in WiFi connection");

  }
}

// Helper routine to dump a byte array as hex values to Serial
void dump_byte_array(byte * buffer, byte bufferSize) {
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(buffer[i] < 0x10 ? " 0" : " ");
    Serial.print(buffer[i], HEX);
  }
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
