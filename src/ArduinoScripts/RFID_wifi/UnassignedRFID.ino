#include "MFRC522.h"
#include <ESP8266HTTPClient.h>
#include <ESP8266WiFi.h>

#define RST_PIN	5 // RST-PIN for RC522 - RFID - SPI - Modul GPIO5 
#define SS_PIN	4 // SDA-PIN for RC522 - RFID - SPI - Modul GPIO4 
MFRC522 mfrc522(SS_PIN, RST_PIN);	// Create MFRC522 instance

char ssid[] = "Dekanat";  // your network SSID (name)
char password[] = "317_FICT_Dekanat";    // your network password
int status = WL_IDLE_STATUS; // the Wifi radio's status

char server[] = "https://85b0a3e0.ngrok.io";
int port = 80;
char userId[] = "6fdf3b68-bb2d-4f4b-bddd-765553db3e06";

void setup() {
  Serial.begin(74880);    // Initialize serial communications
  SPI.begin();	         // Init SPI bus
  mfrc522.PCD_Init();    // Init MFRC522

    Serial.println("Initalized");

  WiFi.mode(WIFI_OFF);        //Prevents reconnection issue (taking too long to connect)
  delay(1000);
  WiFi.mode(WIFI_STA);        //This line hides the viewing of ESP as wifi hotspot
  
  WiFi.begin(ssid, password);     //Connect to your WiFi router
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
  Serial.println(WiFi.localIP());  //IP address assigned to your ESP
 
}

void loop() { 
  // Look for new cards
  if (!mfrc522.PICC_IsNewCardPresent()) {
    delay(50);
    return;
  }
  // Select one of the cards
  if (!mfrc522.PICC_ReadCardSerial()) {
    delay(50);
    return;
  }
  // Show some details of the PICC (that is: the tag/card)
  Serial.print(F("Card UID:"));
  dump_byte_array(mfrc522.uid.uidByte, mfrc522.uid.size);
  Serial.println();

  if(WiFi.status()== WL_CONNECTED){   //Check WiFi connection status
  
    HTTPClient http;    //Declare object of class HTTPClient
  
    http.begin(server);      //Specify request destination
    http.addHeader("Content-Type", "application/x-www-form-urlencoded");    //Specify content-type header
    
    //Post Data
    postData = "rfid=" + mfrc522.uid.uidByte;

    int httpCode = http.POST(postData);   //Send the request
    String payload = http.getString();                  //Get the response payload
  
    Serial.println(httpCode);   //Print HTTP return code
    Serial.println(payload);    //Print request response payload
  
    http.end();  //Close connection
  
  }else{
  
      Serial.println("Error in WiFi connection");   
  
  }
 }

// Helper routine to dump a byte array as hex values to Serial
void dump_byte_array(byte *buffer, byte bufferSize) {
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(buffer[i] < 0x10 ? " 0" : " ");
    Serial.print(buffer[i], HEX);
  }
}
