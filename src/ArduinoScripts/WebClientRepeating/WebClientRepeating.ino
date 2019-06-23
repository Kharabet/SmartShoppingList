/*
 WiFiEsp example: WebClientRepeating

 This sketch connects to a web server and makes an HTTP request
 using an Arduino ESP8266 module.
 It repeats the HTTP call each 10 seconds.

 For more details see: http://yaab-arduino.blogspot.com/p/wifiesp.html
*/

#include "WiFiEsp.h"


char ssid[] = "Tenda_wifi";            // your network SSID (name)
char pass[] = "kukuruza";         // your network password
int status = WL_IDLE_STATUS;     // the Wifi radio's status

char server[] = "85b0a3e0.ngrok.io";
int port = 80;
char userId[] = "6fdf3b68-bb2d-4f4b-bddd-765553db3e06";

unsigned long lastConnectionTime = 0;         // last time you connected to the server, in milliseconds
const unsigned long postingInterval = 10000L; // delay between updates, in milliseconds

// Initialize the Ethernet client object
WiFiEspClient client;

void setup()
{
  // initialize serial for debugging
  Serial.begin(115200);
  // initialize serial for ESP module
  Serial1.begin(115200);
  // initialize ESP module
  WiFi.init(&Serial1);

  // check for the presence of the shield
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present");
    // don't continue
    while (true);
  }

  // attempt to connect to WiFi network
  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network
    status = WiFi.begin(ssid, pass);
  }

  Serial.println("You're connected to the network");
  
  printWifiStatus();
}

void loop()
{
  // if there's incoming data from the net connection send it out the serial port
  // this is for debugging purposes only
  while (client.available()) {
    char c = client.read();
    Serial.println("answer:");
    Serial.write(c);
  }

  // if 10 seconds have passed since your last connection,
  // then connect again and send data
  if (millis() - lastConnectionTime > postingInterval) {

    char buf[128];
    snprintf(buf, sizeof buf, "userId=%s&barcode=%s", userId, "somebarcode");
    
    
    httpPostRequest(buf);
  }
}

// this method makes a HTTP connection to the server
void httpRequest()
{
  Serial.println();
    
  // close any connection before send a new request
  // this will free the socket on the WiFi shield
  client.stop();

  // if there's a successful connection
  if (client.connect(server, port)) {
    Serial.println("Connecting...");
    
    // send the HTTP PUT request
    client.println(("GET /asciilogo.txt HTTP/1.1"));
    client.println(("Host: arduino.cc"));
    client.println("Connection: close");
    client.println();

    // note the time that the connection was made
    lastConnectionTime = millis();
  }
  else {
    // if you couldn't make a connection
    Serial.println("Connection failed");
  }
}

// this method makes a HTTP connection to the server
void httpPostRequest(char content[])
{
  Serial.println();
     
  // close any connection before send a new request
  // this will free the socket on the WiFi shield
  client.stop();

  // if there's a successful connection
  if (client.connect(server, port)) {
    Serial.println("Connecting...");
    
    // send the HTTP PUT request
    client.println(("POST /api/user-product-to-bin-by-barcode HTTP/1.1"));
    Serial.println(("POST /api/user-product-to-bin-by-barcode HTTP/1.1"));


    char hostStr[128];
    snprintf(hostStr, sizeof hostStr, "Host: %s", server);
    client.println((hostStr));
    client.println("Content-type: application/x-www-form-urlencoded");
    
    char contentLength[128];
    snprintf(contentLength, sizeof contentLength, "Content-Length: %d", strlen(content));
    
    client.println(contentLength);
    client.println();
    client.println(content);
    
    Serial.println((hostStr));
    Serial.println("content-type: application/x-www-form-urlencoded");
    Serial.println(contentLength);
    Serial.println();
    Serial.println(content);

    // note the time that the connection was made
    lastConnectionTime = millis();
  }
  else {
    // if you couldn't make a connection
    Serial.println("Connection failed");
  }
}


void printWifiStatus()
{
  // print the SSID of the network you're attached to
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength
  long rssi = WiFi.RSSI();
  Serial.print("Signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}
