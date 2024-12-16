<template>
  <div>
    <div class="card mb-3 p-3">
      <div class="card-body">
        <svg class="bi me-2" width="16" height="16">
          <use :xlink:href="'#' + (device.alternateIcon ?? 'thermometer-half')"/>
        </svg>
        <p class="name">
          {{ device.alternateName ?? device.name }}
        </p>
        <p class="fw-bold fs-4 mb-1">
          {{ device.temperature }} &deg;C
        </p> 
        <p class="fs-4 mb-1">
          {{ device.requestedTemperature }} &deg;C
        </p>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff(device)" :disabled="!device.reachable || device.locked">
        </div>
        <input type="range" class="form-range" min="5" max="30" :value="device.requestedTemperature" @change="setRequestedTemperature($event, device)">
        <div class="btn-group" role="group" v-if="device.type == 'fancoil'">
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-1'" autocomplete="off" :checked="device.speed == 'Low'" @change="setSpeed(device, 'Low')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-1'">Low</label>
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-2'" autocomplete="off" :checked="device.speed == 'Medium'" @change="setSpeed(device, 'Medium')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-2'">Medium</label>
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-3'" autocomplete="off" :checked="device.speed == 'High'" @change="setSpeed(device, 'High')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-3'">High</label>
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-4'" autocomplete="off" :checked="device.speed == 'Auto'" @change="setSpeed(device, 'Auto')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-4'">Auto</label>
        </div>
        <div class="btn-group" role="group" v-if="device.type == 'fancoil'">
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-mode'" :id="device.deviceId + '-radio-mode-1'" autocomplete="off" :checked="device.mode == 'Heating'" @change="setMode(device, 'Heating')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-mode-1'">Heating</label>
          <input type="radio" class="btn-check" :name="device.deviceId + '-radio-mode'" :id="device.deviceId + '-radio-mode-2'" autocomplete="off" :checked="device.mode == 'Cooling'" @change="setMode(device, 'Cooling')">
          <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-mode-2'">Cooling</label>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from '../../api'
export default {
  props: ['device'],
  methods: {
    updateOnOff: function (device) {
      if (device.on) {
        api.sendDeviceCommand(device.deviceId, "turnOff")
      } else {
        api.sendDeviceCommand(device.deviceId, "turnOn")
      }
    },
    setRequestedTemperature: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setRequestedTemperature", {
        t: event.target.value
      })
    },
    setMode: function (device, mode) {
      api.sendDeviceCommand(device.deviceId, "setMode", {
        m: mode
      })
    },
    setSpeed: function (device, speed) {
      api.sendDeviceCommand(device.deviceId, "setSpeed", {
        s: speed
      })
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>