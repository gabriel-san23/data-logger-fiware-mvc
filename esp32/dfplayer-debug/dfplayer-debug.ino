#include "DFRobotDFPlayerMini.h"

HardwareSerial myHardwareSerial(2);
DFRobotDFPlayerMini myDFPlayer;

// Pinos alinhados com o seu projeto FIWARE
const int rxPin = 16; // conectado ao TX do DFPLAYER
const int txPin = 17; // conectado ao RX do DFPLAYER
const int busyPin = 4; 

// Declaração das funções
void printDetail(uint8_t type, int value);
void aguardarAudio();

void setup() {
  Serial.begin(115200);
  Serial.println("\n--- INICIANDO SUPER DEBUG DO DFPLAYER (COM PINO BUSY) ---");

  // Define o pino BUSY como entrada (para "ouvir" o DFPlayer)
  pinMode(busyPin, INPUT);

  myHardwareSerial.begin(9600, SERIAL_8N1, rxPin, txPin);
  
  Serial.println("Tentando comunicar com o modulo...");

  // O TRUQUE: 'false' no final impede o reset e resolve o travamento inicial
  if (!myDFPlayer.begin(myHardwareSerial, false, false)) {
    Serial.println("\nERRO FATAL: Falha na comunicacao!");
    while (true) { delay(0); }
  }

  Serial.println("\nSUCESSO: Comunicacao estabelecida!");
  
  myDFPlayer.volume(15); 
  myDFPlayer.EQ(0);
  delay(500);
  
  // Usaremos play(1) que é mais a prova de falhas do que playFolder para testes
  Serial.println("\nComando enviado: Tocar a PRIMEIRA musica do cartao SD...");
  myDFPlayer.play(1); 
  
  // Agora usamos a SUA lógica para testar se o pino BUSY está funcionando
  aguardarAudio();
}

void loop() {
  // Fica monitorando se o módulo cospe algum erro (como TimeOut ou WrongStack)
  if (myDFPlayer.available()) {
    printDetail(myDFPlayer.readType(), myDFPlayer.read());
  }
  delay(100);
}

// === A SUA FUNÇÃO, COM ADIÇÃO DE TEXTOS PARA DEBUG ===
void aguardarAudio() {
  delay(150); // Dá um tempinho extra para o módulo reagir e puxar o BUSY pra LOW
  
  if (digitalRead(busyPin) == HIGH) {
    Serial.println("[AVISO] O pino BUSY nao foi para LOW. O audio nao iniciou.");
    Serial.println("  -> Verifique se o cartao SD tem musicas e esta em FAT32.");
    return; // Sai da função para não travar para sempre
  }

  Serial.println("[STATUS] O pino BUSY esta LOW (Musica tocando...)");
  
  while (digitalRead(busyPin) == LOW) {
    // Enquanto for LOW, está tocando. Fica travado aqui até acabar.
    delay(10);
  }
  
  Serial.println("[STATUS] O pino BUSY subiu para HIGH (Musica finalizada!)");
}

// === FUNÇÃO PARA TRADUZIR OS CÓDIGOS DE ERRO DO DFPLAYER ===
void printDetail(uint8_t type, int value) {
  switch (type) {
    case TimeOut:
      Serial.println("ERRO: Tempo esgotado (Time Out)!");
      break;
    case WrongStack:
      Serial.println("ERRO: Erro de Stack (Dados incorretos - Possivel queda de tensao)!");
      break;
    case DFPlayerCardInserted:
      Serial.println("INFO: Cartao SD detectado agora!");
      break;
    case DFPlayerCardRemoved:
      Serial.println("ERRO: Cartao SD removido!");
      break;
    case DFPlayerCardOnline:
      Serial.println("INFO: Cartao SD online e pronto!");
      break;
    case DFPlayerPlayFinished:
      Serial.print("INFO: Terminou de tocar a faixa numero: ");
      Serial.println(value);
      break;
    case DFPlayerError:
      Serial.print("ERRO INTERNO: ");
      switch (value) {
        case Busy:
          Serial.println("Modulo Ocupado");
          break;
        case Sleeping:
          Serial.println("Modulo Dormindo (Sleep)");
          break;
        case SerialWrongStack:
          Serial.println("Comando Serial invalido");
          break;
        case CheckSumNotMatch:
          Serial.println("Falha de CheckSum na comunicacao");
          break;
        case FileIndexOut:
          Serial.println("Numero da faixa fora do limite (Musica nao existe)");
          break;
        case FileMismatch:
          Serial.println("Arquivo ou pasta nao encontrados!");
          break;
        case Advertise:
          Serial.println("Em modo anuncio");
          break;
        default:
          break;
      }
      break;
    default:
      break;
  }
}