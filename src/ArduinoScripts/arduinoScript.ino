#include "WiFiEsp.h"

char ssid[] = "Tenda_wifi";  // your network SSID (name)
char pass[] = "kukuruza";    // your network password
int status = WL_IDLE_STATUS; // the Wifi radio's status

char server[] = "85b0a3e0.ngrok.io";
int port = 80;
char userId[] = "6fdf3b68-bb2d-4f4b-bddd-765553db3e06";

unsigned long lastConnectionTime = 0;         // last time you connected to the server, in milliseconds
const unsigned long postingInterval = 10000L; // delay between updates, in milliseconds

// Initialize the Ethernet client object
WiFiEspClient client;

const byte numChars = 16;
char receivedChars[numChars];

boolean newData = false;

void setup()
{
    // initialize serial for debugging
    Serial.begin(115200);
    // initialize serial for ESP module
    Serial1.begin(115200);

    // initialize serial for barcode scaner
    Serial2.begin(9600);
    // initialize ESP module
    WiFi.init(&Serial1);

    // check for the presence of the shield
    if (WiFi.status() == WL_NO_SHIELD)
    {
        Serial.println("WiFi shield not present");
        // don't continue
        while (true)
            ;
    }

    // attempt to connect to WiFi network
    while (status != WL_CONNECTED)
    {
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
    // put your main code here, to run repeatedly:

    recvWithStartEndMarkers();
    showNewData();
}

void Send_command(byte command_array[], int lenth)
{

    for (byte i = 0; i < lenth; i++)
    {
        (Serial2.write(command_array[i]));
    }
    Serial.println("");
}

void recvWithStartEndMarkers()
{
    static boolean recvInProgress = false;
    static byte ndx = 0;
    char startEndMarker = 'A';
    char endMarker = 'D';
    char rc;

    while (Serial2.available() > 0 && newData == false)
    {
        rc = Serial2.read();

        if (rc != endMarker)
        {
            receivedChars[ndx] = rc;
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
                Serial.println("receivedChars:");
                Serial.write(receivedChars);
                newData = true;
            }
        }
    }
}
}

void showNewData()
{
    if (newData == true)
    {
        // if there's incoming data from the net connection send it out the serial port
        // this is for debugging purposes only
        while (client.available())
        {
            char c = client.read();
            Serial.println("answer:");
            Serial.write(c);
        }

        // if 10 seconds have passed since your last connection,
        // then connect again and send data
        if (millis() - lastConnectionTime > postingInterval)
        {
            char buf[128];
            snprintf(buf, sizeof buf, "userId=%s&barcode=%s", userId, receivedChars);
            httpPostRequest(buf);
        }
        newData = false;
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
    if (client.connect(server, port))
    {
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
    else
    {
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
