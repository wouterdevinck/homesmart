<template>
    <div class="card mb-3 p-3">
      <div class="card-body" :class="{ unreachable: !device.reachable }">
        <svg class="bi me-2" width="16" height="16"><use xlink:href="#archive"/></svg>
        {{ device.name }}
        <input type="range" class="form-range mb-3 mt-3" min="0" max="100" :value="device.position" v-if="device.position != undefined" @change="setPosition($event, device)">
        <div class="btn-group" role="group">
          <button type="button" class="btn btn-outline-primary" @click="up($event, device)">Up</button>
          <button type="button" class="btn btn-outline-danger" @click="stop($event, device)">Stop</button>
          <button type="button" class="btn btn-outline-primary" @click="down($event, device)">Down</button>
        </div>
      </div>
    </div>
</template>

<script>
import api from '../../api'
export default {
  props: ['device'],
  methods: {
    setPosition: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveToTarget", {
        t: event.target.value
      })
    },
    up: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveUp")
    },
    stop: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "stop")
    },
    down: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveDown")
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>