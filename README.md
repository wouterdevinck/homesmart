# Home Smart

## Description

Opinionated smart home automation software designed to run on a Raspberry Pi or in a VM. Control your lights, heating, cooling, shutters and other devices from a single interface. Supports automations and manual control, as well as out-of-home access.

## Features

### Supported devices

Support for devices is implemented through plugins called "device providers". The following devices providers are supported:

#### Philips Hue

Support for select Philips Hue lights and switches, as well as for some IKEA Trådfri lights and other Hue compatible devices.

Implemented by conenecting to the Hue bridge using the Hue REST API v2. Local push notifications are supported through server-sent events from the bridge.

#### Siemens Logo!

Support for swithing outputs on and off.

Implemented by connecting to the Logo! PLC over the local network using Modbus TCP. Local polling is used to detect changes in the PLC state.

#### SolarEdge

Telemetry support for SolarEdge solar inverters.

Cloud polling is used to fetch data from the SolarEdge monitoring REST API.

#### Somfy RTS

Support for controlling Somfy RTS rolling shutters.

Radio bridge running on ESP32 development board with CC1101 radio attached running the [ESPSomfy-RTS](https://github.com/rstrouse/ESPSomfy-RTS) open source project. Connection to the bridge is done using its REST API over the local network. Local push notifications are supported through a WebSocket connection to the ESP32.

#### Tuya

Support for some Tuya devices. Limited to fan coil units for now.

Implemented through local TCP connection to the Wi-Fi connected devices using the Tuya protocol. Local push notifications supported over the same connection.

#### Ubiquiti UniFi

Support for some Ubiquiti UniFi devices like switches, access points and cameras.

Implemented by connecting to the UniFi controller over the local network using the UniFi REST API. Local push notifications supported through a WebSocket connection to the controller.

#### Zigbee

Support for Zigbee devices through the [Zigbee2MQTT](https://www.zigbee2mqtt.io/) project.

Zigbee2MQTT running as separate Docker contaitner. Connection over MQTT, including support for local push notifications.

### Supported automations

* Remote control of heating devices
* Push buttons for controlling lights
* Alerts in OpsGenie (water leak detected, laundry done, etc.)

### Telemetry

Time series data is stored in InfluxDB.

### Out-of-home access

Support for accessing the system from outside the local network with the frondend running as an Azure Static Web App leveraging Azure IoT Hub, Azure Functions and Azure SignalR Service.

## Implementation notes

* Backend implemented in C#, frontend in Vue.js.
* Monolithic backend with all device providers running as a single Docker container.
* Supporting services (InfluxDB, MQTT broker, Zigbee2MQTT, etc.) running as separate Docker containers.
* [EdgeOS](https://github.com/wouterdevinck/edgeos) image for Raspberry Pi 4 and VMs.
* Using Roslyn Source Generators with Scriban templates for generating parts of the device provider code.

## Building

### Backend development

See src/backend. Open the solution in Visual Studio and build.

### Frontend development

See src/frontend.

Running Vite:
```
npm dev
```

### Docker image with backend and frontend

Building:
```
make build
```

Running:
```
make run
```

Pushing to Docker Hub:
```
make push
```

### Azure Static Web App

```
make swa
```

### EdgeOS images

Build EdgeOS online upgrade package:
```
make bundle
```

Build image for Raspberry Pi 4:
```
make factory
```

Build image for PC (x86_64) and run in QEMU:
```
make qemu
```