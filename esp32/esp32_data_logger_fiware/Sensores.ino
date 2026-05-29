void readAndPublishSensors() {
  // atributos
  temperatura = dht.readTemperature();
  umidade = dht.readHumidity();
  luminosidade = map(analogRead(A0_LDR), 0, 4095, 100, 0);

  //validação
  if (isnan(temperatura) || isnan(umidade)) {
    Serial.println("Erro: Falha DHT");

    if (digitalRead(busyPin) == HIGH) {
    myDFPlayer.playFolder(6, 8); // "Falha na leitura dos sensores"
    delay(100); 
    }

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
  printSensorValues();
}

void printSensorValues() {
  Serial.print("T: "); Serial.print(temperatura); Serial.println(" C");
  Serial.print("U: "); Serial.print(umidade); Serial.println(" %");
  Serial.print("L: "); Serial.print(luminosidade); Serial.println(" %");
  Serial.println("---");

  lcd.clear(); 
  
  // --- Linha 0 (Superior): Temperatura ---
  lcd.setCursor(0, 0);
  lcd.write(byte(0));         // Ícone 0: Termômetro
  lcd.print(" ");            
  lcd.print(temperatura, 1);  // Temperatura com 1 casa decimal
  lcd.print("C");
  
  // --- Linha 1 (Inferior): Umidade e Luminosidade ---
  lcd.setCursor(0, 1);
  
  // Umidade
  lcd.write(byte(1));         // Ícone 1: Gota
  lcd.print(" ");
  lcd.print(umidade, 1);
  lcd.print("%  ");           // Espaçamento para o próximo ícone
  
  // Luminosidade
  lcd.write(byte(2));         // Ícone 2: Sol
  lcd.print(" ");
  lcd.print(luminosidade);
  lcd.print("%");        
}

void checarLimiares() {
  bool alertaGeral = false;

  // --- TEMPERATURA ---
  if (temperatura > TEMP_MAX || temperatura < TEMP_MIN) {
    if (!alarmeTempAtivo) {         // Só enfileira se antes estava normal
      tocarAudioTemp = true;        // Levanta a flag para o áudio tocar
      alarmeTempAtivo = true;       // "Trava" novos alertas dessa grandeza

      if (temperatura > TEMP_MAX)
        myDFPlayer.playFolder(6, 1); // "Temperatura acima do ideal para a maturação"
      if (temperatura < TEMP_MIN)
      myDFPlayer.playFolder(6, 2); // "Temperatura abaixo do ideal para a maturação"
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
      myDFPlayer.playFolder(6, 3); // "Umidade fora do padrão adequado"
    }
    alertaGeral = true;
  } else {
    alarmeUmidAtivo = false;
  }

  // --- LUMINOSIDADE ---
  if (luminosidade > LUMINOSIDADE_MAX) {
    if (!alarmeLumAtivo) {
      tocarAudioLum = true;
      alarmeLumAtivo = true;
      myDFPlayer.playFolder(6, 4); // "Excesso de luz detectado na câmara"

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