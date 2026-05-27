// WiFi
void reconectWiFi() {
  if (WiFi.status() == WL_CONNECTED) return;
  WiFi.begin(default_SSID, default_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }
  Serial.println("WiFi: OK");
}

// MQTT
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
      //myDFPlayer.playFolder(1, 3); 
      //aguardarAudio();
    } else {
      //myDFPlayer.playFolder(1, 4); 
      //aguardarAudio();
      delay(2000);
    }
  }
}