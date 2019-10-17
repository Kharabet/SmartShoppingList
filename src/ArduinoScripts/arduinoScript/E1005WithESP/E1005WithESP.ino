#include "MFRC522.h"

#include <ESP8266HTTPClient.h>

#include <ESP8266WiFi.h>

#include <DNSServer.h>
#include <ESP8266WebServer.h>
#include <WiFiManager.h> //https://github.com/tzapu/WiFiManager

char server[] = "http://arduinoshoppinglist.serveo.net/api/user-product-to-bin-by-barcode";
int port = 80;
char userId[] = "6fdf3b68-bb2d-4f4b-bddd-765553db3e06";

char ssid[] = "Tenda_wifi"; // your network SSID (name)

unsigned long lastConnectionTime = 0;         // last time you connected to the server, in milliseconds
const unsigned long postingInterval = 10000L; // delay between updates, in milliseconds

const byte numChars = 16;
char receivedChars[numChars];

boolean newData = false;

void setup()
{
    // initialize serial for barcode scaner
    Serial.begin(9600);

    Serial1.begin(9600);

    //WiFiManager
    //Local intialization. Once its business is done, there is no need to keep it around
    WiFiManager wifiManager;
    //reset saved settings
    //wifiManager.resetSettings();

    //set custom ip for portal
    //wifiManager.setAPStaticIPConfig(IPAddress(10,0,1,1), IPAddress(10,0,1,1), IPAddress(255,255,255,0));

    //fetches ssid and pass from eeprom and tries to connect
    //if it does not connect it starts an access point with the specified name
    //here  "AutoConnectAP"
    //and goes into a blocking loop awaiting configuration
    wifiManager.autoConnect("AutoConnectAP");

    WiFi.mode(WIFI_OFF); //Prevents reconnection issue (taking too long to connect)
    delay(1000);
    WiFi.mode(WIFI_STA); //This line hides the viewing of ESP as wifi hotspot

    WiFi.begin(); //Connect to your WiFi router

    Serial1.println("");

    Serial1.print("Connecting");
    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial1.print(".");
    }

    //If connection successful show IP address in serial monitor
    Serial1.println("");
    Serial1.print("Connected to ");
    Serial1.println(ssid);
    Serial1.print("IP address: ");
    Serial1.println(WiFi.localIP()); //IP address assigned to your ESP
}

void loop()
{
    // put your main code here, to run repeatedly:

    recvWithStartEndMarkers();
    showNewData();
}

void Send_command(byte command_array[], int lenth)
{

    for (byte i = 0; i < lenth; i++)
    {
        (Serial.write(command_array[i]));
    }
    Serial1.println("");
}

void recvWithStartEndMarkers()
{
    static boolean recvInProgress = false;
    static byte ndx = 0;
    byte startEndMarker = 13;
    byte endMarker = 10;
    byte rc;

    if (Serial.available() > 0 && newData == false)
    {

        rc = Serial.read();
        Serial1.println(receivedChars); //output in hex ascii nubmers
        Serial1.println((char)rc);      //output in hex ascii nubmers

        if (rc != endMarker)
        {
            receivedChars[ndx] = (char)rc;
            ndx++;
            if (ndx >= numChars)
            {
                ndx = numChars - 1;
            }
        }
        else
        {
            if (receivedChars[ndx - 1] == startEndMarker)
            {
                receivedChars[ndx - 1] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                Serial1.println("receivedChars:");
                Serial1.write(receivedChars);

                newData = true;
            }
        }
    }
}

void showNewData()
{
    if (newData == true)
    {
        Serial1.println("Processing new data");

        // if there's incoming data from the net connection send it out the serial port
        // this is for debugging purposes only

        // if 10 seconds have passed since your last connection,
        // then connect again and send data
        //if (millis() - lastConnectionTime > postingInterval)
        //{
        char buf[128];
        snprintf(buf, sizeof buf, "userId=%s&barcode=%s", userId, receivedChars);
        Serial1.println("post buf:");
        Serial1.println(buf);

        httpPostRequest(buf);
        //}
        newData = false;
    }
}

// this method makes a HTTP connection to the server
void httpPostRequest(char content[])
{
    Serial1.println("check wifi status:");
    if (WiFi.status() == WL_CONNECTED)
    { //Check WiFi connection status

        HTTPClient http; //Declare object of class HTTPClient

        http.begin(server);                                                  //Specify request destination
        http.addHeader("Content-Type", "application/x-www-form-urlencoded"); //Specify content-type header

        //Post Data
        String postData = content;

        Serial1.println("post data:");
        Serial1.println(postData);

        int httpCode = http.POST(postData); //Send the request
        String payload = http.getString();  //Get the response payload
        Serial1.print("httpCode: ");
        Serial1.println(httpCode); //Print HTTP return code

        if (httpCode > 0)
        {
            Serial1.printf("HTTP POST ... code: %d\n", httpCode);
            Serial1.print("payload: ");
            Serial1.println(payload); //Print request response payload
        }
        else
        {
            Serial1.printf("HTTP POST failed, error: %s\n", http.errorToString(httpCode).c_str());
        }

        http.end(); //Close connection
    }
    else
    {
        Serial.println("Error in WiFi connection");
    }
}
