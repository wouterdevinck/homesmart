# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.13.0] - 2024-12-21

### Added

- Support for HomeWizard watermeter device
- Support for alternate names and icons for devices in the frontend
- Support for hiding offline devices in the frontend
- Basic support for rebooting and updating self when running on EdgeOS (meta device provider)

### Improved

- Added a REAMDE.md with instructions on how to run the project
- Allow for running without configuration
- Support for running on EdgeOS in qemu

### Fixed

- Adapted to Bosch TRV and Aqara changes in recent Zigbee2MQTT versions

## [1.12.0] - 2024-06-02

### Added

- Hue API v2, including local push

### Changed

- Update to dotnet 8
- Update all dependencies

## [1.11.0] - 2024-05-03

### Added

- Support for Unifi devices
- Support for switching camera on/off
- Support Hue dimmer over Zigbee and use it to control a TRV

### Changed

- Allow Zigbee values to be retained so that they can be displayed right after startup without being written to telemetry
- Improved OpsGenie alert
- EdgeOS bundler in CI/CD

## [1.10.0] - 2023-10-11

### Added

- Basic mobile web app

## [1.9.0] - 2023-09-22

### Added

- Support for Somfy RTS shutters
- Development UI

## [1.8.0] - 2023-06-28

### Added

- Support for Bosch TRV

## [1.7.1] - 2023-06-21

### Fixed

- Several regressions in 1.7.0

## [1.7.0] - 2023-06-17

### Added

- Support for Tuya fancoils
- Support for out-of home through Azure 
- Support LIDL energy measuring plugs
- Support for invoking commands with parameters on REST api
- Support getting secrets from separate config file

### Changed

- Disable control of locked outlets in dashboard

### Fixed

- Bugs in equals causing high volume of hue device updates
- Made hue discovery more robust

## [1.6.0] - 2023-03-03

### Added

- Basic support for rooms & rooms API
- Support for SolarEdge
- API to get telemetry data

### Fixed

- Friendlyid was not filled for Hue devices

## [1.5.0] - 2023-02-06

### Added

- Opsgenie alarm automation

## [1.4.1] - 2023-02-02

### Fixed

- Added null check in HasId

## [1.4.0] - 2023-02-02

### Added

- Support for telemetry into InfluxDB
- Support for Hue extended color lights

## [1.3.1] - 2023-01-28

### Fixed

- Hue updates not coming through

## [1.3.0] - 2023-01-28

### Added

- Hue support

## [1.2.3] - 2023-01-28

### Added

- Update "ago" time on dashboard periodically

### Fixed

- Added "just now", fixing the temperature disappearing from the dashboard

## [1.2.2] - 2023-01-27

### Fixed

- Fix for missing temperature updates

## [1.2.1] - 2023-01-26

### Changed

- Update to dotnet 7
- Update all dependencies

### Fixed

- Don't clear devices when reconnecting

## [1.2.0] - 2022-04-12

### Added

- Partial Hue implementation
- Docker healthcheck
- Zigbee improvements
- Interfaces

## [1.1.0] - 2022-02-08

### Added

- Frontend, commands, temperature, fixes

## [1.0.0] - 2022-02-08

### Added

- MVP: Zigbee switches controlling Logo lights

[1.13.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.13.0
[1.12.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.12.0
[1.11.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.11.0
[1.10.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.10.0
[1.9.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.9.0
[1.8.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.8.0
[1.7.1]: https://github.com/wouterdevinck/homesmart/releases/tag/1.7.1
[1.7.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.7.0
[1.6.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.6.0
[1.5.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.5.0
[1.4.1]: https://github.com/wouterdevinck/homesmart/releases/tag/1.4.1
[1.4.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.4.0
[1.3.1]: https://github.com/wouterdevinck/homesmart/releases/tag/1.3.1
[1.3.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.3.0
[1.2.3]: https://github.com/wouterdevinck/homesmart/releases/tag/1.2.3
[1.2.2]: https://github.com/wouterdevinck/homesmart/releases/tag/1.2.2
[1.2.1]: https://github.com/wouterdevinck/homesmart/releases/tag/1.2.1
[1.2.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.2.0
[1.1.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.1.0
[1.0.0]: https://github.com/wouterdevinck/homesmart/releases/tag/1.0.0