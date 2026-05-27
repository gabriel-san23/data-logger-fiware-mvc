// MQTT
void mqtt_callback(char* topic, byte* payload, unsigned int length) {
  String msg;
  for (int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }

  // Forma o padrão de tópico para comparação
    String onTopic = String(TOPIC_PREFIX) + "@led_on|";
    String offTopic = String(TOPIC_PREFIX) + "@led_off|";

  if (msg.equals(onTopic)) {
    digitalWrite(EMBEDDED_LED, HIGH);
    Serial.println("CMD: LED_ON");
    MQTT.publish(TOPICO_CMDEXE, "led_on|led_on_ok");
  }
  else if (msg.equals(offTopic)) {
    digitalWrite(EMBEDDED_LED, LOW);
    Serial.println("CMD: LED_OFF");
    MQTT.publish(TOPICO_CMDEXE, "led_off|led_off_ok");
  }
}

void initMQTT() {
  delay(10);
  Serial.println();
  Serial.println("------Conexao MQTT------");
  MQTT.setServer(default_BROKER_MQTT, default_BROKER_PORT);
  MQTT.setCallback(mqtt_callback);
}

void reconnectMQTT() {
  if (!MQTT.connected())
  {
    Serial.print("Conectando-se ao broker ");
    Serial.print("'");
    Serial.print(default_BROKER_MQTT); Serial.print(":");
    Serial.print(default_BROKER_PORT);
    Serial.println("'");
    Serial.print("Aguarde.");
  }
  while (!MQTT.connected()) {
    if (MQTT.connect(ID_MQTT)) {
      MQTT.subscribe(TOPICO_SUBSCRIBE);
      Serial.println("");
      Serial.println("Conectado ao sistema FIWARE!");
      //myDFPlayer.playFolder(1, 3); 
      //aguardarAudio();
    } else {
      //myDFPlayer.playFolder(1, 4); 
      //aguardarAudio();
      Serial.print(".");
      delay(2000);
    }
  }
}

// WiFi
void initWiFi() {
  delay(10);
  Serial.println();
  Serial.println("------Conexao WI-FI------");
  Serial.print("Conectando-se na rede: ");
  Serial.println(default_SSID);
  reconnectWiFi();
}

void reconnectWiFi() {
  if (WiFi.status() == WL_CONNECTED)
    return;

  WiFi.begin(default_SSID, default_PASSWORD);
  Serial.print("Aguarde");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println();
  Serial.print("Conectado com sucesso na rede: ");
  Serial.println(default_SSID);
  Serial.print("IP obtido: ");
  Serial.println(WiFi.localIP());
}