<template>
  {{ device.name }}
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