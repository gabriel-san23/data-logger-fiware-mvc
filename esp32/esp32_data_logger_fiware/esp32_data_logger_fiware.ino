#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <DHT.h>
#include <Wire.h>
#include <PubSubClient.h>
#include "DFRobotDFPlayerMini.h"

// Parâmetros informados pelo usuário
const char* default_SSID = "senha";
const char* default_PASSWORD = "wagnerwuo";
#define DEVICE_ID "003" // ID gerado pelo ASP.NET
// deve ser preenchido de acordo com o valor recebido pelo usuário

// Variáveis MQTT
const char* default_BROKER_MQTT = "54.152.188.233";
const int default_BROKER_PORT = 1883;

const char* ID_MQTT = "fiware_" DEVICE_ID; //fiware_001
const char* TOPIC_PREFIX = "datalogger" DEVICE_ID; //datalogger001
const char* TOPICO_SUBSCRIBE = "/TEF/datalogger" DEVICE_ID "/cmd";
const char* TOPICO_PUBLISH = "/TEF/datalogger" DEVICE_ID "/attrs";
const char* TOPICO_CMDEXE = "/TEF/datalogger" DEVICE_ID "/cmdexe";

const int EMBEDDED_LED = 2;
const int RED_RGB = 19;
const int GREEN_RGB = 18;
const int BLUE_RGB = 5;
const int A0_LDR = 34;

LiquidCrystal_I2C lcd(0x27, 16, 2);
const int SDA_LCD = 25;
const int SCL_LCD = 26; 

// Matrizes dos Ícones
byte charTermometro[8] = {B00100, B01010, B01010, B01010, B10001, B10001, B10001, B01110};
byte charGota[8]       = {B00000, B00000, B00100, B01110, B11111, B11111, B01110, B00000};
byte charSol[8]        = {B00000, B00000, B10101, B01110, B11111, B01110, B10101, B00000};

const int SDA_DHT = 32;
#define DHTTYPE DHT11
DHT dht(SDA_DHT, DHTTYPE);

// Variáveis de Áudio
const int busyPin = 4;
const int rxPin = 16;
const int txPin = 17;
const int default_audioVolume = 30;
HardwareSerial myHardwareSerial(2);
DFRobotDFPlayerMini myDFPlayer;

// --- LIMIARES DE ALERTA (TRIGGERS) ---
const float TEMP_MAX = 35.0;       // Temperatura máxima tolerada
const float TEMP_MIN = 15.0;       // Temperatura mínima tolerada
const int UMIDADE_MIN = 30;        // Umidade mínima aceitável
const int LUMINOSIDADE_MAX = 80;    // Nível de luz considerado muito escuro

float temperatura = 0;
float umidade = 0;
int luminosidade = 0;

bool sistemaIniciado = false;

// --- FLAGS DA FILA DE ÁUDIO ---
bool tocarAudioTemp = false;
bool tocarAudioUmid = false;
bool tocarAudioLum = false;

// --- MEMÓRIA DE ESTADO DOS ALERTAS ---
bool alarmeTempAtivo = false;
bool alarmeUmidAtivo = false;
bool alarmeLumAtivo = false;

// --- TEMPOS DA FUNÇÃO MILLIS() ---
unsigned long tempoAnteriorTelemetria = 0;
const unsigned long intervaloTelemetria = 2000; // 2 segundos para o FIWARE/LCD

unsigned long tempoAnteriorAudio = 0;
const unsigned long intervaloAudio = 20000; // 20 segundos de silêncio entre as falas

WiFiClient espClient;
PubSubClient MQTT(espClient);

void initPins() {
  pinMode(EMBEDDED_LED, OUTPUT);
  pinMode(RED_RGB, OUTPUT);
  pinMode(GREEN_RGB, OUTPUT);
  pinMode(BLUE_RGB, OUTPUT);
  pinMode(A0_LDR, INPUT);
}

void initDisplayLCD() {
  Wire.begin(SDA_LCD, SCL_LCD); 
  lcd.init();
  lcd.backlight();
  
  lcd.createChar(0, charTermometro);
  lcd.createChar(1, charGota);
  lcd.createChar(2, charSol);
  
  lcd.clear(); // Limpa resíduos da tela
  
  // Posiciona o "Hello!" centralizado na linha de cima
  lcd.setCursor(5, 0);
  lcd.print("Hello!");
  
  // Posiciona o "Iniciando..." na linha de baixo
  lcd.setCursor(2, 1);
  lcd.print("Iniciando...");
}

void setup() {
  initPins();
  digitalWrite(EMBEDDED_LED, LOW);

  initDisplayLCD();
  Serial.begin(115200);
  dht.begin();

  initMP3();
  initWiFi();
  initMQTT();

  myDFPlayer.playFolder(1, 3); // Conectado ao sistema fiware
  aguardarAudio();

  myDFPlayer.playFolder(6, 5); // "Iniciando registro de dados"
  aguardarAudio();

  Serial.println();
  Serial.println("------ Leitura dos Sensores ------");
  Serial.println("");
}

void loop() {
  reconnectWiFi();
  reconnectMQTT();
  MQTT.loop();

  unsigned long tempoAtual = millis();

  if (tempoAtual - tempoAnteriorTelemetria >= intervaloTelemetria) {
    tempoAnteriorTelemetria = tempoAtual;
    readAndPublishSensors();
  }

  if (tempoAtual - tempoAnteriorAudio >= intervaloAudio) {    
    falarGrandezas();
    tempoAnteriorAudio = millis();
  }

  delay(10);
}