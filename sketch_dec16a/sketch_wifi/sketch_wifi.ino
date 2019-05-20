
// Basic serial communication with ESP8266
// Uses serial monitor for communication with ESP8266
//
//  Pins
//  Arduino pin 2 (RX) to ESP8266 TX
//  Arduino pin 3 to voltage divider then to ESP8266 RX
//  Connect GND from the Arduiono to GND on the ESP8266
//  Pull ESP8266 CH_PD HIGH
//
// When a command is entered in to the serial monitor on the computer 
// the Arduino will relay it to the ESP8266
//
// TODO: change user config
String ssid     = "Tenda_wifi"; //Wifi SSID
String password = "kukuruza"; //Wifi Password

String host = "192.168.0.103";  // host
String port = "5000";      // port

int AT_cmd_time; 
boolean AT_cmd_result = false; 
void setup() 
{
    Serial.begin(115200);     // communication with the host computer
    
     // open serial 
  Serial.println("*****************************************************");
  Serial.println("********** Program Start : Connect Arduino WiFi to AskSensors");
  Serial2.begin(115200);
  Serial.println("Initiate AT commands with ESP8266 ");
  sendATcmd("AT",5,"OK");
  sendATcmd("AT+CWMODE=1",5,"OK");
  Serial.print("Connecting to WiFi:");
  Serial.println(ssid);
  sendATcmd("AT+CWJAP=\""+ ssid +"\",\""+ password +"\"",20,"OK");
}
 
void loop() 
{
  if ( Serial2.available() )   {  Serial.write( Serial2.read() );  }
 
    // listen for user input and send it to the ESP8266
    if ( Serial.available() )       {  

      Serial2.write( Serial.read() );  }
   // Create the URL for the request
  //String url = "GET /api/Common HTTP/1.1\r\nHost: 192.168.0.103\r\n\r\n";
  
  String url = "POST /api/Common HTTP/1.1\r\n";
  url+="Host: 192.168.0.103\r\n";
  url+="content-type: application/json\r\n";
  url+="content-length: 10\r\n\r\n";
  url+="\"sadfsadf\"";
      
  Serial.println("*****************************************************");
  Serial.println("********** Open TCP connection ");
  
  sendATcmd("AT+CIPSTART=\"TCP\",\"" + host +"\"," + port, 20, "OK");
  sendATcmd("AT+CIPSEND=" + String(url.length()) + "\r\n", 10, ">");
  
  Serial.print("********** requesting URL: ");
  Serial.println(url);
  Serial2.print(url);
  
  bool FAIL = true;
  do{
    if(Serial2.find("SEND OK")) {
      Serial.println("Data send");
      FAIL = false;
    }
  }
  while(FAIL);
  
  delay(2000);
  sendATcmd("AT+CIPCLOSE", 10, "OK");
  
  Serial.println("********** Close TCP Connection ");
  Serial.println("*****************************************************");
  
  delay(20000);   // delay

  if(Serial2.available()>0)
  {
     Serial.println(Serial2.read());//output in hex ascii nubmers
     
   }
}

// sendATcmd
void sendATcmd(String AT_cmd, int AT_cmd_maxTime, char readReplay[]) {
  Serial.print("AT command:");
  Serial.println(AT_cmd);

  while(AT_cmd_time < (AT_cmd_maxTime)) {
    Serial2.println(AT_cmd);
    if(Serial2.find(readReplay)) {
      AT_cmd_result = true;
      break;
    }

  
    AT_cmd_time++;
  }
  Serial.print("...Result:");
  if(AT_cmd_result == true) {
    Serial.println("DONE");
    AT_cmd_time = 0;
  }
  
  if(AT_cmd_result == false) {
    Serial.println("FAILED");
    AT_cmd_time = 0;
  }
  
  AT_cmd_result = false;
 }
