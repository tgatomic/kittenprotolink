# kittenprotolink
KittenProtoLink exposes Kitten Space Agency telemetry and controls over a lightweight protobuf socket on port 47011. It runs a fixed 30 ms send loop, only transmits meaningful changes, supports multiple clients. Ideal for custom hardware (Pi/ESP32, STM32 etc), dashboards, and PC tools.
