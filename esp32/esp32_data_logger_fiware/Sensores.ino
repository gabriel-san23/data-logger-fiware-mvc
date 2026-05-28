void readAndPublishSensors() {
  // atributos
  temperatura = dht.readTemperature();
  umidade = dht.readHumidity();
  luminosidade = map(analogRead(A0_LDR), 0, 4095, 100, 0);

  //validação
  if (isnan(temperatura) || isnan(umidade)) {
    Serial.println("Erro: Falha DHT");
    // myDFPlayer.playFolder(6, 8); 
    // aguardarAudio();
    return;
  }

  // Checar se houve transgressão dos limiares
  checarLimiares();

  // Construindo o payload no padrão Ultralight 2.0 exigido pelo FIWARE
  // Formato: object_id|valor|object_id|valor|...
  String payload = "t|" + String(temperatura) + "|h|" + String(umidade) + "|l|" + String(luminosidade);
  MQTT.publish(TOPICO_PUBLISH, payload.c_str());

  Serial.print("Dados publicados no tópico: ");
  Serial.println(payload);
}

void printSensorValues(){
  // Serial
  Serial.print("T: "); Serial.print(temperatura); Serial.println(" C");
  Serial.print("U: "); Serial.print(umidade); Serial.println(" %");
  Serial.print("L: "); Serial.print(luminosidade); Serial.println(" %");
  Serial.println("---");

  // LCD
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("T: "); lcd.print(temperatura, 1); lcd.print("C");
  lcd.setCursor(0, 1);
  lcd.print("U: "); lcd.print(umidade, 1); lcd.print("%  L:"); lcd.print(luminosidade);
}

void checarLimiares() {
  bool alertaGeral = false;

  // --- TEMPERATURA ---
  if (temperatura > TEMP_MAX || temperatura < TEMP_MIN) {
    if (!alarmeTempAtivo) {         // Só enfileira se antes estava normal
      tocarAudioTemp = true;        // Levanta a flag para o áudio tocar
      alarmeTempAtivo = true;       // "Trava" novos alertas dessa grandeza
    }
    alertaGeral = true;
  } else {
    alarmeTempAtivo = false;        // Voltou ao normal, "destrava" o alarme
  }

  // --- UMIDADE ---
  if (umidade < UMIDADE_MIN) {
    if (!alarmeUmidAtivo) {
      tocarAudioUmid = true;
      alarmeUmidAtivo = true;
    }
    alertaGeral = true;
  } else {
    alarmeUmidAtivo = false;
  }

  // --- LUMINOSIDADE ---
  if (luminosidade < LUMINOSIDADE_MIN) {
    if (!alarmeLumAtivo) {
      tocarAudioLum = true;
      alarmeLumAtivo = true;
    }
    alertaGeral = true;
  } else {
    alarmeLumAtivo = false;
  }

  // Feedback Visual no LED RGB
  if (alertaGeral) {
    digitalWrite(RED_RGB, HIGH);
    digitalWrite(GREEN_RGB, LOW);
  } else {
    digitalWrite(RED_RGB, LOW);
    digitalWrite(GREEN_RGB, HIGH);
  }
}