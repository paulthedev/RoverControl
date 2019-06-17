#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>

#define SERVICE_UUID        "adeff3c9-7d59-4470-a847-da82025400e2"
#define CHARACTERISTIC_UUID "716e54c6-edd1-4732-bde1-aade233caeaa"


uint8_t PWMRear = 16;
uint8_t RearIn1 = 17;
uint8_t RearIn2 = 18;

uint8_t PWMFront = 19;
uint8_t FrontIn1 = 21;
uint8_t FrontIn2 = 22;

uint8_t ENMotorDriver = 23;

class MyCallbacks: public BLECharacteristicCallbacks {
    void onRead(BLECharacteristic *pCharacteristic) {
      pCharacteristic->setValue("70");
    }
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value = pCharacteristic->getValue();
   
      //Forward
      if(value[0] == '1' && value[1] == '0'){
        digitalWrite(RearIn1, HIGH);
        digitalWrite(RearIn2, LOW);
        Serial.println("Forward");
      }
      
      //Backward
      if(value[0] == '0' && value[1] == '1'){
        digitalWrite(RearIn1, LOW);
        digitalWrite(RearIn2, HIGH);
        Serial.println("Backward");
      }

      //Rear Motor Stall
      if(value[0] == '0' && value[1] == '0'){
        digitalWrite(RearIn1, LOW);
        digitalWrite(RearIn2, LOW);
        Serial.println("Rear Stall");
      }

      //Right
      if(value[2] == '0' && value[3] == '1'){
        digitalWrite(FrontIn1, HIGH);
        digitalWrite(FrontIn2, LOW);
        Serial.println("Right");
      }
      
      //Left
      if(value[2] == '1' && value[3] == '0'){
        digitalWrite(FrontIn1, LOW);
        digitalWrite(FrontIn2, HIGH);
        Serial.println("Left");
      }

      //Front Motor Stall
      if(value[2] == '0' && value[3] == '0'){
        digitalWrite(FrontIn1, LOW);
        digitalWrite(FrontIn2, LOW);
        Serial.println("Front Stall");
      }

      if(value[4] == '1'){
        //Headlights on
      }
      if(value[4] == '0'){
        //Headlights off
      }
      
      //Rear Motor Accleration
      String acc = "";
      acc += value[5];
      acc += value[6];
      acc += value[7];
      ledcWrite(1, acc.toInt());
      Serial.println(acc);

    }    
};

class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      digitalWrite(ENMotorDriver, LOW);
      Serial.println("connected");
    }
    
    void onDisconnect(BLEServer* pServer) {
      digitalWrite(ENMotorDriver, HIGH);
      Serial.println("disconnected");
    }
};

void setup() {
  Serial.begin(115200);

  BLEDevice::init("Rover");
  BLEDevice::setPower(ESP_PWR_LVL_P7);
  BLEServer *pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  BLEService *pService = pServer->createService(SERVICE_UUID);

  BLECharacteristic *pCharacteristic = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_READ |
                                         BLECharacteristic::PROPERTY_WRITE
                                       );

  pCharacteristic->setCallbacks(new MyCallbacks());
  pService->start();

  BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pAdvertising->setScanResponse(true);
  pAdvertising->setMinPreferred(0x06);  // functions that help with iPhone connections issue
  pAdvertising->setMinPreferred(0x12);
  BLEDevice::startAdvertising();
  pAdvertising->start();

  ledcAttachPin(PWMRear, 1); // assign Motor PWM pins to channels
  ledcAttachPin(PWMFront, 2);
  ledcSetup(1, 12000, 8); // 12 kHz PWM, 8-bit resolution
  ledcSetup(2, 12000, 8);
  ledcWrite(2, 255);

  pinMode(RearIn1, OUTPUT);
  pinMode(RearIn2, OUTPUT);
  pinMode(FrontIn1, OUTPUT);
  pinMode(FrontIn2, OUTPUT);
  pinMode(ENMotorDriver, OUTPUT);
}

void loop() {
  delay(2000);
}
