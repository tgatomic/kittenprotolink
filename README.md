# KittenProtoLink

## **Background**
I want to create custom hardware to be used with KSA to both get telemetry and to control the game. You can get quite far using HID but the throughput isn't that great and it has many limitations. My aim is to use a controller with USB High-speed interface instead of HID. 

So therefore I wanted to create a lightweight interface for wiring KSA with external hardware. 

So I decided to use protobuf, that is a lightweight message-format, that most languages have libraries to encode/decode. Crucially there is a library called nanopb for C, which is great to use with hardware. Further I've also setup the traffic, so it will limit the amount of traffic to a fixed rate, and only if there is sufficient enough traffic. 

Both reduce load on the hardware. 

## **Why KittenProtoLink?**
- Dodge the latency and API from HID
- High-speed bidirectional control, with support for multiple clients
- Lightweight, only sends the data that actually changed to the clients 
- Support for multiple languages, including low-level embedded hardware
- Fixed 30 ms (later configurable) send loop, with change-threshold that keeps bandwidth and CPU usage low

### Use cases (examples)
- Raspberrypi/ESP32 or similar to receive telemetry or to send commands
- Write a lightweight receiver that pipes the data to USB for USB controllers
- Program on your PC
- Telemetry dashboard
- Button/display hardware
- Custom throttle designs

## **Quickstart**
You need to have python installed and to install protobuf with `pip install protobuf`. Then you can run the included example script by running `python telemetry_client.py`, from your terminal. 

The script will connect to the game on port (47011) with automatic retry. When connected it will:
- Read the telemetry from the game
- After 30 seconds it will set the throttle level to 50% and turn the throttle on

## **Implementation**

### Communication platform
Communication is done via sockets on port 47011, where KSA acts as a server. It supports multiple clients. The send rate is hardcoded to 30ms (`ProtoLink.cs:FrequencyMs`), detached from the game loop. Changes are only sent if they clear thresholds values from `RemoteKsa/KittenProtoLink/KsaWrappers/TelemetryThresholds.cs` (currently placeholder values).

### Protocol
As described in the background, the data is encoded/decoded as protobuf messages. The protobuf file is located in `/proto/ksa_controller.proto`.

## Protobuf generation
In order to encode/decode the data, you must compile the .proto file to the desired language. I've included the generated files for c# and python in the Generated folder (`Generated\ksa_controller_pb2.py`, `Generated\KsaController.cs`) . If you do any changes, you need to generate new files. You need to install [protoc](https://github.com/protocolbuffers/protobuf/releases) and either call it from its installation path, or add it to PATH. I've used version 33.0. Here are the commands I’ve use to generate the files:
- C#: `protoc.exe --csharp_out=Generated --proto_path=proto  proto/ksa_controller.proto`
- Python: `protoc.exe --python_out=Generated --proto_path=proto  proto/ksa_controller.proto`

[Nanopb](https://github.com/nanopb/nanopb) is a great option to use for C. 


## **Planned features:**
- Continue to expand the protobuf with more fields
- Polish the threshold values
- Add configuration
    - Polling-rate 
    - Number of clients accepted
    - Socket
    - IP
- Add Nanopb generated c/h files
- add the possibility to have different send loops depending on data
