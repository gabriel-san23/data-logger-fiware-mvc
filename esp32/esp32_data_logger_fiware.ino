#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <DHT.h>
#include <Wire.h>
// #include <PubSubClient.h>
// #include "DFRobotDFPlayerMini.h"

// Falta implementar o RGB e integrar devidamente ao FIWARE

LiquidCrystal_I2C lcd(0x27, 16, 2);

const char* default_SSID = "Wokwi-GUEST";
const char* default_PASSWORD = "";

// const char* default_BROKER_MQTT = "SEU_IP_AQUI";
// const int default_BROKER_PORT = 1883;
// const char* TOPICO_SUBSCRIBE = "/TEF/datalogger001/cmd";
// const char* TOPICO_PUBLISH = "/TEF/datalogger001/attrs";
// const char* TOPICO_CMDEXE = "/TEF/datalogger001/cmdexe";
// const char* ID_MQTT = "fiware_datalogger";

const int EMBEDDED_LED = 2;
const int RED_RGB = 19;
const int GREEN_RGB = 18;
const int BLUE_RGB = 5;
const int A0_LDR = 34;

const int SDA_DHT = 32;
// trocar para DHT11 na compilação real
#define DHTTYPE DHT22

DHT dht(SDA_DHT, DHTTYPE);

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
const unsigned long intervaloTelemetria = 2000; // 2 segundos para o FIWARE/LCD

unsigned long tempoAnteriorAudio = 0;
const unsigned long intervaloAudio = 20000; // 20 segundos de silêncio entre as falas

WiFiClient espClient;
// PubSubClient MQTT(espClient);

void reconectWiFi() {
  if (WiFi.status() == WL_CONNECTED) return;
  WiFi.begin(default_SSID, default_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }
  Serial.println("WiFi: OK");
}

/*
void aguardarAudio() {
  delay(70);
  while (digitalRead(busyPin) == LOW) {
    // Mantém o FIWARE escutando enquanto fala
    if (MQTT.connected()) {
      MQTT.loop();
    }
    delay(10);
  }
}

void initMP3() {
  pinMode(busyPin, INPUT);
  myHardwareSerial.begin(9600, SERIAL_8N1, rxPin, txPin);
  
  if (!myDFPlayer.begin(myHardwareSerial)) {
    Serial.println("Erro: DFPlayer");
    while(true);
  }
  
  myDFPlayer.volume(default_audioVolume);
  myDFPlayer.EQ(0);
  delay(500);
  
  myDFPlayer.playFolder(1, 9); // "Hello!" de inicialização
  aguardarAudio();
}

void falarNumero(int valor) {
  if (valor < 0) valor = 0;
  if (valor > 100) valor = 100;

  if (valor == 0) {
    myDFPlayer.playFolder(3, 1);
    aguardarAudio();
  } else if (valor == 100) {
    myDFPlayer.playFolder(3, 21);
    aguardarAudio();
  } else if (valor >= 1 && valor <= 19) {
    myDFPlayer.playFolder(3, valor + 1);
    aguardarAudio();
  } else if (valor >= 20 && valor <= 99) {
    int dezena = valor / 10;
    int unidade = valor % 10;

    myDFPlayer.playFolder(4, dezena - 1);
    aguardarAudio();

    if (unidade > 0) {
      myDFPlayer.playFolder(1, 7);
      aguardarAudio();
      myDFPlayer.playFolder(3, unidade + 1);
      aguardarAudio();
    }
  }
}

void falarGrandezas() {
  // Temperatura: "A temperatura atual é de..." + Numero + "...graus Celsius"
  myDFPlayer.playFolder(5, 1);
  aguardarAudio();
  falarNumero((int)temperatura);
  myDFPlayer.playFolder(5, 2);
  aguardarAudio();

  // Umidade: "A umidade do ar está em..." + Numero + "...por cento"
  myDFPlayer.playFolder(5, 3);
  aguardarAudio();
  falarNumero((int)umidade);
  myDFPlayer.playFolder(1, 6);
  aguardarAudio();

  // Luminosidade: "A intensidade da luz é de..." + Numero + "...por cento"
  myDFPlayer.playFolder(5, 4);
  aguardarAudio();
  falarNumero(luminosidade);
  myDFPlayer.playFolder(1, 6);
  aguardarAudio();

  // Aviso final: "Leitura dos sensores atualizada."
  myDFPlayer.playFolder(5, 5);
  aguardarAudio();
}

void mqtt_callback(char* topic, byte* payload, unsigned int length) {
  String msg;
  for (int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }

  if (msg == "datalogger001@led_on|") {
    digitalWrite(EMBEDDED_LED, HIGH);
    Serial.println("CMD: LED_ON");
    MQTT.publish(TOPICO_CMDEXE, "led_on|led_on_ok");
  }
  else if (msg == "datalogger001@led_off|") {
    digitalWrite(EMBEDDED_LED, LOW);
    Serial.println("CMD: LED_OFF");
    MQTT.publish(TOPICO_CMDEXE, "led_off|led_off_ok");
  }
}

void initMQTT() {
  MQTT.setServer(default_BROKER_MQTT, default_BROKER_PORT);
  MQTT.setCallback(mqtt_callback);
}

void reconnectMQTT() {
  while (!MQTT.connected()) {
    if (MQTT.connect(ID_MQTT)) {
      MQTT.subscribe(TOPICO_SUBSCRIBE);
      myDFPlayer.playFolder(1, 3); // "Conectado ao sistema FIWARE"
      aguardarAudio();
    } else {
      myDFPlayer.playFolder(1, 4); // "Erro de conexao"
      aguardarAudio();
      delay(2000);
    }
  }
}
*/

