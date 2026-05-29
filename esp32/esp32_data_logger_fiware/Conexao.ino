// MQTT

// Um callback MQTT é uma função assíncrona invocada automaticamente quando
// um cliente recebe uma mensagem de um broker em um tópico subscrito.
void mqtt_callback(char* topic, byte* payload, unsigned int length) {
  String msg;
  for (int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }

  // Ou seja, a variável "msg" recebe comandos externos para serem processados no ESP32.
  // No data logger, não controlamos nada remotamente, apenas monitoramos grandezas.
}

void initMQTT() {
  delay(10);
  Serial.println();
  Serial.println("------Conexao MQTT------");
  MQTT.setServer(default_BROKER_MQTT, default_BROKER_PORT);
  MQTT.setCallback(mqtt_callback);
  reconnectMQTT();
}

void reconnectMQTT() {
  if (!MQTT.connected()) {
    Serial.print("Conectando-se ao broker MQTT...");
  }
  
  int tentativas = 0;
  
  while (!MQTT.connected()) {
    if (MQTT.connect(ID_MQTT)) {
      delay(2000);
      MQTT.subscribe(TOPICO_SUBSCRIBE);
      Serial.println("");
      Serial.println("Conectado ao sistema FIWARE!");
    } else {
      tentativas++;
      Serial.print(".");
      
      // Se ele falhar 10 vezes (aprox. 5 segundos), ele toca o áudio
      if (tentativas >= 10) {
        myDFPlayer.playFolder(1, 4);
        aguardarAudio();
        tentativas = 0; 
      } else {
        delay(500);
      }
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
  if (WiFi.status() == WL_CONNECTED) {
    return;
  }

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