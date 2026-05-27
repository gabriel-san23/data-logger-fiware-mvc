#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <DHT.h>
#include <Wire.h>
#include <PubSubClient.h>
// #include "DFRobotDFPlayerMini.h"

LiquidCrystal_I2C lcd(0x27, 16, 2);

const char* default_SSID = "Wokwi-GUEST";
const char* default_PASSWORD = "";

// Variáveis MQTT (Comentadas)
const char* default_BROKER_MQTT = "SEU_IP_AQUI";
const int default_BROKER_PORT = 1883;
const char* TOPICO_SUBSCRIBE = "/TEF/datalogger001/cmd";
const char* TOPICO_PUBLISH = "/TEF/datalogger001/attrs";
const char* TOPICO_CMDEXE = "/TEF/datalogger001/cmdexe";
const char* ID_MQTT = "fiware_001";

const int EMBEDDED_LED = 2;
const int RED_RGB = 19;
const int GREEN_RGB = 18;
const int BLUE_RGB = 5;
const int A0_LDR = 34;
const int SDA_DHT = 32;
const int SDA_LCD = 25;
const int SCL_LCD = 26;

#define DHTTYPE DHT22
DHT dht(SDA_DHT, DHTTYPE);

// Variáveis de Áudio (Comentadas)
// const int busyPin = 4;
// const int rxPin = 16;
// const int txPin = 17;
// const int default_audioVolume = 30;
// HardwareSerial myHardwareSerial(2);
// DFRobotDFPlayerMini myDFPlayer;

float temperatura = 0;
float umidade = 0;
int luminosidade = 0;

unsigned long tempoAnteriorTelemetria = 0;
const unsigned long intervaloTelemetria = 2000; 

unsigned long tempoAnteriorAudio = 0;
const unsigned long intervaloAudio = 20000; 

WiFiClient espClient;
PubSubClient MQTT(espClient);

void setup() {
  pinMode(EMBEDDED_LED, OUTPUT);
  digitalWrite(EMBEDDED_LED, LOW);

  pinMode(RED_RGB, OUTPUT);
  pinMode(GREEN_RGB, OUTPUT);
  pinMode(BLUE_RGB, OUTPUT);

  Wire.begin(SDA_LCD, SCL_LCD); // PINOS LCD I2C
  Serial.begin(9600);

  pinMode(A0_LDR, INPUT);
  dht.begin();

  lcd.init();
  lcd.backlight();
  lcd.print("Hello!");

  // initMP3();
  reconectWiFi();
  initMQTT();
  
  // myDFPlayer.playFolder(6, 5);
  // aguardarAudio();
}

void loop() {
  if (WiFi.status() != WL_CONNECTED) reconectWiFi();
  if (!MQTT.connected()) reconnectMQTT();
  MQTT.loop(); 

  unsigned long tempoAtual = millis();

  if (tempoAtual - tempoAnteriorTelemetria >= intervaloTelemetria) {
    tempoAnteriorTelemetria = tempoAtual;
    readAndPublishSensors(); 
  }

  if (tempoAtual - tempoAnteriorAudio >= intervaloAudio) {    
    // falarGrandezas();     
    tempoAnteriorAudio = millis(); 
  }
}