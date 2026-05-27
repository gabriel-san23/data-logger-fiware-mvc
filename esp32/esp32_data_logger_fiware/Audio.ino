/*
void aguardarAudio() {
  delay(70);
  while (digitalRead(busyPin) == LOW) {
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
  
  myDFPlayer.playFolder(1, 9); 
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
  myDFPlayer.playFolder(5, 1);
  aguardarAudio();
  falarNumero((int)temperatura);
  myDFPlayer.playFolder(5, 2);
  aguardarAudio();

  myDFPlayer.playFolder(5, 3);
  aguardarAudio();
  falarNumero((int)umidade);
  myDFPlayer.playFolder(1, 6);
  aguardarAudio();

  myDFPlayer.playFolder(5, 4);
  aguardarAudio();
  falarNumero(luminosidade);
  myDFPlayer.playFolder(1, 6);
  aguardarAudio();

  myDFPlayer.playFolder(5, 5);
  aguardarAudio();
}
*/