<template>
  <div>
    <div class="card mb-3 p-3">
      <div class="card-body" :class="{ unreachable: !device.reachable }">
        <svg class="bi me-2" width="16" height="16">
          <use xlink:href="#outlet"/>
        </svg>
        <p class="name">
          {{ device.name }}
        </p>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff" :disabled="!device.reachable || device.locked">
        </div>
        &nbsp;
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
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>