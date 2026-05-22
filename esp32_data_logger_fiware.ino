#include <LiquidCrystal_I2C.h>
#include <WiFi.h> 
#include <DHT.h> 
#include <PubSubClient.h>
#include <Wire.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

const char* default_SSID = "Wokwi-GUEST";
const char* default_PASSWORD = "";

const char* default_BROKER_MQTT = "SEU_IP_AQUI";
const int default_BROKER_PORT = 1883;
const char* TOPICO_SUBSCRIBE = "/TEF/datalogger001/cmd";
const char* TOPICO_PUBLISH = "/TEF/datalogger001/attrs";
const char* ID_MQTT = "fiware_datalogger";

const int EMBEDDED_LED = 2;
const int RED_RGB = 19;
const int GREEN_RGB = 18;
const int BLUE_RGB = 5; 
const int A0_LDR = 34; 

const int SDA_DHT = 32; 

// MUDANÇA AQUI: Alterado de DHT22 para DHT11
#define DHTTYPE DHT11 

DHT dht(SDA_DHT, DHTTYPE);

float temperatura = 0;
float umidade = 0;
int luminosidade = 0; 

WiFiClient espClient;
PubSubClient MQTT(espClient);

void reconectWiFi() {
  if (WiFi.status() == WL_CONNECTED) return;
  WiFi.begin(default_SSID, default_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }
  Serial.println("WiFi OK");
}

void mqtt_callback(char* topic, byte* payload, unsigned int length) {
  String msg;
  for (int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }
  if (msg == "datalogger001@led_on|") {
    digitalWrite(EMBEDDED_LED, HIGH);
    Serial.println("Comando recebido: LED Ligado");
  } 
  else if (msg == "datalogger001@led_off|") {
    digitalWrite(EMBEDDED_LED, LOW);
    Serial.println("Comando recebido: LED Desligado");
  }
}

void initMQTT() {
  MQTT.setServer(default_BROKER_MQTT, default_BROKER_PORT);
  MQTT.setCallback(mqtt_callback);
}

void reconnectMQTT() {
  while (!MQTT.connected()) {
    if (MQTT.connect(ID_MQTT)) {
      Serial.println("MQTT OK");
      MQTT.subscribe(TOPICO_SUBSCRIBE);
    } else {
      delay(2000);
    }
  }
}

void VerificaConexoes() {
  reconectWiFi();
  if (!MQTT.connected()) reconnectMQTT();
}

void lerEPublicarSensores() {
  temperatura = dht.readTemperature();
  umidade = dht.readHumidity();
  luminosidade = map(analogRead(A0_LDR), 0, 4095, 100, 0);

  if (isnan(temperatura) || isnan(umidade)) {
    Serial.println("Erro na leitura do DHT!");
    return;
  }

  Serial.println("- Leituras Atuais:");
  Serial.print("Luz: "); Serial.print(luminosidade); Serial.println("%");
  Serial.print("Temp: "); Serial.print(temperatura); Serial.println("C");
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Luz: "); lcd.print(luminosidade); lcd.print("%");
  lcd.setCursor(0, 1);
  lcd.print("T:"); lcd.print(temperatura, 1);
  lcd.print(" U:"); lcd.print(umidade, 1);

  String payload = "temperatura|" + String(temperatura) + 
  "|umidade|" + String(umidade) + 
  "|luminosidade|" + String(luminosidade);

  MQTT.publish(TOPICO_PUBLISH, payload.c_str());
  Serial.println("Publicado MQTT: " + payload);
}

void setup() {
  pinMode(EMBEDDED_LED, OUTPUT);
  digitalWrite(EMBEDDED_LED, LOW);
  pinMode(A0_LDR, INPUT); 
  
  pinMode(RED_RGB, OUTPUT);
  pinMode(GREEN_RGB, OUTPUT);
  pinMode(BLUE_RGB, OUTPUT);

  int novo_SDA = 25;
  int novo_SCL = 26;
  Wire.begin(novo_SDA, novo_SCL);
  
  Serial.begin(9600);
  Serial.println("Iniciando ESP32...");

  dht.begin();
  lcd.begin(); // Para o Wokwi: lcd.init() - real begin
  lcd.backlight();
  lcd.print("Iniciando...");

  // reconectWiFi();
  // initMQTT();
}

void loop() {
  int valorBrutoLDR = analogRead(A0_LDR);
  luminosidade = map(valorBrutoLDR, 0, 4095, 100, 0);

  temperatura = dht.readTemperature();
  umidade = dht.readHumidity();

  Serial.print("LDR - Luminosidade: ");
  Serial.print(luminosidade);
  Serial.println("%");

  if (isnan(temperatura) || isnan(umidade)) {
    Serial.println("Erro: Falha ao ler o sensor DHT11!");
  } else {
    Serial.print("DHT - Temperatura: ");
    Serial.print(temperatura);
    Serial.print(" C | Umidade: ");
    Serial.print(umidade);
    Serial.println("%");
  }
  
  delay(2000); 
}