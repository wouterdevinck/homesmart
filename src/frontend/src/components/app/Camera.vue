<template>
  <div :key="componentKey">
    <div class="card mb-3 p-3">
      <div class="card-body">
        <svg class="bi me-2" width="16" height="16">
          <use xlink:href="#camera-video"/>
        </svg>
        <p class="name">
          {{ device.name }}
        </p>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff" :disabled="device.busy">
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
  data() {
    return {
      componentKey: 0,
    };
  },
  methods: {
    forceRender() {
      this.componentKey += 1;
    },
    updateOnOff: function () {
      if (this.device.on) {
        api.sendDeviceCommand(this.device.deviceId, "turnOff", {}, () => this.forceRender())
      } else {
        api.sendDeviceCommand(this.device.deviceId, "turnOn", {}, () => this.forceRender())
      }
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>