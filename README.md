# ESP32 S3 learning

Ссылки:

- [nanoframework](https://www.nanoframework.net/)
- [nf-interpreter](https://github.com/nanoframework/nf-interpreter)
- [nanoff](https://github.com/nanoframework/nanoFirmwareFlasher)
- [samples](https://github.com/nanoframework/Samples)

## Настройка среды разработки

Установка .NET 8 SDK. [Скачать](https://dotnet.microsoft.com/en-us/download)

Установка nanoff:

```
dotnet tool install -g nanoff
```

В файле `%USERPROFILE%\AppData\Roaming\NuGet\NuGet.Config` обязательно нужно оставить 
только фид `https://api.nuget.org/v3/index.json`, так как с него пакет скачается 
быстрее всего. Если скачивать с Azure Artifacts, то будет долго качаться и в итоге ошибка.

## Подготовка платы

Выполните команду `cd` в корень клонированного репозитория.

Включение режима прошивки:
1. Подключите плату к USB.
2. Поставьте перемычку **BOOT**.
3. Включите/отключите перемычку **RESET**.
4. Плата готока к прошивке.

Чтобы узнать список подключённыз портов выполните:

```
nanoff --listports
```

Прошивка firmware в плату:

```
nanoff --update --target ESP32_S3 --serialport COM7
```

Где, `COM7`, это номер порта. В **BOOT** режиме он всегда отличается.

> Выполнять комманду нужно только с подключённым VPN, так как доступ к образам firmware закрыт из РФ. Если нет VPN, то выполните [действия](firmwares/README.md).

После прошивки уберите перемычку **BOOT** и включите/отключите перемычку **RESET**.

Варианты [firmware](https://github.com/nanoframework/nf-interpreter?tab=readme-ov-file#user-content-esp32_s3-boards) для ESP32 S3:

- **ESP32_S3** - Quad spiram support
- **ESP32_S3_BLE** - Display, BLE, Quad spiram support
- **ESP32_S3_ALL** - Display, BLE, Octal spiram support

### Получение информации об устройстве

Получить информацию о подключённом девайсе:

```
nanoff --platform esp32 --serialport COM31 --devicedetails
```

> Комманда работает только в **BOOT** режиме.

### Плата ушла в цикл перезагрузки

Такое бывает и перезапись firmware не помогает, поэтому нужно загрузить новый рабочий образ программы.

Поставьте перемычку **BOOT** и включите/отключите перемычку **RESET**.

Выполните команду:

```
nanoff --target ESP32_S3 --serialport COM7 --deploy --image "nfapp.bin"
```

После прошивки уберите перемычку **BOOT** и включите/отключите перемычку **RESET**. Теперь можно заливать нужный образ программы.