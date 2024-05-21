<template>
  <div>
    <div class="card mb-3 p-3">
      <div class="card-body" :class="{ unreachable: !device.reachable }">
        <svg class="bi me-2" width="16" height="16">
          <use xlink:href="#lightbulb"/>
        </svg>
        <p class="name">
          {{ device.name }}
        </p>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff" :disabled="!device.reachable">
        </div>
        <input type="range" class="form-range" min="0" max="100" :value="device.brightness" v-if="device.brightness != undefined" @change="setBrightness($event, device)">
        <input type="range" class="form-range" min="153" max="500" :value="device.colorTemperature" v-if="device.colorTemperature != undefined" @change="setColorTemperature($event, device)">
      </div>
    </div>
  </div>
</template>

<script>
import api from '../../api'
export default {
  props: ['device'],
  methods: {
    updateOnOff: function () {
      if (this.device.on) {
        api.sendDeviceCommand(this.device.deviceId, "turnOff")
      } else {
        api.sendDeviceCommand(this.device.deviceId, "turnOn")
      }
    },
    setBrightness: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setBrightness", {
        bri: event.target.value
      })
    },
    setColorTemperature: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setColorTemperature", {
        ct: event.target.value
      })
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>