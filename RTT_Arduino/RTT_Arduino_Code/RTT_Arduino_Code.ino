//setup libraries
#include <WiFiS3.h>
#include <WiFiUdp.h>
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>
#include <SCServo.h>
#include "Arduino_LED_Matrix.h"

//Setup led matrix to show wifi status on arduino
ArduinoLEDMatrix matrix;

//Wifi buffering display
byte WifiBuffer[8][12] = {
  { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
  { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0 },
  { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
};

//wifi successfully connected display
byte WifiConnected[8][12] = {
  { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0 },
  { 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0 },
  { 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0 },
  { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
};

//IP successfully connected display
byte IPConnected[8][12] = {
  { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0 },
  { 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0 },
  { 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0 },
  { 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0 },
  { 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
};


//give arduino access to wifi
char ssid[] = "#######";
char pass[] = "#######";
//char ssid[] = "#######";
//char pass[] = "#######";


//setup communications
unsigned int localPort = 5005; //open port for arduino
//const char* unityIP = "192.168.68.111"; //tell router IP address
unsigned int unityPort = 5006; //setup unity port

WiFiUDP Udp;
IPAddress unityIP;
bool hasConnectedToUnity = false;

char packetBuffer[255]; //buffer for incoming data

//Servo setup
Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();

//Serial bus Setup
SMS_STS sms_sts;

int change = 0;

#define SERVOMIN 150
#define SERVOMAX 600
#define BUSMIN 0
#define BUSMAX 4095

#define upNeckPin 0
#define lowNeckPin 1
#define testPin 2
#define elbowPin 3
#define shoulderRollPin 4

#define inputPin 7

void setup() {
  // start serials
  Serial.begin(115200);
  Serial1.begin(1000000);

  //initialize led matrix
  matrix.begin();

  //initialize pca board
  pwm.begin();
  pwm.setOscillatorFrequency(27000000);
  pwm.setPWMFreq(50);

  //intitialize serial bus adapter
  sms_sts.pSerial = &Serial1;

  //setup pins
  pinMode(inputPin, INPUT);

  //wait for arduino to connect to wifi
  Serial.print("Connecting to Wi-Fi...");
  matrix.renderBitmap(WifiBuffer, 8, 12);
  while (WiFi.begin(ssid, pass) != WL_CONNECTED){
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected to Wi-Fi!");

  Serial.print("Waiting for IP address assignment...");
  matrix.renderBitmap(WifiConnected, 8, 12);
  while (WiFi.localIP().toString() == "0.0.0.0")
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nIP assigned successfully!");
  matrix.renderBitmap(IPConnected, 8, 12);

  //Print Arduino's IP Address
  Serial.print("Arduino IP Address: ");
  Serial.println(WiFi.localIP());

  //Start listening for packets
  Udp.begin(localPort);

}

void loop() {
  //check for incoming packets from unity
  int packetSize = Udp.parsePacket();

  if (packetSize)
  {
    //get unitys IP
    unityIP = Udp.remoteIP();
    hasConnectedToUnity = true;

    int len = Udp.read(packetBuffer, 254);
    if (len > 0)
    {
      packetBuffer[len] = 0; //if null terminate string
    }

    //assign unity data in form of "[angle1],[angle2]" to data
    String data = String(packetBuffer);
    int aIndex = data.indexOf('a'); //find comma
    int bIndex = data.indexOf('b');
    int cIndex = data.indexOf('c');
    int dIndex = data.indexOf('d');
    int eIndex = data.indexOf('e');

    //assign angles based on comma
    if (aIndex != -1 && bIndex != -1 && cIndex != -1 && dIndex != -1 && eIndex != -1)
    {
      String neckYawRead = data.substring(0, aIndex);
      String neckPitchRead = data.substring(aIndex + 1, bIndex);
      String servo3Read = data.substring(bIndex + 1, cIndex);
      String elbowRead = data.substring(cIndex + 1, dIndex);
      String shoulderRollRead = data.substring(dIndex + 1, eIndex);
      String rShoulderPitchRead = data.substring(eIndex + 1);

      //set angle to ints
      int neckYaw = constrain(neckYawRead.toInt(), 0, 180);
      int neckPitch = constrain(neckPitchRead.toInt(), 0, 180);
      int servo3Angle = constrain(servo3Read.toInt(), 0, 180);
      int elbowAngle = constrain(elbowRead.toInt(), 0, 180);
      int shoulderRoll = constrain(shoulderRollRead.toInt(), 0, 180);
      int rShoulderPitch = constrain(rShoulderPitchRead.toInt(), 0, 360);

      //activate servos
      int pulse1 = map(neckYaw, 0, 180, SERVOMIN, SERVOMAX);
      int pulse2 = map(neckPitch, 0, 180, SERVOMIN, SERVOMAX);
      int pulse3 = map(servo3Angle, 0, 180, SERVOMIN, SERVOMAX);
      int pulse4 = map(elbowAngle, 0, 180, SERVOMIN, SERVOMAX);
      int pulse5 = map(shoulderRoll, 0, 180, SERVOMIN, SERVOMAX);
      int pos1 = map(rShoulderPitch, 0, 360, BUSMIN, BUSMAX);

      pwm.setPWM(upNeckPin, 0, pulse1);
      pwm.setPWM(lowNeckPin, 0, pulse2);
      pwm.setPWM(testPin, 0, pulse3);
      pwm.setPWM(elbowPin, 0, pulse4);
      pwm.setPWM(shoulderRollPin, 0, pulse5);
      sms_sts.WritePosEx(1, pos1, 2400, 50);
    }
  }

  //Send data to unity
  static unsigned long lastSendTime = 0;
  if (millis() - lastSendTime > 50)
  {
    if (hasConnectedToUnity)
    {
      int sensorState = digitalRead(inputPin);

      Udp.beginPacket(unityIP, unityPort);
      Udp.print(sensorState);
      Udp.endPacket();
    }

    lastSendTime = millis();
  }
}