void readAndPublishSensors() {
  temperatura = dht.readTemperature();
  umidade = dht.readHumidity();
  luminosidade = map(analogRead(A0_LDR), 0, 4095, 100, 0);

  if (isnan(temperatura) || isnan(umidade)) {
    Serial.println("Erro: Falha DHT");
    // myDFPlayer.playFolder(6, 8); // Alerta da Queijaria: Falha na leitura dos sensores
    // aguardarAudio();
    return;
  }

  Serial.print("T: "); Serial.print(temperatura); Serial.println(" C");
  Serial.print("U: "); Serial.print(umidade); Serial.println(" %");
  Serial.print("L: "); Serial.print(luminosidade); Serial.println(" %");
  Serial.println("---");

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("T: "); lcd.print(temperatura, 1); lcd.print("C");
  lcd.setCursor(0, 1);
  lcd.print("U: "); lcd.print(umidade, 1); lcd.print("%  L:"); lcd.print(luminosidade);

  // String payload = "temperatura|" + String(temperatura) + "|umidade|" + String(umidade) + "|luminosidade|" + String(luminosidade);
  // MQTT.publish(TOPICO_PUBLISH, payload.c_str());
}

void setup() {
  pinMode(EMBEDDED_LED, OUTPUT);
  digitalWrite(EMBEDDED_LED, LOW);
  pinMode(A0_LDR, INPUT);

  pinMode(RED_RGB, OUTPUT);
  pinMode(GREEN_RGB, OUTPUT);
  pinMode(BLUE_RGB, OUTPUT);

  Wire.begin(25, 26);
  Serial.begin(9600);

  dht.begin();
  lcd.init();
  lcd.backlight();
  lcd.print("Hello!");

  // initMP3();
  reconectWiFi();
  // initMQTT();
  
  // myDFPlayer.playFolder(6, 5); // Alerta da Queijaria: Iniciando registro de dados
  // aguardarAudio();
}

void loop() {

  // Manutenções
  // if (WiFi.status() != WL_CONNECTED) reconectWiFi();
  // if (!MQTT.connected()) reconnectMQTT();
  // MQTT.loop(); 

  unsigned long tempoAtual = millis();

  // Leitura e publicação a cada 2 seg
  if (tempoAtual - tempoAnteriorTelemetria >= intervaloTelemetria) {
    tempoAnteriorTelemetria = tempoAtual;
    readAndPublishSensors(); 
  }

  // Reprodução auditiva dos dados de 20 em 20 segundos (contando após a finalização)
  if (tempoAtual - tempoAnteriorAudio >= intervaloAudio) {    
    // falarGrandezas();     
    tempoAnteriorAudio = millis(); 
  }
}
