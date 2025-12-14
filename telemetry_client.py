#!/usr/bin/env python3
import asyncio
import sys
import time
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
sys.path.append(str(SCRIPT_DIR / "Generated"))

import ksa_controller_pb2 as pb  # type: ignore

HOST = "127.0.0.1"
PORT = 47011
RETRY_DELAY = 5
PROTOCOL_VERSION = 1

async def _read_envelope(reader: asyncio.StreamReader) -> pb.Envelope:
    length_data = await reader.readexactly(4)
    length = int.from_bytes(length_data, "big")
    payload = await reader.readexactly(length)
    envelope = pb.Envelope()
    envelope.ParseFromString(payload)
    return envelope

def _print_telemetry(telemetry: pb.Telemetry):
    orbit = telemetry.orbit if telemetry.HasField("orbit") else None
    kinematic = telemetry.kinematics if telemetry.HasField('kinematics') else None
    vehicle = telemetry.vehicle if telemetry.HasField("vehicle") else None
    flight_comp = telemetry.flight_comp if telemetry.HasField('flight_comp') else None
    manual = telemetry.manual_state if telemetry.HasField("manual_state") else None
    navball = telemetry.navball if telemetry.HasField("navball") else None

    # if vehicle is not None:
    #     print("\nVehicle")
    #     print(vehicle)

    # if orbit is not None:
    #     print("\nOrbit")
    #     print(orbit)

    # if kinematic is not None:
    #     print("\nKinematics")
    #     print(kinematic)

    # if flight_comp is not None:
    #     print("\nFlight Computer")
    #     print(flight_comp)

    # if navball is not None:
    #     print("\nNavball")
    #     print(navball)

async def reader_task(reader: asyncio.StreamReader):
    while True:
        envelope = await _read_envelope(reader)
        if envelope.HasField("telemetry"):
            _print_telemetry(envelope.telemetry)

_seq = 0

async def toggle_task(writer: asyncio.StreamWriter):
    global _seq
    while True:
        await asyncio.sleep(30)
        _seq += 1
        command = pb.ManualControlCommand(
            timestamp=time.time(),
            command=pb.ManualControlState(
                throttle=0.5,
                translate=pb.TranslationControl(translate_forward=True),
            ),
            actions=[pb.ControlAction.CONTROL_ACTION_TOGGLE_MANUAL_THRUST],
        )
        envelope = pb.Envelope(
            protocol_version=PROTOCOL_VERSION,
            sequence=_seq,
            controls=command,
        )
        data = envelope.SerializeToString()
        try:
            writer.write(len(data).to_bytes(4, "big") + data)
            await writer.drain()
            print("Sent toggle")
        except OSError as exc:
            raise ConnectionError(f"Failed to write to server: {exc}") from exc

async def run_client():
    while True:
        try:
            reader, writer = await asyncio.open_connection(HOST, PORT)
            print(f"Connected to {HOST}:{PORT}")
        except OSError as exc:
            print(f"Unable to connect ({exc}). Retrying in {RETRY_DELAY}s...")
            await asyncio.sleep(RETRY_DELAY)
            continue

        try:
            await asyncio.gather(reader_task(reader), toggle_task(writer))
        except (ConnectionError, asyncio.IncompleteReadError) as exc:
            print(f"Connection lost: {exc}. Reconnecting in {RETRY_DELAY}s...")
        finally:
            writer.close()
            try:
                await writer.wait_closed()
            except Exception:
                pass

        await asyncio.sleep(RETRY_DELAY)

if __name__ == "__main__":
    asyncio.run(run_client())
